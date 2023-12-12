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

namespace LoadTestingApp
{
    public partial class LoadTestingApp : Form
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;
        private List<LoadTestResult> allLoadTestResults = new List<LoadTestResult>();
        private List<LoadTestResult> currentTestResults = new List<LoadTestResult>();
        private int currentRoundNumber = 1; // Default round number is 1
        private List<int> roundNumbers = new List<int>();

        public LoadTestingApp()
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
                MessageBox.Show("Gagal menginisialisasi counters. Pastikan sistem mendukung counters yang digunakan.");
                return;
            }

            string url = inputUrl.Text;

            // Validasi jumlah request
            if (!int.TryParse(numericUpDown.Value.ToString(), out int numberOfRequests) || numberOfRequests <= 0)
            {
                MessageBox.Show("Masukkan jumlah request yang valid.");
                return;
            }

            // Validasi input URL
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Masukkan URL yang valid.");
                return;
            }

            // Validasi input Timeout
            if (!int.TryParse(timeoutNumericUpDown.Value.ToString(), out int timeoutInSeconds) || timeoutInSeconds <= 0)
            {
                MessageBox.Show("Masukkan nilai timeout yang valid.");
                return;
            }

            // Mulai pemantauan penggunaan sumber daya
            StartResourceMonitoring();

            await RunLoadTest(url, numberOfRequests, timeoutInSeconds);

            // Berhenti pemantauan penggunaan sumber daya setelah selesai
            StopResourceMonitoring();

            // Increment currentRoundNumber setelah setiap ronde
            currentRoundNumber++;

            // Tambahkan nilai currentRoundNumber ke dalam roundNumbers
            roundNumbers.Add(currentRoundNumber);
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

                    // Handle the response if needed
                    // Example: string content = await response.Content.ReadAsStringAsync();

                    return new List<LoadTestResult>
            {
                new LoadTestResult
                {
                    RequestNumber = requestNumber,
                    StatusCode = response.StatusCode,
                    ReasonPhrase = response.ReasonPhrase,
                    ResponseTime = responseTime,
                    RoundNumber = currentRoundNumber // Tambahkan roundNumber ke dalam hasil
                }
            };
                }
            }
            catch (TaskCanceledException)
            {
                // Tangani timeout
                outputTextBox.AppendText($"Request {requestNumber} (Round {currentRoundNumber}): Timeout - Request timed out after {timeoutInSeconds} seconds\n");

                return new List<LoadTestResult>
        {
            new LoadTestResult
            {
                RequestNumber = requestNumber,
                StatusCode = System.Net.HttpStatusCode.RequestTimeout,
                ReasonPhrase = "Timeout",
                ResponseTime = stopwatch.Elapsed,
                RoundNumber = currentRoundNumber // Tambahkan roundNumber ke dalam hasil
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
            outputTextBox.AppendText($"CPU Usage: {cpuUsage}%\n");
            outputTextBox.AppendText(Environment.NewLine);
            outputTextBox.AppendText($"RAM Usage: {ramUsage} MB\n");
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

            // Evaluasi hasil load testing
            EvaluateLoadTestResults();

            // Bersihkan koleksi untuk uji selanjutnya
            currentTestResults.Clear();
        }

        private void EvaluateLoadTestResults()
        {
            if (currentTestResults.Any()) // Tambahkan pemeriksaan apakah koleksi tidak kosong
            {
                // Hitung metrik evaluasi (contoh: rata-rata waktu respons)
                double averageResponseTime = currentTestResults
                    .Average(result => result.ResponseTime.TotalMilliseconds);

                // Logika evaluasi sederhana: Jika rata-rata waktu respons kurang dari 1000 ms, dianggap baik; sebaliknya, dianggap buruk
                string conclusion = (averageResponseTime < 1000) ? "Load testing result: Good" : "Load testing result: Bad";

                // Menampilkan kesimpulan di outputTextBox
                outputTextBox.AppendText($"{conclusion}\n");
                outputTextBox.AppendText(Environment.NewLine);
                outputTextBox.AppendText(Environment.NewLine);
            }
            else
            {
                // Koleksi kosong, berikan pesan bahwa tidak ada data untuk dievaluasi
                outputTextBox.AppendText("No data to evaluate.\n");
            }
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

            // Tambahkan panduan dalam bahasa Inggris
            helpMessage.AppendLine("User Guide for Load Testing Application");
            helpMessage.AppendLine();
            helpMessage.AppendLine("1. Enter the URL in the input box.");
            helpMessage.AppendLine("2. Set the number of requests using the numeric up-down control.");
            helpMessage.AppendLine("3. Set the timeout value using the numeric up-down control.");
            helpMessage.AppendLine("4. Click the 'Start' button to run the load test.");
            helpMessage.AppendLine("5. The application will display the CPU and RAM usage during the test.");
            helpMessage.AppendLine("6. After the test, the results, including response times, will be shown.");
            helpMessage.AppendLine("7. The application will provide a simple evaluation of the load testing result.");

            return helpMessage.ToString();
        }

        private void btnEksport_Click(object sender, EventArgs e)
        {
            // Export hasil uji beban saat ini ke file CSV
            if (roundNumbers.Any()) // Periksa apakah roundNumbers tidak kosong sebelum menggunakan Last()
            {
                ExportToCsv(allLoadTestResults, roundNumbers.Last());
            }
            else
            {
                MessageBox.Show("Tidak ada data hasil uji beban yang dapat diekspor.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ExportToCsv(List<LoadTestResult> results, int roundNumber)
        {
            if (results == null || results.Count == 0)
            {
                MessageBox.Show("Tidak ada data hasil uji beban yang dapat diekspor.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Menggunakan direktori Documents untuk penyimpanan
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string filePath = Path.Combine(documentsPath, $"LoadTestResults_Round{roundNumber - 1}.csv"); // Gunakan roundNumber dalam nama file

                StringBuilder csvContent = new StringBuilder();
                // Tambahkan header CSV sesuai dengan urutan kolom yang diinginkan
                csvContent.AppendLine("RequestNumber,StatusCode,ReasonPhrase,ResponseTime(ms),RoundNumber");

                // Menggunakan parameter results yang berisi hasil uji beban saat ini
                foreach (var result in results)
                {
                    // Mengonversi ResponseTime ke dalam format numerik (mengambil angka di depan koma)
                    string responseTime = result.ResponseTime.TotalMilliseconds.ToString("F0");

                    // Sertakan RoundNumber
                    string csvLine = $"{result.RequestNumber},{(int)result.StatusCode},{result.ReasonPhrase},{responseTime},{result.RoundNumber}";
                    csvContent.AppendLine(csvLine);
                }

                System.IO.File.WriteAllText(filePath, csvContent.ToString());

                MessageBox.Show($"Data berhasil diekspor ke: {filePath}", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal melakukan ekspor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
}