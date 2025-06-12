using System.Reflection;
using TourneyPal.SQLManager;
using TourneyPal.SQLManager.DataModels.SQLTables.Errorlogs;

namespace TourneyPal.Commons
{
    public static class Logger
    {
        private static string? message { get; set; }
        private static string? exceptionMessage { get; set; }
        private static string? foundIn { get; set; }

        public static void log(MethodBase? foundInItem, string? messageItem = null, string? exceptionMessageItem = null)
        {
            message = messageItem;
            exceptionMessage = exceptionMessageItem;
            foundIn = foundInItem?.ReflectedType?.Name + "." + foundInItem?.Name;

            writeToDB();
        }

        private static void writeToDB()
        {
            try
            {
                var errorlogs = new Errorlogs();
                var errorlogsRow = new ErrorlogsRow(nameof(errorlogs));
                errorlogsRow.insertNewRowData();
                errorlogsRow.Message = message;
                errorlogsRow.ExceptionMessage = exceptionMessage;
                errorlogsRow.FoundIn = foundIn;

                errorlogs.Rows.Add(errorlogsRow);

                errorlogs = (Errorlogs)SQLHandler.saveData(errorlogs);
                if (errorlogs == null)
                {
                    return;
                }

                LogFile.WriteToLogFile("** Error found at ID: " + errorlogsRow.ID);

            }
            catch (Exception ex)
            {
                LogFile.WriteToLogFile("EXCEPTION: " + ex.Message);
            }
        }
    }
}
