using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using TourneyPal.DataHandling.StartGGHelper;
using EnumsNET;
using TourneyPal.DataHandling.ChallongeHelper;
using System.Reflection;
using TourneyPal.Commons.DataObjects;
using TourneyPal.Commons.DataObjects.ApiResponseModels;
using TourneyPal.Commons;

namespace TourneyPal.Api
{

    public static class ApiHandler
    {
        public static async Task<ApiResponse> examineAllDataAsync(List<int> StartGGGameIDs, List<string> challongeURLS)
        {
            var response = new ApiResponse();
            try
            {
                foreach(var gameID in StartGGGameIDs)
                {
                    var startGGTournamentsResponse = await handleStartGGDataAsync(gameID);
                    if (!startGGTournamentsResponse.Success)
                    {
                        return startGGTournamentsResponse;
                    }

                    response.Tournaments.AddRange(startGGTournamentsResponse.Tournaments);
                    response.Requests.Add(startGGTournamentsResponse.ApiRequestedData);
                }
                

                var challongeTournamentsResponse = await handleChallongeDataASync(challongeURLS);
                if (!challongeTournamentsResponse.Success)
                {
                    return challongeTournamentsResponse;
                }

                response.Tournaments.AddRange(challongeTournamentsResponse.Tournaments);
                response.Requests.AddRange(challongeTournamentsResponse.Requests);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Error = new LogMessage()
                {
                    ExceptionMessageItem = ex.Message + " -- " + ex.StackTrace,
                    FoundInItem = MethodBase.GetCurrentMethod()
                };
                response.Success = false;
            }
            return response;
        }

        public static async Task<ApiResponse> examineSingleChallongeRequest(string tournament)
        {
            var response = new ApiResponse();
            try
            {
                var challongeTournamentResponse = await handleChallongeSingleDataASync(tournament);
                if (!challongeTournamentResponse.Success)
                {
                    return challongeTournamentResponse;
                }
                response.Tournaments.AddRange(challongeTournamentResponse.Tournaments);
                response.Success= true;
            }
            catch (Exception ex)
            {
                response.Error = new LogMessage()
                {
                    ExceptionMessageItem = ex.Message + " -- " + ex.StackTrace,
                    FoundInItem = MethodBase.GetCurrentMethod()
                };
                response.Success = false;
            }
            return response;
        }

        private static async Task<ApiRequestResponse> handleStartGGDataAsync(int GameID) 
        {
            var response = new ApiRequestResponse();
            try
            {
                //getData
                var apiResponseData = await ConnectAndGetData_StartGG(GameID);
                if (!apiResponseData.Success ||
                    String.IsNullOrEmpty(apiResponseData.ApiRequestedData.ApiResponse))
                {
                    return apiResponseData;
                }

                var tournamentDataRequested = CallStartGGApiAsync(apiResponseData.ApiRequestedData.ApiResponse);
                if (!tournamentDataRequested.Success ||
                    (tournamentDataRequested.CallerData as StartGGJsonObject.Root)?.data?.tournaments?.nodes == null)
                {
                    return tournamentDataRequested;
                }
                
                //setData
                var tournamentDataSet = SetDataToSystem(tournamentDataRequested.CallerData as StartGGJsonObject.Root);
                if (!tournamentDataSet.Success)
                {
                    return tournamentDataSet;
                }

                //finalize data
                response.ApiRequestedData = apiResponseData.ApiRequestedData;
                response.ApiRequestedData.Tournaments = (tournamentDataRequested.CallerData as StartGGJsonObject.Root).data.tournaments.nodes.Select(x => x.id).ToList();
                response.Tournaments = tournamentDataSet.Tournaments;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Error = new LogMessage()
                {
                    ExceptionMessageItem = ex.Message + " -- " + ex.StackTrace,
                    FoundInItem = MethodBase.GetCurrentMethod()
                };
                response.Success = false;
            }
            return response;
        }

