namespace EmuCVBase
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            displayPic = new PictureBox();
            btnStart = new Button();
            btnStop = new Button();
            btnExit = new Button();
            FPS_Label = new Label();
            ((System.ComponentModel.ISupportInitialize)displayPic).BeginInit();
            SuspendLayout();
            // 
            // displayPic
            // 
            displayPic.Location = new Point(42, 28);
            displayPic.Name = "displayPic";
            displayPic.Size = new Size(544, 317);
            displayPic.SizeMode = PictureBoxSizeMode.Zoom;
            displayPic.TabIndex = 0;
            displayPic.TabStop = false;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(173, 365);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(92, 53);
            btnStart.TabIndex = 1;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(305, 365);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(92, 53);
            btnStop.TabIndex = 2;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnExit
            // 
            btnExit.Location = new Point(693, 365);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(81, 53);
            btnExit.TabIndex = 3;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // FPS_Label
            // 
            FPS_Label.AutoSize = true;
            FPS_Label.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FPS_Label.ForeColor = Color.Red;
            FPS_Label.Location = new Point(540, 372);
            FPS_Label.Name = "FPS_Label";
            FPS_Label.Size = new Size(46, 30);
            FPS_Label.TabIndex = 4;
            FPS_Label.Text = "FPS";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(FPS_Label);
            Controls.Add(btnExit);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(displayPic);
            Name = "Form1";
            Text = "Object Detection App";
            ((System.ComponentModel.ISupportInitialize)displayPic).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox displayPic;
        private Button btnStart;
        private Button btnStop;
        private Button btnExit;
        private Label FPS_Label;
    }
}
