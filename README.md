# Load Testing Application

## Overview

The Load Testing Application is a tool designed for evaluating the performance of a web server by simulating multiple user requests. This application provides real-time monitoring of CPU and RAM of the computer usage during the load test and offers insightful metrics such as response time, success rate, and failure rate.

## Features

- **Easy Configuration:**
  - Enter the target URL, set the number of requests, and specify the timeout value effortlessly through a user-friendly interface.

- **Real-time Resource Monitoring:**
  - Monitor the CPU and RAM of the computer usage in real-time during the load test to identify potential resource bottlenecks.

- **Detailed Results:**
  - View detailed results for each request, including HTTP status code, reason phrase, response time, and round number.

- **Automatic Evaluation:**
  - The application automatically evaluates the load test results, providing average response time, total requests, successful requests, and failed requests after each round.

- **Exportable Results:**
  - Export the load test results to a CSV file for further analysis and reporting.

## Getting Started

### Prerequisites

- [.NET Framework](https://dotnet.microsoft.com/download/dotnet-framework) installed on your machine.

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/rahmabintangpratama/LoadTestingApp.git
2. Open the project in Visual Studio or your preferred C# development environment.
3. Build and run the application.

## Usage
1. Enter Target URL:
   - In the application, enter the target URL in the designated text box.

2. Enter Number of Requests:
   - Enter the number of request in the designated text box to set the desired number of requests.

3. Enter Timeout Value:
   - Enter the timeout value in the designated text box to set the maximum time allowed for each request.

4. Start Load Test:
   - Click the 'Start' button to initiate the load test.

5. Monitor Resource Usage:
   - Real-time CPU and RAM of the computer usage during the test will be displayed in the output area.

6. View Results:
   - After the test, the application will display:
   - Computer's CPU Usage (in percentage).
   - Computer's RAM Usage (in megabytes).
   - Detailed results for each request, including:
     - Request Number.
     - HTTP Status Code.
     - Reason Phrase.
     - Response Time (in milliseconds).
     - Round Number.

7. Export Results:
   - Optionally, export the load test results to a CSV file using the 'Export' button.

8. Clear Results:
   - Click the 'Clear' button to clear the output area and reset the test.
