using static FileModifyDLL.FileHelper;

namespace NonSpecific
{

    public static class ErrorHandler
    {
        private static Exception? CatchError(Action function)
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
            Exception? e = CatchError(function);
            if (e != null)
            {
                Logger.Log($"[ERROR] {function.Method.Name}", e.ToString());
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
        private static bool bWriteToConsole = true;
        private static string GetPath()
        {
            // Get the current directory where the application is executed
            string currentDirectory = Directory.GetCurrentDirectory();
            //System.Console.WriteLine(currentDirectory); //! DEBUG

            // Construct the path to the log file
            return Path.Combine(currentDirectory, "log.md");
        }
        /// <summary>
        /// Log errors to the error file
        /// </summary>
        /// <param name="Subject"></param>
        /// <param name="TextToWrite"></param>
        public static void Log(string subject, string text)
        {
            DateTime now = DateTime.Now; //get the current date
            string output = $"{now} - {subject}:\n\t{text}\n"; //construct the output

            AppendToFile(GetPath(), output); //AppendToFile(fileName, output);

            if (bWriteToConsole)
            {
                System.Console.WriteLine(output); // DEBUGGING to console
            }
        }

        public static string ReadLog() => ReadFile(GetPath())!; //ReadFile(fileName);
    }

}