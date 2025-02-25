using Emgu.CV;

namespace EmuCVBase
{
    public partial class Form1 : Form
    {
        VideoCapture capture;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGrab_Click(object sender, EventArgs e)
        {
            string address = "http://192.168.20.92:8080/video";

            Mat frame = new Mat();
            //capture=new VideoCapture(0);
            capture=new VideoCapture(address);

            bool readSuccess = capture.Read(frame);

            if (readSuccess)
            {
                displayPic.Image = frame.ToBitmap();
                frame.Dispose();
            }

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
