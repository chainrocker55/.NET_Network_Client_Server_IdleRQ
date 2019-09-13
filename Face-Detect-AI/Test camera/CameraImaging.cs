using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Video.DirectShow;
using System.Drawing;
using AForge.Video;

namespace Test_camera
{
    public class CameraImaging
    {
        // enumerate video devices
        public FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        //camera
        public VideoCaptureDevice videoSource;
        //screen shot
        public Bitmap bitmap;
        public CameraImaging()
        {
            // create video source
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            // set NewFrame event handler
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
        }
        public void StartVideo(VideoCaptureDevice videoSource)
        {
            // start the video source
            videoSource.Start();
            // ...
        }
        public void StopVideo(VideoCaptureDevice videoSource)
        {
            // stop the video source
            videoSource.Stop();
            // ...
        }
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // get new frame
            bitmap = eventArgs.Frame;
            // process the frame
        }

    }
}