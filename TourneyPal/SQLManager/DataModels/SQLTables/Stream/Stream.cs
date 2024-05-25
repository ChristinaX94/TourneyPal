﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Stream
{
    public class Stream : Model
    {
        public override Result load(MySqlDataReader reader)
        {
            Result result = new Result();
            try
            {
                result = this.initializeRows();
                if (!result.success)
                {
                    return result;
                }

                while (reader.Read())
                {
                    StreamRow row = new StreamRow(GetType().Name);
                    result = row.loadRow(reader);
                    if (!result.success)
                    {
                        return result;
                    }

                    rows.Add(row);
                }
                result.success = true;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;

        }
    }
}