using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace LoadTesting
{
    public partial class LoadTesting : Form
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;
        private List<LoadTestResult> allLoadTestResults = new List<LoadTestResult>();
        private List<LoadTestResult> currentTestResults = new List<LoadTestResult>();
        private int currentRoundNumber = 1;
        private List<int> roundNumbers = new List<int>();
        private EvaluationData currentEvaluationData; // Properti untuk menyimpan data evaluasi saat ini
        private List<EvaluationData> allEvaluatedData = new List<EvaluationData>(); // Koleksi semua data yang telah dievaluasi
        private int currentEvaluationRoundNumber = 1;

        public LoadTesting()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            textBoxInputRequest.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);
            textBoxInputTimeout.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
            ramCounter = new PerformanceCounter("Memory", "Available MBytes", true);
        }

        private void textBoxOnlyNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Hanya izinkan input angka dan kontrol khusus (seperti Backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBoxInputRequest_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxInputRequest.Text) && !int.TryParse(textBoxInputRequest.Text, out _))
            {
                MessageBox.Show("Please enter a valid number for requests.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxInputRequest.Text = string.Empty;
            }

            if (textBoxInputRequest.Text.Length > 8)
            {
                MessageBox.Show("Input request should be limited to 8 digits.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxInputRequest.Text = textBoxInputRequest.Text.Substring(0, 8);
                textBoxInputRequest.SelectionStart = textBoxInputRequest.Text.Length;
            }
        }

        private void textBoxInputTimeout_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxInputTimeout.Text) && !int.TryParse(textBoxInputTimeout.Text, out _))
            {
                MessageBox.Show("Please enter a valid number for timeout.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxInputTimeout.Text = string.Empty;
            }

            if (textBoxInputTimeout.Text.Length > 5)
            {
                MessageBox.Show("Input timeout should be limited to 5 digits.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxInputTimeout.Text = textBoxInputTimeout.Text.Substring(0, 5);
                textBoxInputTimeout.SelectionStart = textBoxInputTimeout.Text.Length;
            }
        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            // Pastikan counters diinisialisasi sebelum digunakan
            if (cpuCounter == null || ramCounter == null)
            {
                MessageBox.Show("Failed to initialize counters. Ensure that the system supports the counters being used.");
                return;
            }

            string url = textBoxInputUrl.Text;

            // Validasi input URL tidak kosong
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Please enter a valid URL.");
                return;
            }

            // Validasi input Request tidak kosong
            if (string.IsNullOrWhiteSpace(textBoxInputRequest.Text))
            {
                MessageBox.Show("Please enter a valid number of requests.");
                return;
            }

            // Validasi input Timeout tidak kosong
            if (string.IsNullOrWhiteSpace(textBoxInputTimeout.Text))
            {
                MessageBox.Show("Please enter a valid timeout value.");
                return;
            }

            int inputRequest = Convert.ToInt32(textBoxInputRequest.Text);
            int inputTimeout = Convert.ToInt32(textBoxInputTimeout.Text);

            // Validasi jumlah request
            if (!int.TryParse(inputRequest.ToString(), out int numberOfRequests) || numberOfRequests <= 0)
            {
                MessageBox.Show("Please enter a valid number of requests.");
                return;
            }

            // Validasi input Timeout
            if (!int.TryParse(inputTimeout.ToString(), out int timeoutInSeconds) || timeoutInSeconds <= 0)
            {
                MessageBox.Show("Please enter a valid timeout value.");
                return;
            }

            // Mulai pemantauan penggunaan sumber daya
            StartResourceMonitoring();

            await RunLoadTest(url, numberOfRequests, timeoutInSeconds);

            // Berhenti pemantauan penggunaan sumber daya setelah selesai
            StopResourceMonitoring();
        }

        private async Task RunLoadTest(string url, int numberOfRequests, int timeoutInSeconds)
        {
            HttpClient httpClient = new HttpClient();

            // Set timeout pada objek HttpClient
            httpClient.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);

            var tasks = new List<Task<List<LoadTestResult>>>(numberOfRequests);

            for (int i = 0; i < numberOfRequests; i++)
            {
                tasks.Add(SendHttpRequest(httpClient, url, i + 1, timeoutInSeconds));
            }

            // Tunggu hingga semua tugas selesai
            await Task.WhenAll(tasks);

            // Ambil hasil dari tugas yang selesai
            foreach (var task in tasks)
            {
                currentTestResults.AddRange(task.Result); // Tambahkan hasil ke dalam koleksi
            }
        }

        private async Task<List<LoadTestResult>> SendHttpRequest(HttpClient httpClient, string url, int requestNumber, int timeoutInSeconds)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                // Gunakan CancellationTokenSource untuk menerapkan timeout
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutInSeconds)))
                {
                    // Kombinasikan CancellationToken dengan GetAsync
                    HttpResponseMessage response = await httpClient.GetAsync(url, cts.Token);
                    TimeSpan responseTime = stopwatch.Elapsed;

                    // Perbarui properti RoundNumber
                    int roundNumber = currentRoundNumber; // Simpan nilai roundNumber saat ini
                    return new List<LoadTestResult>
                    {
                        new LoadTestResult
                        {
                            RequestNumber = requestNumber,
                            StatusCode = response.StatusCode,
                            ReasonPhrase = response.ReasonPhrase,
                            ResponseTime = responseTime,
                            RoundNumber = roundNumber // Gunakan nilai roundNumber yang disimpan
                        }
                    };
                }
            }
            catch (TaskCanceledException)
            {
                // Tangani timeout
                outputTextBox.AppendText($"Request {requestNumber} (Round {currentRoundNumber}): Timeout - Request timed out after {timeoutInSeconds} seconds\n");

                // Perbarui properti RoundNumber
                int roundNumber = currentRoundNumber; // Simpan nilai roundNumber saat ini
                return new List<LoadTestResult>
        {
            new LoadTestResult
            {
                RequestNumber = requestNumber,
                StatusCode = System.Net.HttpStatusCode.RequestTimeout,
                ReasonPhrase = "Timeout",
                ResponseTime = stopwatch.Elapsed,
                RoundNumber = roundNumber // Gunakan nilai roundNumber yang disimpan
            }
        };
            }
            catch (Exception ex)
            {
                // Tangani pengecualian lainnya
                outputTextBox.AppendText($"Request {requestNumber} (Round {currentRoundNumber}): Error - {ex.Message}\n");

                return new List<LoadTestResult>
                {
                    new LoadTestResult
                    {
                        RequestNumber = requestNumber,
                        StatusCode = System.Net.HttpStatusCode.InternalServerError,
                        ReasonPhrase = "Error",
                        ResponseTime = stopwatch.Elapsed,
                        RoundNumber = currentRoundNumber // Tambahkan roundNumber ke dalam hasil
                    }
                };
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        private void StartResourceMonitoring()
        {
            // Mulai pemantauan CPU dan RAM
            cpuCounter.NextValue();
            ramCounter.NextValue();
        }

        private void StopResourceMonitoring()
        {
            // Mendapatkan penggunaan sumber daya terakhir setelah uji beban selesai
            float cpuUsage = cpuCounter.NextValue();
            float ramUsage = ramCounter.NextValue();

            // Menampilkan penggunaan sumber daya di outputTextBox
            outputTextBox.AppendText($"Computer's CPU Usage: {cpuUsage}%\n");
            outputTextBox.AppendText(Environment.NewLine);
            outputTextBox.AppendText($"Computer's RAM Usage: {ramUsage} MB\n");
            outputTextBox.AppendText(Environment.NewLine);

            // Menampilkan hasil request di outputTextBox
            foreach (var result in currentTestResults.OrderBy(result => result.RequestNumber))
            {
                string resultString = $"Request {result.RequestNumber}: {result.StatusCode} - {result.ReasonPhrase}, " +
                    $"Response Time: {result.ResponseTime.TotalMilliseconds} ms";

                outputTextBox.AppendText(resultString + Environment.NewLine);
            }

            // Tambahkan hasil uji beban saat ini ke koleksi semua hasil
            allLoadTestResults.AddRange(currentTestResults);

            // Evaluasi hasil load testing hanya jika ada hasil uji beban
            if (currentTestResults.Any())
            {
                // Increment currentRoundNumber setelah setiap ronde
                currentRoundNumber++;

                // Tambahkan nilai currentRoundNumber ke dalam roundNumbers
                roundNumbers.Add(currentRoundNumber);

                // Jalankan evaluasi
                EvaluateLoadTestResults();
            }

            // Bersihkan koleksi untuk uji selanjutnya
            currentTestResults.Clear();
        }

        private void EvaluateLoadTestResults()
        {
            if (currentTestResults.Any())
            {
                // Hitung metrik evaluasi
                double averageResponseTime = currentTestResults.Average(result => result.ResponseTime.TotalMilliseconds);
                int totalRequests = currentTestResults.Count;
                int successfulRequests = currentTestResults.Count(result => IsSuccessful(result));
                int failedRequests = totalRequests - successfulRequests;

                // Perbarui properti RoundNumber pada objek EvaluationData
                int roundNumber = currentRoundNumber; // Simpan nilai roundNumber saat ini
                currentEvaluationData = new EvaluationData
                {
                    RoundNumber = roundNumber, // Gunakan nilai roundNumber yang disimpan
                    AverageResponseTime = averageResponseTime,
                    TotalRequests = totalRequests,
                    SuccessfulRequests = successfulRequests,
                    FailedRequests = failedRequests
                };

                // Tambahkan hasil evaluasi ke koleksi semua hasil
                allEvaluatedData.Add(currentEvaluationData);

                // Menampilkan evaluasi di outputTextBox
                outputTextBox.AppendText($"Round {currentEvaluationRoundNumber} - Average Response Time: {averageResponseTime} ms\n");
                outputTextBox.AppendText(Environment.NewLine);
                outputTextBox.AppendText($"Total Requests: {totalRequests}\n");
                outputTextBox.AppendText(Environment.NewLine);
                outputTextBox.AppendText($"Successful Requests: {successfulRequests}\n");
                outputTextBox.AppendText(Environment.NewLine);
                outputTextBox.AppendText($"Failed Requests: {failedRequests}\n");
                outputTextBox.AppendText(Environment.NewLine);
                outputTextBox.AppendText(Environment.NewLine);

                // Increment currentEvaluationRoundNumber setelah setiap evaluasi
                currentEvaluationRoundNumber++;
            }
        }

        private bool IsSuccessful(LoadTestResult result)
        {
            // Logika untuk menentukan apakah permintaan berhasil atau gagal
            // Permintaan dengan status code 200 OK dianggap berhasil
            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            ShowHelpMessageBox();
        }

        private void ShowHelpMessageBox()
        {
            string helpMessage = GenerateHelpMessage();
            MessageBox.Show(helpMessage, "User Guide", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GenerateHelpMessage()
        {
            StringBuilder helpMessage = new StringBuilder();

            helpMessage.AppendLine("User Guide for Load Testing:");
            helpMessage.AppendLine();
            helpMessage.AppendLine("1. Enter the target URL in the URL column.");
            helpMessage.AppendLine("2. Enter the number of requests in the Number of Request column (maximum 8 digits).");
            helpMessage.AppendLine("3. Enter the timeout value (maximum time allowed for each request) in the Timeout column (maximum 5 digits).");
            helpMessage.AppendLine("4. Click the 'Start' button to initiate the load testing.");
            helpMessage.AppendLine("5. Monitor real-time CPU and RAM of the computer usage during the test in the output area.");
            helpMessage.AppendLine("6. After the test, the output will display:");
            helpMessage.AppendLine("   a. Computer's CPU Usage (in percentage).");
            helpMessage.AppendLine("   b. Computer's RAM Usage (in megabytes).");
            helpMessage.AppendLine("   c. Detailed results for each request, including:");
            helpMessage.AppendLine("      - Request Number.");
            helpMessage.AppendLine("      - HTTP Status Code.");
            helpMessage.AppendLine("      - Reason Phrase.");
            helpMessage.AppendLine("      - Response Time (in milliseconds).");
            helpMessage.AppendLine("      - Round Number.");
            helpMessage.AppendLine("7. Optionally, export the load testing results to a CSV file using the 'Export' button. The result will display all the evaluation data for each round.");
            helpMessage.AppendLine("8. Click the 'Clear' button to clear the output area and reset the test.");
            helpMessage.AppendLine();
            helpMessage.AppendLine("Note: Ensure that your internet connection is stable and reliable for conducting this test.");

            return helpMessage.ToString();
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            ShowInfoMessageBox();
        }

        private void ShowInfoMessageBox()
        {
            string infoMessage = GenerateInfoMessage();
            MessageBox.Show(infoMessage, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GenerateInfoMessage()
        {
            StringBuilder infoMessage = new StringBuilder();

            infoMessage.AppendLine("Load Testing:");
            infoMessage.AppendLine();
            infoMessage.AppendLine("\"Load testing is a performance testing technique in which the system's response is measured under various load conditions. This testing helps determine how the software behaves when multiple users access it simultaneously. Load testing is essential to simulate concurrent access to a website application.\"[1]");
            infoMessage.AppendLine("[1] D. I. Permatasari, B. Santoso, N. Ningtias, M. H. Y. R., R. Atika, N. Widad, I. Maulana and A. A. R., \"Pengukuran Throughput Load Testing Menggunakan Test Case Sampling Gorilla Testing,\" in Seminar Nasional Sistem Informasi (SENASIF), Malang, 2019.");
            infoMessage.AppendLine();
            infoMessage.AppendLine("Metrics:");
            infoMessage.AppendLine();
            infoMessage.AppendLine("1. CPU Usage:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        The CPU usage metric represents the percentage of the computer's processing power utilized during the load testing.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        CPU Usage = Current CPU Usage");
            infoMessage.AppendLine("2. RAM Usage:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        The RAM usage metric indicates the amount of computer memory used during the load testing.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        RAM Usage = Current RAM Usage");
            infoMessage.AppendLine("3. Total Requests:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Total Requests is the sum of all requests sent during a single testing round.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Total Requests = Number of Requests Sent");
            infoMessage.AppendLine("4. Successful Requests:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Successful Requests is the sum of requests with an HTTP status code of 200 (OK).");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Successful Requests = Number of Requests with HTTP Status Code 200 (OK)");
            infoMessage.AppendLine("5. Failed Requests:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Failed Requests is the sum of requests that failed, calculated as Total Requests minus Successful Requests.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Failed Requests = Total Requests−Successful Requests");
            infoMessage.AppendLine("6. Average Response Time:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Average Response Time represents the mean response time per request during a testing round.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Average Response Time = Total Response Time/Total Requests");

            return infoMessage.ToString();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // Export hasil uji beban saat ini ke file CSV
            if (roundNumbers.Any()) // Periksa apakah roundNumbers tidak kosong
            {
                ShowExportDialog(allLoadTestResults, allEvaluatedData);
            }
            else
            {
                MessageBox.Show("There is no load testing data that can be exported.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowExportDialog(List<LoadTestResult> results, List<EvaluationData> allEvaluatedData)
        {
            // Tampilkan dialog SaveFileDialog
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Ekspor data ke file CSV
                    ExportToCsv(results, allEvaluatedData, saveFileDialog.FileName);

                    // Jalankan evaluasi setelah mengekspor data
                    EvaluateLoadTestResults();

                    // Munculkan pesan bahwa data telah diekspor
                    MessageBox.Show($"The data has been successfully exported to: {saveFileDialog.FileName}", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ExportToCsv(List<LoadTestResult> results, List<EvaluationData> allEvaluatedData, string filePath)
        {
            if (results == null || results.Count == 0)
            {
                MessageBox.Show("There is no load testing result data that can be exported.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Create the directory if it doesn't exist
                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                StringBuilder csvContent = new StringBuilder();

                csvContent.AppendLine($"Load Testing");
                csvContent.AppendLine();

                csvContent.AppendLine($"The Load Testing Result of URL:,{textBoxInputUrl.Text}");
                csvContent.AppendLine();

                // Tambahkan header CSV sesuai dengan urutan kolom yang diinginkan
                csvContent.AppendLine("Request Number,Http Status Code,Reason Phrase,Response Time (ms),Round Number");

                // Menggunakan parameter results yang berisi hasil uji beban saat ini
                foreach (var result in results)
                {
                    // Mengonversi ResponseTime ke dalam format numerik (mengambil angka di depan koma)
                    string responseTime = result.ResponseTime.TotalMilliseconds.ToString("F0");

                    // Sertakan RoundNumber
                    string csvLine = $"{result.RequestNumber},{(int)result.StatusCode},{result.ReasonPhrase},{responseTime},{result.RoundNumber}";
                    csvContent.AppendLine(csvLine);
                }

                csvContent.AppendLine();

                // Tambahkan semua data evaluasi sebelumnya
                foreach (var evalData in allEvaluatedData)
                {
                    // Gunakan evalData.RoundNumber untuk mendapatkan nomor putaran yang benar
                    csvContent.AppendLine($"Summary of Round {evalData.RoundNumber - 1}:");
                    csvContent.AppendLine($"Average Response Time (ms):,{evalData.AverageResponseTime.ToString("F0")}");
                    csvContent.AppendLine($"Total Requests:,{evalData.TotalRequests}");
                    csvContent.AppendLine($"Successful Requests:,{evalData.SuccessfulRequests}");
                    csvContent.AppendLine($"Failed Requests:,{evalData.FailedRequests}");
                    csvContent.AppendLine();
                }

                System.IO.File.WriteAllText(filePath, csvContent.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed.: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBoxInputUrl.Clear();
            textBoxInputRequest.Clear();
            textBoxInputTimeout.Clear();
            outputTextBox.Clear();
            allEvaluatedData.Clear();
            allLoadTestResults.Clear();
            roundNumbers.Clear();
            currentRoundNumber = 1;
            currentEvaluationRoundNumber = 1;
        }
    }

    public class LoadTestResult
    {
        public int RequestNumber { get; set; }
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public TimeSpan ResponseTime { get; set; }
        public int RoundNumber { get; set; }
    }

    public class EvaluationData
    {
        public int RoundNumber { get; set; }
        public double AverageResponseTime { get; set; }
        public int TotalRequests { get; set; }
        public int SuccessfulRequests { get; set; }
        public int FailedRequests { get; set; }
    }
}