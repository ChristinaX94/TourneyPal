using System.ComponentModel;

namespace TourneyPal
{
    public static class Common
    {

        /// <summary>
        /// 9 am (EET) 
        /// </summary>
        public const int TimeOfDayRefreshData = 9;

        public enum YesNo
        {
            No = 0,
            Yes = 1
        }

        public static readonly int PageButtonTimeoutMinutes = 5;

        public static DateTime? getDate()
        {
            return DateTime.Now;
        }

        public enum TournamentSiteHost
        {
            [Description("https://www.start.gg/")]
            Start = 1,
            [Description("https://challonge.com/")]
            Challonge = 2
        }

        public enum Games
        {
            [Description("Soul Calibur VI")]
            SoulCalibur6 = 1,
            [Description("Tekken 8")]
            Tekken8 = 2,
            [Description("Street Fighter 6")]
            StreetFighter6 = 3,
            [Description("Soul Calibur II")]
            SoulCalibur2 = 4
        }

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
                //Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                //           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
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
                //Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                //           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
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
                //Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                //           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
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
                //Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                //           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
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
                //Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                //           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return null;
        }
    }
}
