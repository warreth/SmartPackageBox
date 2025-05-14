using static CameraFeed.CameraFeed;
using System.Threading.Tasks;
using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;

// Take a picture and handle any errors, ensuring the program doesn't crash
byte[]? tImage = null;

if (!File.Exists($"latest.png"))
{
    Log("Function", "latest.png does not exist.");
    File.Create("latest.png");
}

bool success = HandleError(() =>
{
    tImage = TakePicture();
});

if (!success || tImage == null)
{
    Log("Function", "Failed to take picture.");
}
else
{
    Log("Function", "Took picture.");
}

if (tImage != null)
{
    SaveImage(tImage, "latest.png");
    Log("Function", "Saved image to latest.png");
}
else
{
    Log("Function", "Failed to save image to latest.png");
}