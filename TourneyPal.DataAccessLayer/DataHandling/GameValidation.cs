using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;
using static TourneyPal.Common;

namespace TourneyPal.DataAccessLayer.DataHandling
{
    public static class GameValidation
    {
        public static bool gameIsSoulCalibur6(string game)
        {
            try
            {
                if (string.IsNullOrEmpty(game))
                {
                    return false;
                }

                game = game.ToLower().Replace(" ", "").Trim();

                if (game.Contains("soulcalibur") &&
                    (game.Contains("6") || game.Contains("vi")))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return false;
        }

        public static bool gameIsSoulCalibur2(string game)
        {
            try
            {
                if (string.IsNullOrEmpty(game))
                {
                    return false;
                }

                game = game.ToLower().Replace(" ", "").Trim();

                if (game.Contains("soulcalibur") &&
                    (game.Contains("2") || game.Contains("ii")))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return false;
        }

        public static bool gameIsTekken8(string game)
        {
            try
            {
                if (string.IsNullOrEmpty(game))
                {
                    return false;
                }

                game = game.ToLower().Replace(" ", "").Trim();

                if (game.Contains("tekken") && game.Contains("8"))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return false;
        }

        public static bool gameIsStreetFighter6(string game)
        {
            try
            {
                if (string.IsNullOrEmpty(game))
                {
                    return false;
                }

                game = game.ToLower().Replace(" ", "").Trim();

                if (game.Contains("streetfighter") &&
                    (game.Contains("6") || game.Contains("vi")))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return false;
        }

        public static Games? getGameType(string game)
        {
            try
            {
                if (string.IsNullOrEmpty(game))
                {
                    return null;
                }

                if (gameIsSoulCalibur6(game))
                {
                    return Games.SoulCalibur6;
                }

                if (gameIsSoulCalibur2(game))
                {
                    return Games.SoulCalibur2;
                }

                if (gameIsTekken8(game))
                {
                    return Games.Tekken8;
                }

                if (gameIsStreetFighter6(game))
                {
                    return Games.StreetFighter6;
                }
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return null;
        }
    }
}
