namespace CameraFeed;
using Emgu.CV;
using System.Drawing;

public static class CameraFeed
{
    public static VideoCapture StartCamera()
    {
        VideoCapture capture = new VideoCapture(0); // Make a new VideoCapture object (open the webcam)
        return capture;
    }
    public static void TakePicture(VideoCapture capture)
    {
        Bitmap image = capture.QueryFrame().ToBitmap(); // Take the current frame, and convert it into a bitmap
        //TODO: now save it using maybe image.Save(name.jpg)
    }
}
