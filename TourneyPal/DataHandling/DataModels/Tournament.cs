using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourneyPal.DataHandling.DataModels
{
    public class Tournament
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
        public int? GameID { get; set; }
        public int? TournamentHostSiteID { get; set; }
        public int? SiteTournamentID { get; set; }
        public bool? IsExpired { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
