using AI;
using Functions;
using MotorController;
using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;

public class Program
{
    public static void Main(string[] args)
    {
        AiHelper aiHelper = new();
        Function function = new();

        // Infinite loop to continuously check for package detection
        while (true)
        {
            if (aiHelper.detectPackageAI())
            {
                function.packageIsDetected();
            }
            else
            {
                Log("Program", "Package is not detected");
            }
        }
    }
}