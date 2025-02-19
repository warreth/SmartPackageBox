using static FileModifyDLL.FileHelper;

namespace NonSpecific
{

    public static class ErrorHandler
    {
        private static Exception CatchError(Action function)
        {
            try
            {
                function(); // Execute the function
                return null; // No exception occurred
            }
            catch (Exception e)
            {
                return e; // Return the caught exception
            }
        }
        public static bool HandleError(Action function)
        {
            Exception e = CatchError(function);
            if (e != null)
            {
                Logger.Log(function.Method.Name, e);
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Static class to log errors
    /// </summary>
    public static class Logger
    {
        private static string GetPath()
        {
            // Get the current directory where the application is executed
            string currentDirectory = Directory.GetCurrentDirectory();

            // Construct the path to the log file
            return Path.Combine(currentDirectory, "logs", "log.md");
        }
        /// <summary>
        /// Log errors to the error file
        /// </summary>
        /// <param name="classToUse"></param>
        /// <param name="Exception"></param>
        public static void Log(object classToUse, Exception e)
        {
            DateTime now = DateTime.Now; //get the current date
            string output = $"{now} - {classToUse}\n\t{e}"; //construct the output

            AppendToFile(GetPath(), output); //AppendToFile(fileName, output);
        }

        public static string ReadLog() => ReadFile(GetPath()); //ReadFile(fileName);
    }

}