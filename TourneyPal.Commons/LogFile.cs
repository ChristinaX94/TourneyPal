namespace TourneyPal.Commons
{
    public static class LogFile
    {
        private const string LogDirectoryPath = @".\Log";
        private const string LogFilePath = LogDirectoryPath + @"\Log.txt";
        public static void WriteToLogFile(string message)
        {
            try
            {
                if (!Directory.Exists(LogDirectoryPath))
                {
                    Directory.CreateDirectory(LogDirectoryPath);
                }

                List<string> line = new List<string> { Common.getDate() + ": " + message + "\n" };
                File.AppendAllLines(LogFilePath, line);
            }
            catch(Exception ex)
            { 
                Console.WriteLine(Common.getDate() + ": Something went wrong writing to LogFile! -> " + ex.ToString());
            }
        }
    }
}
