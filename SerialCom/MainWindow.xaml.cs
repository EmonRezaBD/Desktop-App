using System;
using System.IO.Ports;
using System.Windows;

namespace SerialCommunicationWPF
{
    public partial class MainWindow : Window
    {
        private SerialPort _serialPort;

        public MainWindow()
        {
            InitializeComponent();
            LoadAvailablePorts();
        }

        private void LoadAvailablePorts()
        {
            string[] ports = SerialPort.GetPortNames();
            ComboBoxPorts.ItemsSource = ports;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort == null)
            {
                string selectedPort = ComboBoxPorts.SelectedItem.ToString();
                _serialPort = new SerialPort(selectedPort, 9600);
                try
                {
                    _serialPort.Open();
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
                _serialPort.Close();
                StatusLabel.Content = "Disconnected";
                ConnectButton.Content = "Connect";
                _serialPort = null;
            }
        }

      
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
               // string dataToSend = SendDataTextBox.Text;
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


                // _serialPort.WriteLine(dataToSend);
                //SendCommand(fcmd);
                _serialPort.Write(fcmd, 0, fcmd.Length);
                StatusLabel.Content = "Command Sent";
                //SendDataTextBox.Clear();
                // string incomingData = _serialPort.ReadLine();
                _serialPort.ReadTimeout = 500;

                while (_serialPort.IsOpen)
                {
                    try
                    {
                        //string incomingData = _serialPort.ReadLine();
                        string incomingData = _serialPort.ReadExisting(); // Read all available data
                        ReceivedDataTextBox.AppendText(incomingData + Environment.NewLine);
                    }
                    catch (TimeoutException) { }
                }

                int x = 10 + 20;

            }
            else
            {
                MessageBox.Show("Connect to a port first.");
            }
        }

        private void ReceiveData()
        {
            while (_serialPort.IsOpen)
            {
                try
                {
                    string incomingData = _serialPort.ReadLine();
                    ReceivedDataTextBox.AppendText(incomingData + Environment.NewLine);
                }
                catch (TimeoutException) { }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }
    }
}
