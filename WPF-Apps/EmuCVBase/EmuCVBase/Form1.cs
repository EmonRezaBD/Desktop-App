using System;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;

namespace EmuCVBase
{
    public partial class Form1 : Form
    {
        bool streamVideo = false;
        static int cameraIdx = 0;
        double fpsOld = 30.0;
        double fps;
        DateTime last, now;
        TimeSpan ts;

        static string address = "http://192.168.20.92:8080/video";
        VideoCapture capture = new VideoCapture(address);

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

            streamVideo = true;
            capture.ImageGrabbed += Capture_ImageGrabbed;
            capture.Start();
           // timer1.Enabled = true;
        }

        private void Capture_ImageGrabbed(object sender, System.EventArgs e)
        {
            var frameSize = new System.Drawing.Size(1920, 1080);

            now = DateTime.Now;

            if (streamVideo)
            {
                Mat frame = new Mat();
                capture.Retrieve(frame);

                CvInvoke.Resize(frame, frame, frameSize);

                displayPic.Image = frame.ToBitmap();

                ts = now - last;
                frame.Dispose();

            }

            last = now;

        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            streamVideo = false;
            capture.ImageGrabbed -= Capture_ImageGrabbed;
            capture.Stop();
            //timer1.Enabled =false;
        }

        private void timer1_tick(object sender, EventArgs e)
        {
            if(ts.Milliseconds>0.0)
            {
                double fpsNew = 1/(0.001*ts.TotalMilliseconds);
                fps = 0.9*fpsOld+0.1*fpsNew;
                FPS_Label.Text = fps.ToString("F0")+" FPS";
                fpsOld = fps;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

      

        
    }
}
