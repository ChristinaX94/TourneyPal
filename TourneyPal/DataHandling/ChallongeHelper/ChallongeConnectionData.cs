using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourneyPal.DataHandling.ChallongeHelper
{
    public static class ChallongeConnectionData
    {
        
        /// <summary>
        /// Auth Token of Challonge
        /// </summary>
        public const string ChallongeToken = "XIkXgh8o82D8ulgwJ4viTCd613Kq2SxPobjRv77l";

        /// <summary>
        /// Username of Challonge User
        /// </summary>
        public const string ChallongeUser = "Frieden";

        /// <summary>
        /// API Endpoint of Challonge 
        /// https://username:api-key@api.challonge.com/v1/...
        /// </summary>
        public const string ChallongeEndpoint = "https://"+ ChallongeUser + ":"+ ChallongeToken + "@api.challonge.com/v1/tournaments/";

        /// <summary>
        /// Always requesting Challonge data in json format
        /// </summary>
        private const string ChallongeDataFormat = ".json";

        public static string getFullChallongeApiRequestURI(string tournamentID)
        {
            return ChallongeEndpoint + tournamentID + ChallongeDataFormat;
        }

    }
}
