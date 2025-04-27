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

    /// <summary>
    /// Takes a picture using the provided VideoCapture instance, or creates one if none is provided.
    /// </summary>
    /// <param name="capture">Optional VideoCapture instance. If null, a new one is created and disposed.</param>
    /// <returns>Bitmap image of the captured frame.</returns>
    public static Bitmap TakePicture(VideoCapture? capture = null)
    {
        bool createdCapture = false;

        if (capture == null) // If no capture was given, create one
        {
            capture = StartCamera();
            createdCapture = true;
        }

        // Error checking: ensure capture is not null and is opened
        if (capture == null)
        {
            throw new ArgumentNullException(nameof(capture), "VideoCapture instance cannot be null.");
        }
        if (!capture.IsOpened)
        {
            if (createdCapture)
            {
                StopCamera(capture);
            }
            throw new InvalidOperationException("The VideoCapture device is not opened.");
        }

        // Query a frame and convert it to Bitmap
        using (var frame = capture.QueryFrame())
        {
            if (frame == null)
            {
                if (createdCapture)
                {
                    StopCamera(capture);
                }
                throw new InvalidOperationException("Failed to capture a frame from the camera.");
            }
            Bitmap image = frame.ToBitmap();
            // If we created the capture, dispose it
            if (createdCapture)
            {
                StopCamera(capture);
            }
            return image;
        }
    }

    public static void StopCamera(VideoCapture capture)
    {
        capture.Dispose(); // Dispose of the VideoCapture object
    }

    public static void SaveImage(Bitmap image, string name)
    {
        string fileName = $"{name}.png";
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image), "Image cannot be null.");
        }

        image.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
    }
}
