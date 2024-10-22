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
using System.Reflection;
using TourneyPal.Commons;
using TourneyPal.Data.Commons;

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
            _ = this.examineAllDataAsync();
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

                    _ = this.examineAllDataAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        private async Task examineAllDataAsync()
        {
            try
            {
                await handleStartGGDataAsync();
                await handleChallongeDataASync();

                GeneralData.saveFindings();
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }

        }

        public async Task examineSingleChallongeRequest(string tournament)
        {
            try
            {
                await handleChallongeSingleDataASync(tournament);

                GeneralData.saveFindings();
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }

        }

        public async Task handleStartGGDataAsync() 
        {
            try
            {
                //getData
                var apiResponseData = await ConnectAndGetData_StartGG();
                if (apiResponseData == null ||
                    String.IsNullOrEmpty(apiResponseData.ApiResponse))
                {
                    return;
                }

                var TournamentData = CallStartGGApiAsync(apiResponseData.ApiResponse);
                if (TournamentData?.data?.tournaments?.nodes == null)
                {
                    return;
                }

                //setData
                SetDataToSystem(TournamentData);

                //finalize apiRequestdata
                apiResponseData.Tournaments = TournamentData.data.tournaments.nodes.Select(x => x.id).ToList();
                GeneralData.addApiRequestedData(apiResponseData);

            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        private async Task<ApiRequestedDataHandler?> ConnectAndGetData_StartGG()
        {
            try
            {
                Console.WriteLine("Calling Api: " + System.DateTime.Now);
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
                            return null;
                        }

                        var responseString = await response.Content.ReadAsStringAsync();
                        if (String.IsNullOrEmpty(responseString) ||
                            responseString.Contains("error"))
                        {
                            Console.WriteLine("Invalid Response after startGG API call");
                            return null;
                        }

                        return new ApiRequestedDataHandler
                        {
                            ApiRequestJson = JsonConvert.SerializeObject(json),
                            ApiRequestContent = request.ToString(),
                            ApiResponse = responseString,
                            HostSite = (int)Common.TournamentSiteHost.Start
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return null;
        }

        public StartGGJsonObject.Root? CallStartGGApiAsync(string responseData)
        {
            try
            {
                StartGGJsonObject.Root startGGData = JsonConvert.DeserializeObject<StartGGJsonObject.Root>(responseData);
                var noEvents = startGGData.data.tournaments.nodes.Where(x => x.events.Count == 0).ToList();
                if (noEvents != null)
                {
                    foreach (var item in noEvents)
                    {
                        startGGData.data.tournaments.nodes.Remove(item);
                    }
                }

                return startGGData;
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return null;
        }

        private void SetDataToSystem(StartGGJsonObject.Root? tournamentData)
        {
            try
            {
                foreach (var tournament in tournamentData.data.tournaments.nodes)
                {
                    var systemTourney = new Data.Commons.TournamentData()
                    {
                        ID = tournament.id,
                        Name = tournament.name,
                        CountryCode = tournament.countryCode,
                        City = tournament.city,
                        AddrState = tournament.addrState,
                        StartsAT = DateTime.UnixEpoch.AddSeconds(tournament.startAt),
                        Online = tournament.isOnline,
                        URL = tournament.url,
                        State = tournament.state,
                        VenueAddress = tournament.venueAddress,
                        VenueName = tournament.venueName,
                        RegistrationOpen = tournament.isRegistrationOpen,
                        NumberOfAttendees = tournament.numAttendees==null ? 0: (int)tournament.numAttendees,
                        Game = Common.getGameType(tournament.events.Select(x => x.videogame?.name)?.FirstOrDefault())?.AsString(EnumFormat.Description),
                        Streams = tournament.streams==null? new List<string>() : tournament.streams?.Select(x => "https://www.twitch.tv/" + x.streamName)?.ToList(),
                        HostSite = Common.TournamentSiteHost.Start.AsString(EnumFormat.Description),
                    };

                    GeneralData.addTournament(systemTourney);
                }
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public async Task handleChallongeDataASync()
        {
            try
            {
                foreach (var tournamentURL in DataObjects.GeneralData.TournamentsData
                    .Where(x => x.HostSite.Equals(Common.TournamentSiteHost.Challonge.AsString(EnumFormat.Description)))
                    .ToList()
                    .Select(y=>y.URL))
                {
                    await handleChallongeSingleDataASync(tournamentURL);
                }
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        private async Task handleChallongeSingleDataASync(string? tournamentURL)
        {
            try
            {
                var apiResponseData = await ConnectAndGetData_Challonge(tournamentURL);
                if (apiResponseData == null ||
                    String.IsNullOrEmpty(apiResponseData.ApiResponse))
                {
                    return;
                }


                //getData
                var TournamentData = CallChallongeApiAsync(apiResponseData.ApiResponse);
                if (TournamentData?.tournament == null)
                {
                    return;
                }
                
                if(!Common.gameIsSoulCalibur6(TournamentData.tournament.game_name))
                {
                    return;
                }

                //setData
                SetDataToSystem(TournamentData);

                //finalize apiRequestdata
                apiResponseData.Tournaments = new List<int> { TournamentData.tournament.id };
                GeneralData.addApiRequestedData(apiResponseData);
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            
        }

        private async Task<ApiRequestedDataHandler?> ConnectAndGetData_Challonge(string tournamentID)
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
                            return null;
                        }

                        var responseString = await response.Content.ReadAsStringAsync();
                        if (String.IsNullOrEmpty(responseString) ||
                            responseString.Contains("error"))
                        {
                            Console.WriteLine("Invalid Response after Challonge API call");
                            return null;
                        }

                        return new ApiRequestedDataHandler
                        {
                            ApiRequestJson = uri,
                            ApiRequestContent = string.Empty,
                            ApiResponse = responseString,
                            HostSite = (int)Common.TournamentSiteHost.Challonge
                        };
                    }
                }


            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return null;
        }

        public ChallongeJsonObject.Root? CallChallongeApiAsync(string responseData)
        {
            try
            {
                Console.WriteLine("Calling Challonge Api: " + System.DateTime.Now);

                ChallongeJsonObject.Root challongeGGData = JsonConvert.DeserializeObject<ChallongeJsonObject.Root>(responseData);

                return challongeGGData;
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return null;
        }

        private void SetDataToSystem(ChallongeJsonObject.Root? tournamentData)
        {
            try
            {
                var tournament = tournamentData?.tournament;

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
                    Game = Common.getGameType(tournament.game_name)?.AsString(EnumFormat.Description),
                    Streams = new List<string>(),
                    HostSite = Common.TournamentSiteHost.Challonge.AsString(EnumFormat.Description),
                };

                GeneralData.addTournament(systemTourney);
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }


    }
}
