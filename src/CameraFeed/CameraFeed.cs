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
    public static Bitmap TakePicture(VideoCapture capture)
    {
        Bitmap image = capture.QueryFrame().ToBitmap(); // Take the current frame, and convert it into a bitmap
        return image;
    }
    public static void StopCamera(VideoCapture capture)
    {
        capture.Dispose(); // Dispose of the VideoCapture object
    }

    public static void SaveImage(Bitmap image, string name)
    {
        // Save the image as PNG to ensure cross-platform compatibility
        // Add error checking for null arguments and invalid file names
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image), "Image cannot be null.");
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("File name cannot be null or whitespace.", nameof(name));
        }

        // Ensure the file name ends with .png for clarity
        string fileName = name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? name : $"{name}.png";
        image.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
    }
}
