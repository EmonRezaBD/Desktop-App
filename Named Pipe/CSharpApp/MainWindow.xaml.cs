using System.IO;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq; // Add this for LINQ
using System.Text.Json;
using System.Diagnostics;
using System.Windows;

namespace CSharpApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SendData();
        }

        private void SendData()
        {
            // Create a person object to send
            var person = new Person
            {
                Name = "FSMB",
                TeamName = "Software",
                Members = 34,
                //Scores = new List<double> { 1.1, 2.2, 3.3, 4.4, 5.5 }
                Scores = Enumerable.Range(1, 200).Select(x => (double)x).ToList()
            };

            // Serialize the person object to JSON
            string jsonData = JsonSerializer.Serialize(person);
            Console.WriteLine($"Sending JSON Data: {jsonData}"); // Log JSON before sending
                                                                 // Create a stopwatch instance
            Stopwatch stopwatch = new Stopwatch();     // Create a stopwatch instance

            try
            {
                // Create named pipe server
                using (var server = new NamedPipeServerStream("myNamedPipe1"))
                {
                    Console.WriteLine("Waiting for client connection...");
                    server.WaitForConnection();
                    Console.WriteLine("Client connected.");

                    using (var writer = new StreamWriter(server))
                    {
                        writer.AutoFlush = true; // Ensure data is sent immediately

                        // Start measuring time
                        stopwatch.Start();

                        for (int i = 0; i < 100; i++)
                        {
                            writer.WriteLine(jsonData); // Send JSON data
                            Console.WriteLine($"Sent data {i + 1}: {jsonData}");
                        }

                        // Stop measuring time
                        stopwatch.Stop();
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IOException occurred: {ex.Message}");
            }

            // Output the elapsed time in milliseconds
            Debug.WriteLine($"Time taken to send data: {stopwatch.ElapsedMilliseconds} ms");
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public string TeamName { get; set; }
        public int Members { get; set; }
        public List<double> Scores { get; set; }
    }
}
