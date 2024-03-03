using DSharpPlus.SlashCommands;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading;
using System.Xml.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using TourneyPal.DataHandling.StartGGHelper;
using static TourneyPal.DataHandling.StartGGHelper.StartGGJsonObject;
using TourneyPal.DataHandling.DataObjects;
using EnumsNET;

namespace TourneyPal.DataHandling
{
    
    public class ApiHandler
    {
        /// <summary>
        /// 9 am (EET) 
        /// </summary>
        public const int TimeOfDayRefreshData = 9;

        public ApiHandler()
        {
            handleData();
        }

        
        public async Task runAsync()
        {
            try
            {
                var timer = new PeriodicTimer(TimeSpan.FromHours(1));
                var time = System.DateTime.Now;
                while (await timer.WaitForNextTickAsync())
                {
                    time = System.DateTime.Now;
                    if (time.Hour != TimeOfDayRefreshData)
                    {
                        continue;
                    }

                    handleData();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }

        public void handleData() 
        {
            try
            {
                //getData
                var TournamentData = CallApiAsync();
                if(TournamentData?.Result?.data?.tournaments?.nodes == null)
                {
                    return;
                }

                //setData
                SetDataToSystem(TournamentData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }

        private void SetDataToSystem(Task<Root?> tournamentData)
        {
            try
            {
                foreach(var tournament in tournamentData.Result.data.tournaments.nodes)
                {
                    var systemTourney = new Tournament()
                    {
                        ID = tournament.id,
                        Name = tournament.name,
                        CountryCode = tournament.countryCode,
                        City = tournament.city,
                        AddrState = tournament.addrState,
                        StartsAT = tournament.startAt == null? null:DateTime.UnixEpoch.AddSeconds(tournament.startAt),
                        Online = tournament.isOnline,
                        URL = tournament.url,
                        State = tournament.state,
                        VenueAddress = tournament.venueAddress,
                        VenueName = tournament.venueName,
                        RegistrationOpen = tournament.isRegistrationOpen,
                        NumberOfAttendees = tournament.numAttendees,
                        Game = tournament.events.Select(x=>x.videogame?.name)?.FirstOrDefault(),
                        Streams = tournament.streams?.Select(x=> "https://www.twitch.tv/" + x.streamName)?.ToList(),
                        TournamentHostSite = GeneralData.TournamentSiteHost.Start.AsString(EnumFormat.Description),
                    };

                    GeneralData.addTournament(systemTourney);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }

        public async Task<Root?> CallApiAsync()
        {
            try
            {
                Console.WriteLine("Calling Api: " + System.DateTime.Now);

                var responseData = await ConnectAndGetData_StartGG();
                if (String.IsNullOrEmpty(responseData))
                {
                    return null;
                }

                Root startGGData = JsonConvert.DeserializeObject<Root>(responseData);
                var noEvents = startGGData.data.tournaments.nodes.Where(x => x.events.Count == 0).ToList();
                if(noEvents !=null)
                {
                    foreach(var item in noEvents)
                    {
                        startGGData.data.tournaments.nodes.Remove(item);
                    }
                    
                }

                return startGGData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
            return null;
        }

        private async Task<string> ConnectAndGetData_StartGG()
        {
            try
            {
                var json = new StartGGConnectionData.StartGGJsonFormatter();

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(StartGGConnectionData.StartGGEndpoint);

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", StartGGConnectionData.StartGGToken);

                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        Content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json")
                    };

                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        if (response?.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine("Invalid Status after startGG API call: " + response?.StatusCode);
                            return string.Empty;
                        }

                        var responseString = await response.Content.ReadAsStringAsync();
                        if (String.IsNullOrEmpty(responseString) ||
                            responseString.Contains("error"))
                        {
                            Console.WriteLine("Invalid Response after startGG API call");
                            return string.Empty;
                        }

                        return responseString;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
            return string.Empty;
        }


        

    }
}
