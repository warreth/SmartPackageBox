using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;
using System.Device.Gpio;

namespace MotorController
{
    public class HatchProperties
    {
        /// <summary>
        /// Indicates the action to be performed on the hatch.
        /// </summary>
        /// <remarks>
        /// True: Opens the hatch.
        /// False: Closes the hatch.
        /// </remarks>
        public bool openAction;

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
            // set the isOpen value using a light sensor or something
        }
    }

    public class HatchController
    {
        public HatchController()
        {

            // Couldn't write to pin 8 because it was not open.
            controller.OpenPin(HatchProperties.openPin, PinMode.Output);
            controller.OpenPin(HatchProperties.closePin, PinMode.Output);

            // Initialize pins to Low state
            controller.Write(HatchProperties.openPin, PinValue.Low);
            controller.Write(HatchProperties.closePin, PinValue.Low);
        }
        public HatchProperties hatchProperties = new();
        public GpioController controller = new();

        public bool MoveHatch(bool openAction)
        {
            if (hatchProperties.openAction)
            {

                bool didOpen = HandleError(() => OpenHatch());  // Wrap in lambda
                if (didOpen)
                {
                    hatchProperties.isOpen = true;
                }

            }
            else if (!hatchProperties.openAction)
            {
                bool didClose = HandleError(() => CloseHatch());  // Wrap in lambda
                if (didClose)
                {
                    hatchProperties.isOpen = false;
                }
            }
            return true;
        }
        private void OpenHatch()
        {
            controller.Write(HatchProperties.closePin, PinValue.Low);
            Thread.Sleep(1000);
            controller.Write(HatchProperties.openPin, PinValue.High);
            Console.WriteLine("Hatch Opened");
            Log("MotorController", "Hatch Opened");
        }
        private void CloseHatch()
        {
            controller.Write(HatchProperties.openPin, PinValue.Low);
            Thread.Sleep(1000);
            controller.Write(HatchProperties.closePin, PinValue.High);
            Console.WriteLine("Hatch Closed");
            Log("MotorController", "Hatch Closed");
        }
        private void StopHatch()
        {
            controller.Write(HatchProperties.openPin, PinValue.Low);
            controller.Write(HatchProperties.closePin, PinValue.Low);
            Console.WriteLine("Hatch Stopped");
            Log("MotorController", "Hatch Stopped");
        }
    }
}