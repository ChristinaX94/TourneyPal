﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourneyPal.DataHandling.StartGGHelper
{
    public static class StartGGConnectionData
    {
        /// <summary>
        /// API Endpoint of start.gg  
        /// </summary>
        public const string StartGGEndpoint = "https://api.start.gg/gql/alpha";

        /// <summary>
        /// Auth Token of start.gg  
        /// </summary>
        public static readonly string StartGGToken = System.Configuration.ConfigurationManager.AppSettings["StartGGToken"]!;

        /// <summary>
        /// Query for start.gg  
        /// </summary>
        public const string StartGGQuery = "query TournamentsByDate($perPage: Int!, $startAt: Timestamp!, $endAt: Timestamp!, $videogameIDs: [ID!]) " +
                                                "{tournaments(    query: {perPage: $perPage, sortBy: \"startAt asc\", filter: {afterDate: $startAt, beforeDate: $endAt, videogameIds: $videogameIDs}}) " +
                                                    "{nodes {id name countryCode addrState city startAt isOnline url state venueAddress venueName isRegistrationOpen numAttendees events(filter: {videogameId: $videogameIDs}) { name videogame { name } } streams{streamName} } } }";

        
        /// <summary>
        /// Variables for start.gg  
        /// </summary>
        public static string getStartGGVariables()
        {
            return "{\"perPage\": 500,\"startAt\": "+ getUnixTimeStamp(DateTime.Now) + ",\"endAt\": "+ getUnixTimeStamp(DateTime.Now.AddYears(1)) + ",\"videogameIDs\": [904]}";
        }

        public static long getUnixTimeStamp(DateTime date)
        {
            return ((DateTimeOffset)date).ToUnixTimeSeconds();
        }

        public class StartGGJsonFormatter
        {
            public string query { get; set; }
            public string operationName { get; set; }
            public string variables { get; set; }

            public StartGGJsonFormatter()
            {
                query = StartGGQuery;
                operationName = null;
                variables = getStartGGVariables();
            }

            //public StartGGJsonFormatter()
            //{
            //    query = "query TournamentsByDate($perPage: Int!, $startAt: Timestamp!, $endAt: Timestamp!, $videogameIDs: [ID!]) {tournaments(    query: {perPage: $perPage, sortBy: \"startAt asc\", filter: {afterDate: $startAt, beforeDate: $endAt, videogameIds: $videogameIDs}}) {nodes {id name countryCode addrState city startAt isOnline url state venueAddress venueName isRegistrationOpen numAttendees events(filter: {videogameId: $videogameIDs}) { name videogame { name } } streams{streamName} } } }";
            //    operationName = null;
            //    variables = "{\"perPage\": 500,\"startAt\": 1704123544,\"endAt\": 1735738744,\"videogameIDs\": [904]}";
            //}
        }

    }
}
