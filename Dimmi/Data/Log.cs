using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace Dimmi.Data
{
    public static class Log
    {

        public static void WriteDataToLog(string caller, Exception e)
        {
            String combinedM = caller + ":  MESSAGE:" + e.Message + " STACKTRACE: " + e.StackTrace;
            if (e.InnerException != null)
            {
                combinedM += " INNER MESSAGE:" + e.InnerException.Message + " INNER STACKTRACE: " + e.InnerException.StackTrace;
            }
            InternalWriteDataToLog(combinedM);
        }
        public static void WriteDataToLog(string caller, string message)
        {
            String combinedM = caller + ":  MESSAGE:" + message;
            InternalWriteDataToLog(combinedM);
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