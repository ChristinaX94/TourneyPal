using System.Reflection;

namespace TourneyPal.Commons.DataObjects
{
    public class LogMessage
    {
        public MethodBase? FoundInItem { get; set; }
        public string? MessageItem { get; set; }
        public string? ExceptionMessageItem { get; set; }
    }
}
