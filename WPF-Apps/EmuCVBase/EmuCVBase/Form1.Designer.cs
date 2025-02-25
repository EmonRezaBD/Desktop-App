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
            btnGrab = new Button();
            btnExit = new Button();
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
            // btnGrab
            // 
            btnGrab.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnGrab.Location = new Point(157, 383);
            btnGrab.Name = "btnGrab";
            btnGrab.Size = new Size(121, 40);
            btnGrab.TabIndex = 1;
            btnGrab.Text = "Grab";
            btnGrab.UseVisualStyleBackColor = true;
            btnGrab.Click += btnGrab_Click;
            // 
            // btnExit
            // 
            btnExit.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnExit.Location = new Point(628, 398);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(121, 40);
            btnExit.TabIndex = 2;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnExit);
            Controls.Add(btnGrab);
            Controls.Add(displayPic);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)displayPic).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox displayPic;
        private Button btnGrab;
        private Button btnExit;
    }
}
