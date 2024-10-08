using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.SQLManager;
using TourneyPal.SQLManager.DataModels.SQLTables.Errorlogs;
using TourneyPal.SQLManager.DataModels.SQLTables.Stream;
using TourneyPal.SQLManager.DataModels.SQLTables.Tournament_Api_Data;

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

                errorlogs.rows.Add(errorlogsRow);

                errorlogs = (Errorlogs)SQLHandler.saveData(errorlogs);
                if (errorlogs == null)
                {
                    return;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }
    }
}
