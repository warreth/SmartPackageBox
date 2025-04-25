namespace Functions;
using MotorController;


public static class StaticFunctions
{
    HatchController hatch = new();

    public void packageIsDetected()
    {
        Console.WriteLine("Package is detected");
        hatch.MoveHatch(true);

        if (!hatch.hatchProperties.isOpen) //If closed
        {
            hatch.MoveHatch(true); //Open the hatch
        }
    }
}