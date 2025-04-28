using Emgu.CV;
using System;
using System.IO;
using SixLabors.ImageSharp; // ImageSharp for cross-platform image handling
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

namespace CameraFeed
{
    public static class CameraFeed
    {
        // Start the camera and return a VideoCapture object
        public static VideoCapture StartCamera()
        {
            var capture = new VideoCapture(0); // Open default camera
            if (capture == null || !capture.IsOpened)
            {
                throw new InvalidOperationException("Failed to open camera.");
            }
            return capture;
        }

        // Take a picture and return the image as a byte array (PNG format)
        public static byte[] TakePicture(VideoCapture? capture = null)
        {
            bool createdCapture = false;
            if (capture == null) // If no capture is given, make one
            {
                capture = StartCamera();
                createdCapture = true;
            }
            if (capture == null || !capture.IsOpened)
            {
                if (createdCapture && capture != null) { StopCamera(capture); }
                throw new InvalidOperationException("Camera is not available.");
            }
            var frame = capture.QueryFrame();
            if (frame == null)
            {
                if (createdCapture) { StopCamera(capture); }
                throw new InvalidOperationException("Failed to capture frame.");
            }

            // Convert Mat to byte array using ImageSharp for Linux compatibility
            var img = frame.ToImage<Emgu.CV.Structure.Bgr, byte>();
            if (img == null)
            {
                if (createdCapture) { StopCamera(capture); }
                throw new InvalidOperationException("Failed to convert frame to EmguCV Image.");
            }

            var image = new Image<Rgb24>(img.Width, img.Height); // Create ImageSharp image
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    var color = img[y, x]; // Bgr
                    // Convert BGR to RGB and assign to ImageSharp image
                    image[x, y] = new Rgb24((byte)color.Red, (byte)color.Green, (byte)color.Blue);
                }
            }
            var ms = new MemoryStream();
            image.Save(ms, new PngEncoder()); // Save as PNG
            byte[] imageBytes = ms.ToArray();
            ms.Dispose();
            image.Dispose();

            if (createdCapture) { StopCamera(capture); }
            return imageBytes;
        }

        // Dispose the VideoCapture object
        public static void StopCamera(VideoCapture capture)
        {
            if (capture != null)
            {
                capture.Dispose();
            }
        }

        // Save the image bytes to a PNG file
        public static void SaveImage(byte[] imageBytes, string name)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                throw new ArgumentException("Image bytes cannot be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or whitespace.");
            }
            string fileName = $"{name}.png";
            File.WriteAllBytes(fileName, imageBytes);
        }
    }
}
