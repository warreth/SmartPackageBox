using AI;
using Functions;
using MotorController;
using static Functions.StaticFunctions;

AiHelper aiHelper = new();

while (true)
{
    if (aiHelper.detectPackageAI())
    {
        packageIsDetected();
    }
    else
    {
        Console.WriteLine("Package is not detected");
    }
}
public class Program
{

}

