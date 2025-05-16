using static NonSpecific.ErrorHandler;
using static NonSpecific.Logger;
using System.Device.Gpio;

namespace MotorController
{
    public class HatchProperties //TODO: Make a shared instance or use a stored json file for the isOpen value
    {
        /// <summary>
        /// Indicates the current position of the hatch
        /// </summary>
        /// True: Open
        /// False: Closed
        public bool isOpen = false; //! Make sure users cant control both sides at the same time
        public const byte openPin = 8;
        public const byte closePin = 7;

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

                bool didOpen = HandleError(() => OpenHatch());  // Wrap in lambda
                if (didOpen)
                {
                    hatchProperties.isOpen = true;
                }

            }
            else if (!openAction)
            {
                bool didClose = HandleError(() => CloseHatch());  // Wrap in lambda
                if (didClose)
                {
                    hatchProperties.isOpen = false;
                }
            }
            return true;
        }
        private bool OpenHatch()
        {
            controller.Write(HatchProperties.closePin, PinValue.Low);
            Thread.Sleep(1000);
            controller.Write(HatchProperties.openPin, PinValue.High);
            Log("MotorController", "Hatch Opened");
            return true;
        }
        private bool CloseHatch()
        {
            controller.Write(HatchProperties.openPin, PinValue.Low);
            Thread.Sleep(1000);
            controller.Write(HatchProperties.closePin, PinValue.High);
            Log("MotorController", "Hatch Closed");
            return true;
        }
        public bool StopHatch()
        {
            controller.Write(HatchProperties.openPin, PinValue.Low);
            controller.Write(HatchProperties.closePin, PinValue.Low);
            Log("MotorController", "Hatch Stopped");
            return true;
        }
    }
}