using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourneyPal.DataHandling.DataObjects
{
    public class ApiRequestedDataHandler
    {
        public string ApiRequestJson { get; set; }
        public string ApiRequestContent { get; set; }
        public string ApiResponse { get; set; }
        public int HostSite { get; set; }
        public List<int> Tournaments { get; set; }
    }
}
