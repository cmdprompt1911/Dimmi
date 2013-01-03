using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Dimmi.Models;
using System.Data;
using Dimmi.DataInterfaces;


namespace Dimmi.Data
{
    public class LogRepository : ILogRepository
    {
        public void WriteDataToLog(Log log)
        {
            if (log.ex != null)
            {
                String combinedM = log.caller + ":  MESSAGE:" + log.ex.Message + " STACKTRACE: " + log.ex.StackTrace;
                if (log.ex.InnerException != null)
                {
                    combinedM += " INNER MESSAGE:" + log.ex.InnerException.Message + " INNER STACKTRACE: " + log.ex.InnerException.StackTrace;
                }
                InternalWriteDataToLog(combinedM);
            }
            else
            {
                String combinedM = log.caller + ":  MESSAGE:" + log.message;
                InternalWriteDataToLog(combinedM);
            }
        }

        private static void InternalWriteDataToLog(String message)
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("WriteToLog", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@message", message);
                cmd.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }
    }
}