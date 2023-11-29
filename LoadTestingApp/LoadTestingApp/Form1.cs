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

namespace LoadTestingApp
{
    public partial class LoadTestingApp : Form
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;
        private List<LoadTestResult> loadTestResults = new List<LoadTestResult>();

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

        private async void numericUpAndDown(object sender, EventArgs e)
        {

        }

        private void output(object sender, EventArgs e)
        {

        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            // Membersihkan outputTextBox sebelum menguji URL baru
            outputTextBox.Clear();

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

            // Baca nilai timeout dari numericUpDown
            int timeoutInSeconds = (int)timeoutNumericUpDown.Value;

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

            loadTestResults.Clear(); // Membersihkan koleksi sebelum setiap uji beban

            var tasks = new List<Task<LoadTestResult>>(numberOfRequests);

            for (int i = 0; i < numberOfRequests; i++)
            {
                tasks.Add(SendHttpRequest(httpClient, url, i + 1));
            }

            for (int i = 0; i < numberOfRequests; i++)
            {
                LoadTestResult result = await tasks[i];
                loadTestResults.Add(result); // Menambahkan hasil request ke dalam koleksi
            }
        }

        private async Task<LoadTestResult> SendHttpRequest(HttpClient httpClient, string url, int requestNumber)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                TimeSpan responseTime = stopwatch.Elapsed;

                // Handle the response if needed
                // Example: string content = await response.Content.ReadAsStringAsync();

                return new LoadTestResult
                {
                    RequestNumber = requestNumber,
                    StatusCode = response.StatusCode,
                    ReasonPhrase = response.ReasonPhrase,
                    ResponseTime = responseTime
                };
            }
            catch (Exception ex)
            {
                // Handle exceptions
                outputTextBox.AppendText($"Request {requestNumber}: Error - {ex.Message}\n");

                return new LoadTestResult
                {
                    RequestNumber = requestNumber,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    ReasonPhrase = "Error",
                    ResponseTime = stopwatch.Elapsed
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

            // Urutkan hasil berdasarkan nomor permintaan
            loadTestResults = loadTestResults.OrderBy(result => result.RequestNumber).ToList();

            // Menampilkan hasil request di outputTextBox
            foreach (var result in loadTestResults)
            {
                string resultString = $"Request {result.RequestNumber}: {result.StatusCode} - {result.ReasonPhrase}, " +
                                      $"Response Time: {result.ResponseTime.TotalMilliseconds} ms";

                outputTextBox.AppendText(resultString + Environment.NewLine); // Menambahkan baris baru setelah setiap hasil
            }

            // Evaluasi hasil load testing
            EvaluateLoadTestResults();
        }

        private void EvaluateLoadTestResults()
        {
            // Hitung metrik evaluasi (contoh: rata-rata waktu respons)
            double averageResponseTime = loadTestResults.Average(result => result.ResponseTime.TotalMilliseconds);

            // Logika evaluasi sederhana: Jika rata-rata waktu respons kurang dari 500 ms, dianggap baik; sebaliknya, dianggap buruk
            string conclusion = (averageResponseTime < 1000) ? "Load testing result: Good" : "Load testing result: Bad";

            // Menampilkan kesimpulan di outputTextBox
            outputTextBox.AppendText($"{conclusion}\n");
        }

        private void inputUrl_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }

    public class LoadTestResult
    {
        public int RequestNumber { get; set; }
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public TimeSpan ResponseTime { get; set; }
    }
}
