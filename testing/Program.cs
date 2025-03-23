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

while (keepgoing)
{
    System.Console.WriteLine("Do you want to move the hatch? (y/n)");
    string command = Console.ReadLine();
    if (command == "y")
    {
        if (hatch.hatchProperties.isOpen)
        {
            hatch.MoveHatch(false); //Close the hatch
        }
        if (!hatch.hatchProperties.isOpen) //If closed
        {
            hatch.MoveHatch(true); //Open the hatch
        }
    }
    else
    {
        keepgoing = false;
    }
}

