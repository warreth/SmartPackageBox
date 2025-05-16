namespace AI;

using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;

public class AiHelper
{
    public bool detectPackageAI()
    {
        System.Console.WriteLine("Do you want to run the main program? (y/n)");
        string input = Console.ReadLine();
        if (input == "y")
        {
            Log("AiHelper", "Package detected");
            return true;
        }
        else
        {
            return false;
        }
    }
}
