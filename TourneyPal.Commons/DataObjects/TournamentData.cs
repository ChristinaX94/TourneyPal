using EnumsNET;

namespace TourneyPal.Commons.DataObjects
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
        public int NumberOfEntrants { get; set; }
        public Common.Game GameEnum { get; set; }
        public string Game => GameEnum==null? string.Empty: ((Common.Game)GameEnum).AsString(EnumFormat.Description);
        public List<string> Streams { get; set; } = new List<string>();
        public string HostSite { get; set; }
        public bool isModified { get; set; } = false;
    }

}
