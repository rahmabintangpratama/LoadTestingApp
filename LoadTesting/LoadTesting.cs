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
        private int currentRoundNumber = 1; // Default round number is 1
        private List<int> roundNumbers = new List<int>();
        private EvaluationData currentEvaluationData; // Properti untuk menyimpan data evaluasi saat ini
        private List<EvaluationData> allEvaluatedData = new List<EvaluationData>(); // Koleksi semua data yang telah dievaluasi
        private int currentEvaluationRoundNumber = 1; // Default evaluation round number is 1

        public LoadTesting()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Inisialisasi counters
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
            ramCounter = new PerformanceCounter("Memory", "Available MBytes", true);
        }

        private void numericUpAndDown(object sender, EventArgs e)
        {

        }

        private void output(object sender, EventArgs e)
        {

        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            // Pastikan counters diinisialisasi sebelum digunakan
            if (cpuCounter == null || ramCounter == null)
            {
                MessageBox.Show("Failed to initialize counters. Ensure that the system supports the counters being used.");
                return;
            }

            string url = inputUrl.Text;

            // Validasi jumlah request
            if (!int.TryParse(numericUpDown.Value.ToString(), out int numberOfRequests) || numberOfRequests <= 0)
            {
                MessageBox.Show("Please enter a valid number of requests.");
                return;
            }

            // Validasi input URL
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Please enter a valid URL.");
                return;
            }

            // Validasi input Timeout
            if (!int.TryParse(timeoutNumericUpDown.Value.ToString(), out int timeoutInSeconds) || timeoutInSeconds <= 0)
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
            helpMessage.AppendLine("2. Set the number of requests using the numeric up-down control.");
            helpMessage.AppendLine("3. Set the timeout value (maximum time allowed for each request) using the numeric up-down control.");
            helpMessage.AppendLine("4. Click the 'Start' button to initiate the load test.");
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
            helpMessage.AppendLine("7. Optionally, export the load test results to a CSV file using the 'Export' button. The result will display all the evaluation data for each round.");

            return helpMessage.ToString();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // Export hasil uji beban saat ini ke file CSV
            if (roundNumbers.Any()) // Periksa apakah roundNumbers tidak kosong sebelum menggunakan Last()
            {
                ShowExportDialog(allLoadTestResults, roundNumbers.Last(), currentEvaluationData, allEvaluatedData);
            }
            else
            {
                MessageBox.Show("There is no load test data that can be exported.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowExportDialog(List<LoadTestResult> results, int roundNumber, EvaluationData currentEvaluationData, List<EvaluationData> allEvaluatedData)
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
                    ExportToCsv(results, roundNumber, currentEvaluationData, allEvaluatedData, saveFileDialog.FileName);

                    // Jalankan evaluasi setelah mengekspor data
                    EvaluateLoadTestResults();

                    // Munculkan pesan bahwa data telah diekspor
                    MessageBox.Show($"The data has been successfully exported to: {saveFileDialog.FileName}", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ExportToCsv(List<LoadTestResult> results, int roundNumber, EvaluationData currentEvaluationData, List<EvaluationData> allEvaluatedData, string filePath)
        {
            if (results == null || results.Count == 0)
            {
                MessageBox.Show("There is no load test result data that can be exported.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    csvContent.AppendLine($"Round {evalData.RoundNumber - 1} of Evaluation Data:");
                    csvContent.AppendLine($"Average Response Time: {evalData.AverageResponseTime} ms");
                    csvContent.AppendLine($"Total Requests: {evalData.TotalRequests}");
                    csvContent.AppendLine($"Successful Requests: {evalData.SuccessfulRequests}");
                    csvContent.AppendLine($"Failed Requests: {evalData.FailedRequests}");
                    csvContent.AppendLine(); // Baris kosong sebagai pemisah
                }

                csvContent.AppendLine($"The Load Testing Result of URL: {inputUrl.Text}");

                System.IO.File.WriteAllText(filePath, csvContent.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed.: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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