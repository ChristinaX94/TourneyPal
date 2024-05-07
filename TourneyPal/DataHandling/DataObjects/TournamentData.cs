using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourneyPal.DataHandling.DataObjects
{

    public class TournamentData
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public string AddrState { get; set; }
        public DateTime? StartsAT { get; set; }
        public bool? Online { get; set; }
        public string URL { get; set; }
        public int? State { get; set; }
        public string VenueAddress { get; set; }
        public string VenueName { get; set; }
        public bool? RegistrationOpen { get; set; }
        public int? NumberOfAttendees { get; set; }
        public string Game { get; set; }
        public List<string> Streams { get; set; }
        public string TournamentHostSite { get; set; }

        public bool isModified { get; set; } = false;
    }

}
