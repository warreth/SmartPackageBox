namespace FileModifyDLL
{
    public class FileHelper
    {
        public static string? GetString(string? message)
        {
            System.Console.WriteLine($"{message}: ");
            return Console.ReadLine();
        }
        public static string? ReadFile(string path)
        {
            return File.ReadAllText(path);
        }
        public static string? AppendToFile(string path, string value)
        {
            string error;

            try
            {
                //File.Create(path);
                File.AppendAllText(path, value);
                return "True"; // Success
            }
            catch (Exception ex)
            {
                error = ex.Message; // Capture the error message
                return error; // Indicate failure
            }
        }
        public static string? WriteFile(string path, string value)
        {
            string error;

            try
            {
                File.WriteAllText(path, value);
                return "True"; // Success
            }
            catch (Exception ex)
            {
                error = ex.Message; // Capture the error message
                return error; // Indicate failure
            }
        }
    }
}