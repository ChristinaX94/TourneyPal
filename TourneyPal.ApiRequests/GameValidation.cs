using System.Reflection;
using TourneyPal.Commons.DataObjects;
using TourneyPal.Commons.DataObjects.ApiResponseModels;
using static TourneyPal.Common;

namespace TourneyPal.Api
{
    public static class GameValidation
    {
        private static bool gameIsSoulCalibur6(string game)
        {
            return (game.Contains("soulcalibur") || game.Contains("sc")) && (game.Contains("6") || game.Contains("vi"));
        }

        private static bool gameIsSoulCalibur2(string game)
        {
            return (game.Contains("soulcalibur") || game.Contains("sc")) && (game.Contains("2") || game.Contains("ii"));
        }

        private static bool gameIsTekken8(string game)
        {
            return (game.Contains("tekken") || game.Contains("tk")) && (game.Contains("8") || game.Contains("viii"));
        }

        private static bool gameIsStreetFighter6(string game)
        {
            return (game.Contains("streetfighter") || game.Contains("sf")) && (game.Contains("6") || game.Contains("vi"));
        }

        public static ApiGameResponse getGameType(string game)
        {
            var response = new ApiGameResponse();
            try
            {
                if (string.IsNullOrEmpty(game))
                {
                    response.Success = false;
                    return response;
                }

                game = game.ToLower().Replace(" ", "").Trim();

                if (gameIsSoulCalibur6(game))
                {
                    response.Game = Game.SoulCalibur6;
                    response.Success = true;
                    return response;
                }

                if (gameIsSoulCalibur2(game))
                {
                    response.Game = Game.SoulCalibur2;
                    response.Success = true;
                    return response;
                }

                if (gameIsTekken8(game))
                {
                    response.Game = Game.Tekken8;
                    response.Success = true;
                    return response;
                }

                if (gameIsStreetFighter6(game))
                {
                    response.Game = Game.StreetFighter6;
                    response.Success = true;
                    return response;
                }

                response.Success = false;
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
