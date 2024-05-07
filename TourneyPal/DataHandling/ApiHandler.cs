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
using TourneyPal.DataHandling.DataObjects;
using EnumsNET;
using TourneyPal.DataHandling.ChallongeHelper;
using System.Net;

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
            //TODO: delete
            //handleData("1gthtrfs");

            //updateDB
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
                var TournamentData = CallStartGGApiAsync();
                if (TournamentData?.Result?.data?.tournaments?.nodes == null)
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

        private void SetDataToSystem(Task<StartGGJsonObject.Root?> tournamentData)
        {
            try
            {
                foreach (var tournament in tournamentData.Result.data.tournaments.nodes)
                {
                    var systemTourney = new TournamentData()
                    {
                        ID = tournament.id,
                        Name = tournament.name,
                        CountryCode = tournament.countryCode,
                        City = tournament.city,
                        AddrState = tournament.addrState,
                        StartsAT = tournament.startAt == null ? null : DateTime.UnixEpoch.AddSeconds(tournament.startAt),
                        Online = tournament.isOnline,
                        URL = tournament.url,
                        State = tournament.state,
                        VenueAddress = tournament.venueAddress,
                        VenueName = tournament.venueName,
                        RegistrationOpen = tournament.isRegistrationOpen,
                        NumberOfAttendees = tournament.numAttendees,
                        Game = tournament.events.Select(x => x.videogame?.name)?.FirstOrDefault(),
                        Streams = tournament.streams?.Select(x => "https://www.twitch.tv/" + x.streamName)?.ToList(),
                        TournamentHostSite = "https://www.start.gg/",
                    };

                    DataObjects.GeneralData.addTournament(systemTourney);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }

        public async Task<StartGGJsonObject.Root?> CallStartGGApiAsync()
        {
            try
            {
                Console.WriteLine("Calling Api: " + System.DateTime.Now);

                var responseData = await ConnectAndGetData_StartGG();
                if (String.IsNullOrEmpty(responseData))
                {
                    return null;
                }

                StartGGJsonObject.Root startGGData = JsonConvert.DeserializeObject<StartGGJsonObject.Root>(responseData);
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

        public void handleData(string tournamentID)
        {
            try
            {
                //getData
                var TournamentData = CallChallongeApiAsync(tournamentID);
                if (TournamentData?.Result?.tournament == null)
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

        private void SetDataToSystem(Task<ChallongeJsonObject.Root?> tournamentData)
        {
            try
            {
                var tournament =tournamentData?.Result?.tournament;

                var systemTourney = new TournamentData()
                {
                    ID = tournament.id,
                    Name = tournament.name,
                    CountryCode = string.Empty,
                    City = string.Empty,
                    AddrState = string.Empty,
                    StartsAT = tournament.start_at == null ? null : (DateTime)tournament.start_at,
                    Online = null,
                    URL = tournament.url,
                    State = null,
                    VenueAddress = string.Empty,
                    VenueName = string.Empty,
                    RegistrationOpen = tournament.open_signup,
                    NumberOfAttendees = tournament.participants_count,
                    Game = tournament.game_name,
                    Streams = null,
                    TournamentHostSite = "https://challonge.com/",
                };

                if(systemTourney.Game.ToLower().Remove(' ').Contains("soulcalibur"))
                {
                    DataObjects.GeneralData.addTournament(systemTourney);
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }

        public async Task<ChallongeJsonObject.Root?> CallChallongeApiAsync(string tournamentID)
        {
            try
            {
                Console.WriteLine("Calling Challonge Api: " + System.DateTime.Now);

                var responseData = await ConnectAndGetData_Challonge(tournamentID);
                if (String.IsNullOrEmpty(responseData))
                {
                    return null;
                }

                ChallongeJsonObject.Root challongeGGData = JsonConvert.DeserializeObject<ChallongeJsonObject.Root>(responseData);

                return challongeGGData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
            return null;
        }

        private async Task<string> ConnectAndGetData_Challonge(string tournamentID)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var uri = ChallongeConnectionData.getFullChallongeApiRequestURI(tournamentID);
                    using (var response = await client.GetAsync(uri))
                    {
                        response.EnsureSuccessStatusCode();
                        if (response?.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            Console.WriteLine("Invalid Status after Challonge API call: " + response?.StatusCode);
                            return string.Empty;
                        }

                        var responseString = await response.Content.ReadAsStringAsync();
                        if (String.IsNullOrEmpty(responseString) ||
                            responseString.Contains("error"))
                        {
                            Console.WriteLine("Invalid Response after Challonge API call");
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
