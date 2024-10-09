﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Tournament_Host_Sites
{
    public class Tournament_Host_SitesRow : ModelRow
    {
        public Tournament_Host_SitesRow(string? tableName) : base(tableName)
        {
        }

        public string? Site { get; private set; }
        public string? Endpoint { get; private set; }
        public string? Token { get; private set; }
        public string? DataFormat { get; private set; }

        public override bool loadRow(MySqlDataReader reader)
        {
            bool result = false;
            try
            {
                result = base.loadRow(reader);
                if (!result)
                {
                    return result;
                }

                //Site
                if (reader[nameof(Site)] == null ||
                    string.IsNullOrEmpty(reader[nameof(Site)].ToString()))
                {
                    result = false;
                    return result;
                }
                Site = reader[nameof(Site)].ToString();

                //Endpoint
                if (reader[nameof(Endpoint)] == null ||
                    string.IsNullOrEmpty(reader[nameof(Endpoint)].ToString()))
                {
                    result = false;
                    return result;
                }
                Endpoint = reader[nameof(Endpoint)].ToString();

                //Token
                if (reader[nameof(Token)] == null ||
                    string.IsNullOrEmpty(reader[nameof(Token)].ToString()))
                {
                    result = false;
                    return result;
                }
                Token = reader[nameof(Token)].ToString();

                //DataFormat
                DataFormat = reader[nameof(DataFormat)]?.ToString();

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

        public override bool validateRow()
        {
            bool result = false;
            try
            {
                result = base.validateRow();
                if (!result)
                {
                    return result;
                }

                if (string.IsNullOrEmpty(this.Site))
                {
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.Site) + ", of table: " + this.tableName + "-- Cannot be null");
                }

                if (string.IsNullOrEmpty(this.Token))
                {
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.Token) + ", of table: " + this.tableName + "-- Cannot be null");
                }

                if (string.IsNullOrEmpty(this.Endpoint))
                {
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.Endpoint) + ", of table: " + this.tableName + "-- Cannot be null");
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

    }

}