        private static async Task<ApiRequestResponse> ConnectAndGetData_StartGG(int GameID)
        {
            var ApiResponse = new ApiRequestResponse();
            try
            {
                LogFile.WriteToLogFile("Calling Api: " + System.DateTime.Now);
                var json = new StartGGConnectionData.StartGGJsonFormatter(GameID);

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
                            ApiResponse.Success = false;
                            ApiResponse.Error = new LogMessage()
                            {
                                MessageItem = "Invalid Status after startGG API call: " + response?.StatusCode,
                                FoundInItem = MethodBase.GetCurrentMethod()
                            };
                            return ApiResponse;
                        }

                        var responseString = await response.Content.ReadAsStringAsync();
                        if (String.IsNullOrEmpty(responseString) ||
                            responseString.Contains("error"))
                        {
                            ApiResponse.Success = false;
                            ApiResponse.Error = new LogMessage()
                            {
                                MessageItem = "Invalid Response after startGG API call: " + responseString,
                                FoundInItem = MethodBase.GetCurrentMethod()
                            };
                            return ApiResponse;
                        }

                        ApiResponse.ApiRequestedData = new ApiRequestedDataHandler
                        {
                            ApiRequestJson = JsonConvert.SerializeObject(json),
                            ApiRequestContent = request.ToString(),
                            ApiResponse = responseString,
                            HostSite = (int)Common.TournamentSiteHost.Start
                        };

                        ApiResponse.Success = true;
                        
                    }
                }
            }
            catch (Exception ex)
            {
                ApiResponse.Error = new LogMessage()
                {
                    ExceptionMessageItem = ex.Message + " -- " + ex.StackTrace,
                    FoundInItem = MethodBase.GetCurrentMethod()
                };
                ApiResponse.Success = false;
            }
            return ApiResponse;
        }

        private static ApiRequestResponse CallStartGGApiAsync(string responseData)
        {
            var response = new ApiRequestResponse();
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
                response.CallerData = startGGData;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Error = new LogMessage()
                {
                    ExceptionMessageItem = ex.Message + " -- " + ex.StackTrace,
                    FoundInItem = MethodBase.GetCurrentMethod()
                };
                response.Success = false;
            }
            return response;
        }

        private static ApiRequestResponse SetDataToSystem(StartGGJsonObject.Root? tournamentData)
        {
            var response = new ApiRequestResponse();
            try
            {
                foreach (var tournament in tournamentData.data.tournaments.nodes)
                {
                    var gameResponse = GameValidation.getGameType(tournament.events.Select(x => x.videogame?.name)?.FirstOrDefault());
                    if (!gameResponse.Success)
                    {
                        response.Error = gameResponse.Error;
                        response.Success = false;
                        return response;
                    }

                    var systemTourney = new TournamentData()
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
                        NumberOfEntrants = tournament.events.Select(x => x.numEntrants)?.FirstOrDefault() ==null ? 0: (int)tournament.events.Select(x => x.numEntrants)?.FirstOrDefault(),
                        GameEnum = gameResponse.Game,
                        Streams = tournament.streams==null? new List<string>() : tournament.streams?.Select(x => x.streamName)?.ToList(),
                        HostSite = Common.TournamentSiteHost.Start.AsString(EnumFormat.Description),
                    };
                    response.Tournaments.Add(systemTourney);
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Error = new LogMessage()
                {
                    ExceptionMessageItem = ex.Message + " -- " + ex.StackTrace,
                    FoundInItem = MethodBase.GetCurrentMethod()
                };
                response.Success = false;
            }
            return response;
        }

        private static async Task<ApiRequestResponse> handleChallongeDataASync(List<string> challongeURLS)
        {
            var response = new ApiRequestResponse();
            try
            {
                foreach(var url in challongeURLS)
                {
                    var singleRequestResponse = await handleChallongeSingleDataASync(url);
                    if (!singleRequestResponse.Success)
                    {
                        return singleRequestResponse;
                    }
                    response.Tournaments.AddRange(singleRequestResponse.Tournaments);
                    response.Requests.Add(singleRequestResponse.ApiRequestedData);
                }
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Error = new LogMessage()
                {
                    ExceptionMessageItem = ex.Message + " -- " + ex.StackTrace,
                    FoundInItem = MethodBase.GetCurrentMethod()
                };
                response.Success = false;
            }
            return response;
        }

        private static async Task<ApiRequestResponse> handleChallongeSingleDataASync(string? tournamentURL)
        {
            var response = new ApiRequestResponse();
            try
            {
                var apiResponseData = await ConnectAndGetData_Challonge(tournamentURL);
                if (!apiResponseData.Success ||
                    String.IsNullOrEmpty(apiResponseData.ApiRequestedData.ApiResponse))
                {
                    return apiResponseData;
                }


                //getData
                var tournamentDataRequested = CallChallongeApiAsync(apiResponseData.ApiRequestedData.ApiResponse);
                if (!tournamentDataRequested.Success ||
                    (tournamentDataRequested.CallerData as ChallongeJsonObject.Root)?.tournament == null)
                {
                    return tournamentDataRequested;
                }

                //setData
                var tournamentDataSet = SetDataToSystem(tournamentDataRequested.CallerData as ChallongeJsonObject.Root);
                if (!tournamentDataSet.Success)
                {
                    return tournamentDataSet;
                }

                //finalize data
                response.ApiRequestedData = apiResponseData.ApiRequestedData;
                response.ApiRequestedData.Tournaments = (tournamentDataRequested.CallerData as StartGGJsonObject.Root).data.tournaments.nodes.Select(x => x.id).ToList();
                response.Tournaments = tournamentDataSet.Tournaments;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Error = new LogMessage()
                {
                    ExceptionMessageItem = ex.Message + " -- " + ex.StackTrace,
                    FoundInItem = MethodBase.GetCurrentMethod()
                };
                response.Success = false;
            }
            return response;
        }

        private static async Task<ApiRequestResponse> ConnectAndGetData_Challonge(string tournamentID)
        {
            var apiResponse = new ApiRequestResponse();
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
                            apiResponse.Success = false;
                            apiResponse.Error = new LogMessage()
                            {
                                MessageItem = "Invalid Status after Challonge API call: " + response?.StatusCode,
                                FoundInItem = MethodBase.GetCurrentMethod()
                            };
                            return apiResponse;
                        }

                        var responseString = await response.Content.ReadAsStringAsync();
                        if (String.IsNullOrEmpty(responseString) ||
                            responseString.Contains("error"))
                        {
                            apiResponse.Success = false;
                            apiResponse.Error = new LogMessage()
                            {
                                MessageItem = "Invalid Response after Challonge API call",
                                FoundInItem = MethodBase.GetCurrentMethod()
                            };
                            return apiResponse;
                        }

                        apiResponse.ApiRequestedData = new ApiRequestedDataHandler
                        {
                            ApiRequestJson = uri,
                            ApiRequestContent = string.Empty,
                            ApiResponse = responseString,
                            HostSite = (int)Common.TournamentSiteHost.Challonge
                        };
                        apiResponse.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                apiResponse.Error = new LogMessage()
                {
                    ExceptionMessageItem = ex.Message + " -- " + ex.StackTrace,
                    FoundInItem = MethodBase.GetCurrentMethod()
                };
                apiResponse.Success = false;
            }
            return apiResponse;
        }

        private static ApiRequestResponse CallChallongeApiAsync(string responseData)
        {
            var response = new ApiRequestResponse();
            try
            {
                LogFile.WriteToLogFile("Calling Challonge Api: " + System.DateTime.Now);

                ChallongeJsonObject.Root challongeGGData = JsonConvert.DeserializeObject<ChallongeJsonObject.Root>(responseData);

                response.CallerData = challongeGGData;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Error = new LogMessage()
                {
                    ExceptionMessageItem = ex.Message + " -- " + ex.StackTrace,
                    FoundInItem = MethodBase.GetCurrentMethod()
                };
                response.Success = false;
            }
            return response;
        }

        private static ApiRequestResponse SetDataToSystem(ChallongeJsonObject.Root? tournamentData)
        {
            var response = new ApiRequestResponse();
            try
            {
                var tournament = tournamentData?.tournament;

                var gameResponse = GameValidation.getGameType(tournament.game_name);
                if (!gameResponse.Success)
                {
                    response.Error = gameResponse.Error;
                    response.Success = false;
                    return response;
                }

                response.Tournaments.Add( new TournamentData()
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
                    NumberOfEntrants = tournament.participants_count,
                    GameEnum = gameResponse.Game,
                    Streams = new List<string>(),
                    HostSite = Common.TournamentSiteHost.Challonge.AsString(EnumFormat.Description),
                });
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Error = new LogMessage()
                {
                    ExceptionMessageItem = ex.Message + " -- " + ex.StackTrace,
                    FoundInItem = MethodBase.GetCurrentMethod()
                };
                response.Success = false;
            }
            return response;
        }
    }
}
