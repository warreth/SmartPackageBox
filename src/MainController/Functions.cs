namespace Functions;
using MotorController;
using CameraFeed;

public class Function
{
    HatchController hatch = new();

    public void packageIsDetected()
    {
        Console.WriteLine("Package is detected");
        byte[] tImage = CameraFeed.TakePicture();


        if (File.Exists($"latest.png"))
        {
            CameraFeed.SaveImage(tImage, "latest.png");
        }

        if (!hatch.hatchProperties.isOpen) //If closed
        {
            hatch.MoveHatch(true); //Open the hatch
        }
        else
        {
            Console.WriteLine("Hatch is already open");
        }

        //sentNotification();
    }
}

public class StaticFunctions
{

}