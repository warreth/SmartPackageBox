namespace AI;

using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;
// TODO: Make this actually do something
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
