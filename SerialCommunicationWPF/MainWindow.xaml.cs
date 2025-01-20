using System;
using System.IO;
using System.IO.Pipes;
using System.IO.Ports;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace SerialCommunicationWPF
{
    public partial class MainWindow : Window
    {
        //Global variables
        private SerialPort portObj; //Port object
        private NamedPipeClientStream pipeClient; //Pipe object
        string receivedCommand; //Command from CPP
        string temperatureOmron; //Result from HW

        //Testing data
        /*struct Person
        {
            public string Name { get; set; }
            public string TeamName { get; set; }
            public int Members { get; set; }
        }*/
        public MainWindow()
        {
            InitializeComponent();
            LoadAvailablePorts();
        }

        //Eastablish COM with HW
        private void LoadAvailablePorts()
        {
            string[] ports = SerialPort.GetPortNames();
            ComboBoxPorts.ItemsSource = ports;
        }

        //Connecting to a port
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (portObj == null)
            {
                string selectedPort = ComboBoxPorts.SelectedItem.ToString();

                int baudRate = 9600;
                Parity parity = Parity.Even; // Equivalent to 'E' in your C++ code
                int dataBits = 7; // Data bits
                StopBits stopBits = StopBits.Two; // Stop bits

                try
                {
                    // Initialize and configure the serial port
                    portObj = new SerialPort(selectedPort, baudRate, parity, dataBits, stopBits);
                    // portObj.Handshake = Handshake.None; // Set handshake if needed

                    // Set read and write timeouts (optional)
                    portObj.ReadTimeout = 500; // Set a timeout for reading
                    portObj.WriteTimeout = 500; // Set a timeout for writing

                    // Open the serial port
                    portObj.Open();

                    StatusLabel.Content = "Connected to " + selectedPort;
                    ConnectButton.Content = "Disconnect";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error connecting to port: " + ex.Message);
                }
            }
            else
            {
                portObj.Close();
                StatusLabel.Content = "Disconnected";
                ConnectButton.Content = "Connect";
                portObj = null;
            }

            //Receive command from C++
            receiveCommandFromCPP();//step-01: Open pipe, step-02: Read command from C++, step-03: store the command 
        }
       
        //Receive command from C++
        private void receiveCommandFromCPP()
        {
            //Open pip in both direction
            openPipeInOut();

            //Reading data from C++
            byte[] buffer = new byte[4096];
            int bytesRead = pipeClient.Read(buffer, 0, buffer.Length);
            receivedCommand = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            //Other codes

            /*string receivedData = reader.ReadToEnd();
            Console.WriteLine("Received data: " + receivedData);

            JsonDocument jsonData = JsonDocument.Parse(receivedData);

            if (jsonData.RootElement.TryGetProperty("GetTemperature", out JsonElement getTemperatureElement))
            {
                string getTemperature = getTemperatureElement.ToString();
                Console.WriteLine("GetTemperature command received: " + getTemperature);
            }
            else
            {
                Console.WriteLine("No valid data found for GetTemperature.");
            }*/

            // }
        }
        private void openPipeInOut()
        {
            pipeClient = new NamedPipeClientStream(".", "myNamedPipe1", PipeDirection.InOut);
            pipeClient.Connect();
            Console.WriteLine("Connected to pipe.");
        }

        //Form command and Send it to HW
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (portObj != null && portObj.IsOpen)
            {
                //Reza add
                byte[] fcmd = new byte[25];
                // Initialize command array
                fcmd[0] = 0x02; // STX
                fcmd[1] = (byte)'0'; // Node Number
                fcmd[2] = (byte)'1'; // Node Number
                fcmd[3] = (byte)'0'; // Sub Address
                fcmd[4] = (byte)'0'; // Sub Address
                fcmd[5] = (byte)'0'; // SID
                fcmd[6] = (byte)'0'; // MRC
                fcmd[7] = (byte)'1'; // MRC
                fcmd[8] = (byte)'0'; // SRC
                fcmd[9] = (byte)'1'; // SRC
                fcmd[10] = (byte)'C'; // Variable Type
                fcmd[11] = (byte)'0'; // Variable Type
                fcmd[12] = (byte)'0'; // Address
                fcmd[13] = (byte)'0'; // Address
                fcmd[14] = (byte)'0'; // Address
                fcmd[15] = (byte)'0'; // Address
                fcmd[16] = (byte)'0'; // Bit Position
                fcmd[17] = (byte)'0'; // Bit Position
                fcmd[18] = (byte)'0'; // Number Of Elements
                fcmd[19] = (byte)'0'; // Number Of Elements
                fcmd[20] = (byte)'0'; // Number Of Elements
                fcmd[21] = (byte)'1'; // Number Of Elements
                fcmd[22] = 0x03; // ETX

                // Calculate BCC
                uint BCC = 0;
                for (int i = 1; i < 23; i++)
                {
                    BCC ^= fcmd[i];
                }

                fcmd[23] = (byte)BCC; // BCC
                fcmd[24] = 0;         // Null terminator

                if(receivedCommand== "ReadTempOMR")
                {
                    portObj.Write(fcmd, 0, fcmd.Length);
                    StatusLabel.Content = "Command Sent";
                }

            }
            else
            {
                MessageBox.Show("Connect to a port first.");
            }

        }

        //Now click Read Button to show the temp and send it to C++
        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            StatusLabel.Content = "Displaying reply";

            //Receive reply from hardware

            portObj.ReadTimeout = 500;
            string incomingData = "";

            while (portObj.IsOpen && portObj.BytesToRead > 0)
            {
                try
                {
                    portObj.ReadTimeout = 500;
                    //string incomingData = portObj.ReadLine();
                    incomingData = portObj.ReadExisting(); // Read all available data
                    portObj.DiscardInBuffer();
                    portObj.DiscardOutBuffer();
                }
                catch (TimeoutException) { }
            }

            // Ensure that there is enough data to process
            if (incomingData.Length < 16)
            {
                Console.WriteLine("Insufficient data.");
                return;
            }

            char[] rply = incomingData.ToCharArray();

            // Extract Str[0] and Str[1] from rply[5] and rply[6]
            char[] Str = new char[2];
            Str[0] = rply[5];
            Str[1] = rply[6];

            // Extract str from rply[15] to rply[22] (8 characters)
            char[] str = new char[8];
            Array.Copy(rply, 15, str, 0, 8);

            // Convert the extracted string to an integer
            string strHex = new string(str);
            int PV = Convert.ToInt32(strHex, 16);  // Convert hex string to integer

            // Convert the integer PV to float E5CNPV
            int q = PV / 10;
            int r = PV % 10;
            string val = q + "." + r;

            float E5CNPV = float.Parse(val);  // Convert to float

            // Format the result as a string
            temperatureOmron = $"Temp: {E5CNPV:F2}";  // Format to 2 decimal places

            //ReceivedDataTextBox.Clear();
            ReceivedDataTextBox.AppendText(temperatureOmron + " deg" + Environment.NewLine);

            //Sending result to CPP
            writeDataToCPP();
        }


        //Send result to C++
        private void writeDataToCPP()
        {
            // Send response to C++
            byte[] responseBytes = Encoding.UTF8.GetBytes(temperatureOmron); 
            pipeClient.Write(responseBytes, 0, responseBytes.Length);

            //closing pipe
            closePipe();



            //Other codes

            /*var person = new Person
            {
                Name = "FSMB",
                TeamName = "Software",
                Members = 14
            };

            // Serialize the person object to JSON
            string jsonData = JsonSerializer.Serialize(person);
            Console.WriteLine($"Sending JSON Data: {jsonData}"); // Log JSON before sending

            try
            {
                // Create named pipe server
               
                using (var writer = new StreamWriter(server))
                {
                    writer.AutoFlush = true; // Ensure data is sent immediately

                    //for (int i = 0; i < 100; i++)
                    //{
                        writer.WriteLine(jsonData); // Send JSON data
                        //Console.WriteLine($"Sent data {i + 1}: {jsonData}");
                        Console.WriteLine($"Sent data: {jsonData}");
                   // }
                }
                
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IOException occurred: {ex.Message}");
            }*/

        }

        //Close Pipe
        private void closePipe()
        {
            if (pipeClient != null && pipeClient.IsConnected)
             {
                pipeClient.Close();
                 Console.WriteLine("Pipe closed.");
             }
            //SendDataTextBox.Clear();
            // string incomingData = portObj.ReadLine();
        }

        //Closing App
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (portObj != null && portObj.IsOpen)
            {
                portObj.Close();
            }
        }

        
    }
}
