using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;
using System.Device.Gpio;

// Motor and hatch control
namespace MotorController
{
    public class HatchProperties
    {
        /// <summary>
        /// Indicates the current position of the hatch
        /// </summary>
        /// True: Open
        /// False: Closed
        public bool isOpen = false; //! Make sure users cant control both sides at the same time
        public const byte openPin = 7;
        public const byte closePin = 8;

        public HatchProperties()
        {
            // Set the isOpen value using a sensor if needed
        }
    }

    public class HatchController
    {
        public HatchController()
        {
            // Setup GPIO pins
            controller.OpenPin(HatchProperties.openPin, PinMode.Output);
            controller.OpenPin(HatchProperties.closePin, PinMode.Output);
            // Initialize pins to Low state
            controller.Write(HatchProperties.openPin, PinValue.Low);
            controller.Write(HatchProperties.closePin, PinValue.Low);
        }
        public HatchProperties hatchProperties = new();
        private GpioController controller = new();

        /// <summary>
        /// Opens/Closes the hatch
        /// </summary>
        /// <param name="openAction"></param>
        /// <returns></returns>
        public bool MoveHatch(bool openAction)
        {
            if (openAction)
            {
                // Try to open hatch
                bool didOpen = HandleError(() => OpenHatch());  // Wrap in lambda
                if (didOpen)
                {
                    hatchProperties.isOpen = true;
                }
            }
            else if (!openAction)
            {
                // Try to close hatch
                bool didClose = HandleError(() => CloseHatch());  // Wrap in lambda
                if (didClose)
                {
                    hatchProperties.isOpen = false;
                }
            }
            return true;
        }
        // Open hatch
        private bool OpenHatch()
        {
            controller.Write(HatchProperties.closePin, PinValue.Low);
            Thread.Sleep(1000);
            controller.Write(HatchProperties.openPin, PinValue.High);
            Log("MotorController", "Hatch Opened");
            return true;
        }
        // Close hatch
        private bool CloseHatch()
        {
            controller.Write(HatchProperties.openPin, PinValue.Low);
            Thread.Sleep(1000);
            controller.Write(HatchProperties.closePin, PinValue.High);
            Log("MotorController", "Hatch Closed");
            return true;
        }
        // Stop hatch
        public bool StopHatch()
        {
            controller.Write(HatchProperties.openPin, PinValue.Low);
            controller.Write(HatchProperties.closePin, PinValue.Low);
            Log("MotorController", "Hatch Stopped");
            return true;
        }
    }
}