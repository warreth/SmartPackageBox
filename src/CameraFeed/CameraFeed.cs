using Emgu.CV;
using Emgu.CV.Structure;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using System.IO;
using System.Threading;
using static NonSpecific.Logger;

// Camera feed and image handling
namespace CameraFeed
{
    public static class CameraFeed
    {
        // Global camera instance
        private static VideoCapture? _globalCapture; // Keep a single instance
        private static readonly object _lock = new object(); // For thread safety

        // Start or get camera
        private static VideoCapture GetOrStartCamera()
        {
            lock (_lock)
            {
                if (_globalCapture == null || !_globalCapture.IsOpened)
                {
                    _globalCapture?.Dispose(); // Dispose if it exists but isn't open
                    _globalCapture = new VideoCapture(0); // Open default camera
                    if (_globalCapture == null || !_globalCapture.IsOpened)
                    {
                        Log("CameraFeed", "Failed to open camera.");
                        throw new InvalidOperationException("Failed to open camera.");
                    }
                    Log("CameraFeed", "Camera started/re-opened successfully.");
                    System.Threading.Thread.Sleep(1000); // Increased delay, might help
                }
                return _globalCapture;
            }
        }

        // Take a picture and return bytes
        public static byte[] TakePicture()
        {
            VideoCapture capture = GetOrStartCamera();
            byte[] imageBytes = Array.Empty<byte>();
            int maxRetries = 5;
            int grabRetries = 3;
            int delayMs = 300;
            bool frameCapturedSuccessfully = false;

            // Clear camera buffer
            lock (_lock)
            {
                int dummyGrabs = 8; // Enough grabs to remove old frames
                for (int i = 0; i < dummyGrabs; i++)
                {
                    capture.Grab(); // Dummy grab
                    Thread.Sleep(30); // Small delay
                }
            }

            // Try to capture frame
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                lock (_lock)
                {
                    if (!capture.IsOpened)
                    {
                        Log("CameraFeed", $"[WARNING] Camera not open at start of attempt {attempt}. Trying to re-open.");
                        _globalCapture?.Dispose();
                        _globalCapture = null;
                        try
                        {
                            capture = GetOrStartCamera();
                        }
                        catch (Exception ex)
                        {
                            Log("CameraFeed", $"[ERROR] Failed to re-open camera in attempt {attempt}: {ex.Message}");
                            Thread.Sleep(delayMs);
                            continue;
                        }
                    }
                    bool currentFrameGrabbed = false;
                    for (int gr = 1; gr <= grabRetries; gr++)
                    {
                        if (capture.Grab())
                        {
                            using (var frameMat = new Mat())
                            {
                                if (capture.Retrieve(frameMat) && !frameMat.IsEmpty)
                                {
                                    using (var img = frameMat.ToImage<Bgr, byte>())
                                    {
                                        if (img == null)
                                        {
                                            continue;
                                        }
                                        // Convert frame to PNG bytes
                                        var imageSharpImg = new Image<Rgb24>(img.Width, img.Height);
                                        for (int y = 0; y < img.Height; y++)
                                        {
                                            for (int x = 0; x < img.Width; x++)
                                            {
                                                var color = img[y, x];
                                                imageSharpImg[x, y] = new Rgb24((byte)color.Red, (byte)color.Green, (byte)color.Blue);
                                            }
                                        }
                                        using (var ms = new MemoryStream())
                                        {
                                            imageSharpImg.Save(ms, new PngEncoder());
                                            imageBytes = ms.ToArray();
                                        }
                                        imageSharpImg.Dispose();
                                        frameCapturedSuccessfully = true;
                                        currentFrameGrabbed = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (currentFrameGrabbed) break;
                        Thread.Sleep(delayMs / 2);
                    }
                    if (frameCapturedSuccessfully)
                    {
                        break;
                    }
                    if (attempt >= maxRetries / 2 && attempt < maxRetries)
                    {
                        Log("CameraFeed", $"[WARNING] Attempting to re-initialize camera due to persistent frame capture failure (attempt {attempt}).");
                        _globalCapture?.Dispose();
                        _globalCapture = null;
                        try
                        {
                            capture = GetOrStartCamera();
                        }
                        catch (Exception ex)
                        {
                            Log("CameraFeed", $"[ERROR] Failed to re-initialize camera during retry: {ex.Message}");
                        }
                    }
                }
                if (frameCapturedSuccessfully) break;
                Thread.Sleep(delayMs);
            }
            if (!frameCapturedSuccessfully)
            {
                Log("CameraFeed", $"[ERROR] Failed to capture frame after {maxRetries} attempts.");
                throw new InvalidOperationException("Failed to capture frame.");
            }
            return imageBytes;
        }

        // Release camera resource
        public static void ReleaseCamera()
        {
            lock (_lock)
            {
                if (_globalCapture != null)
                {
                    _globalCapture.Dispose();
                    _globalCapture = null;
                    Log("CameraFeed", "Global camera capture released.");
                }
            }
        }

        public static void StopCamera(VideoCapture? capture) // Kept for if needed
        {
            if (capture != null)
            {
                capture.Dispose();
            }
        }

        // Save image to file
        public static void SaveImage(byte[] imageBytes, string name)
        {
            // Check for null or empty image bytes
            if (imageBytes == null || imageBytes.Length == 0)
            {
                Log("CameraFeed", "Image bytes cannot be null or empty.");
                throw new ArgumentException("Image bytes cannot be null or empty.");
            }
            // Check for null or whitespace name
            if (string.IsNullOrWhiteSpace(name))
            {
                Log("CameraFeed", "Name cannot be null or whitespace.");
                throw new ArgumentException("Name cannot be null or whitespace.");
            }
            // Check if imageBytes is a valid PNG image (corruption check)
            try
            {
                using (var ms = new MemoryStream(imageBytes))
                {
                    var decoder = SixLabors.ImageSharp.Image.DetectFormat(ms);
                    if (decoder == null || decoder.Name != "PNG")
                    {
                        Log("CameraFeed", "[ERROR] Image bytes are not a valid PNG file or are corrupted.");
                        throw new InvalidDataException("Image bytes are not a valid PNG file or are corrupted.");
                    }
                    // Try to load the image to check for corruption
                    ms.Position = 0;
                    try
                    {
                        using (var img = SixLabors.ImageSharp.Image.Load(ms))
                        {
                            // If loading succeeds, image is valid
                        }
                    }
                    catch (Exception ex)
                    {
                        Log("CameraFeed", $"[ERROR] Image bytes are corrupted or unreadable: {ex.Message}");
                        throw new InvalidDataException("Image bytes are corrupted or unreadable.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Log("CameraFeed", $"[ERROR] Exception during image validation: {ex.Message}");
                throw;
            }
            string fileName = $"{name}";
            int maxRetries = 5;
            int delayMs = 200;
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    File.WriteAllBytes(fileName, imageBytes);
                    Log("CameraFeed", $"Image saved as {fileName}");
                    return;
                }
                catch (IOException ex)
                {
                    if (attempt == maxRetries)
                    {
                        Log("CameraFeed", $"[ERROR] Failed to save image after {maxRetries} attempts: {ex.Message}");
                        throw;
                    }
                    Log("CameraFeed", $"[WARNING] File in use, retrying ({attempt}/{maxRetries})...");
                    Thread.Sleep(delayMs);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Log("CameraFeed", $"[ERROR] Unauthorized access when saving image: {ex.Message}");
                    throw;
                }
                catch (ArgumentException ex)
                {
                    Log("CameraFeed", $"[ERROR] Invalid file name or argument: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Log("CameraFeed", $"[ERROR] Unexpected error when saving image: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
