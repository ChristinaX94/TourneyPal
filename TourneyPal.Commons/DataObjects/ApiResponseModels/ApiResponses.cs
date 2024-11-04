using static TourneyPal.Commons.Common;

namespace TourneyPal.Commons.DataObjects.ApiResponseModels
{
    public class ApiSimpleResponse
    {
        public bool Success { get; set; }
        public LogMessage? Error { get; set; } = new LogMessage();
        
    }

    public class ApiGameResponse : ApiSimpleResponse
    {
        public Game Game { get; set; }
    }

    public class ApiResponse: ApiSimpleResponse
    {
        public List<TournamentData> Tournaments { get; set; } = new List<TournamentData> { };
        public List<ApiRequestedDataHandler> Requests { get; set; } = new List<ApiRequestedDataHandler> { };
    }

    public class ApiRequestResponse : ApiResponse
    {
        public ApiRequestedDataHandler ApiRequestedData { get; set; } = new ApiRequestedDataHandler();
        public object CallerData { get; set; }
    }
}
