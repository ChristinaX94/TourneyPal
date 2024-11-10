﻿using System.ComponentModel;

namespace TourneyPal.Commons
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

        public static readonly int TourneyDataPageRows = 5;

        public static readonly string TourneyPalRole = "TourneyPalAnnouncements";

        public static DateTime getDate()
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

        public enum Game
        {
            [Description("Soul Calibur VI - SCVI")]
            SoulCalibur6 = 1,
            [Description("Tekken 8 - T8")]
            Tekken8 = 2,
            [Description("Street Fighter 6 - SF6")]
            StreetFighter6 = 3,
            [Description("Soul Calibur II - SCII")]
            SoulCalibur2 = 4
        }
    }
}
