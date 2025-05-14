using MotorController;

HatchController hatch = new();

/*
System.Console.WriteLine("Give your command (open, close, stop)");
bool keepgoing = true;

while (keepgoing)
{
    System.Console.Write("> ");
    string command = Console.ReadLine();
    switch (command)
    {
        case "open":
            hatch.OpenHatch();
            break;
        case "close":
            hatch.CloseHatch();
            break;
        case "stop":
            hatch.StopHatch();
            break;
    }
}
*/

bool keepgoing = true;
hatch.StopHatch();

while (keepgoing)
{
    System.Console.WriteLine("Move the hatch? (n=stop)");
    string? command = Console.ReadLine();
    if (command != "n")
    {
        hatch.MoveHatch(!hatch.hatchProperties.isOpen); //Open the hatch
    }
    else
    {
        hatch.StopHatch();
        keepgoing = false;
    }
}

