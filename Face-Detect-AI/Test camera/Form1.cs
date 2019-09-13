using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test_camera
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void VideoRecording()
        {
            camImg.videoSource.Start();

            while (!StopVideo)
            {
                pictureBox1.Image = camImg.bitmap;
                pictureBox1.Invalidate();
            }
            camImg.videoSource.Stop();

        }

        private void BtnStop_Click(object sender, EventArgs e)
        {

        }
    }
}
