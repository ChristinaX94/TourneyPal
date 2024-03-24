using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourneyPal.Commons
{
    public static class Logger
    {
        public static string message { get; set; }
        public static string exceptionMessage { get; set; }

        public static void log(string messageItem, string exceptionMessageItem)
        {
            message = messageItem;
            exceptionMessage = exceptionMessageItem;
            writeToDB();
        }

        private static void writeToDB()
        {
            throw new NotImplementedException();
        }
    }
}
