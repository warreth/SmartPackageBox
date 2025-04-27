using AI;
using Functions;
using MotorController;


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
                Console.WriteLine("Package is not detected");
            }
        }
    }
}