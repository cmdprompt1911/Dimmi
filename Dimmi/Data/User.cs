using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace Dimmi.Data
{
    public class User
    {
        public static DataTable GetUser(string emailAddress)
        {
            SqlConnection conn = null;
            
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("GetUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@email", emailAddress);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ret = new DataSet();
                da.Fill(ret);
                return ret.Tables[0];
                
            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("GetUser", e);
                return null;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            
        }
        public static void UpdateUserLoginTimeStamp(string emailAddress)
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();

                SqlCommand cmd = new SqlCommand("UpdateUserLastLoginTimeStamp", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@emailAddress", emailAddress);
                cmd.Parameters.AddWithValue("@timeStamp", DateTime.UtcNow);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("UpdateUserLoginTimeStamp", e);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        public static DataTable UpdateFBUser(string emailAddress, string locale, string firstName, string lastName, int timezoneFromUTC, string name, string gender, string location, string fBUserName, string fBLink)
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("UpdateFBUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@emailAddress", emailAddress);
                cmd.Parameters.AddWithValue("@local", locale);
                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);
                cmd.Parameters.AddWithValue("@timezoneFromUTC", timezoneFromUTC);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@location", location);
                cmd.Parameters.AddWithValue("@fBUserName", fBUserName);
                cmd.Parameters.AddWithValue("@fBLink", fBLink);
                cmd.ExecuteNonQuery();



                return GetUser(emailAddress);

            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("UpdateFBUser", e);
                return null;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public static DataTable CreateFBUser(string emailAddress, string locale, string firstName, string lastName, int timezoneFromUTC, string name, string gender, string location, string fBUserName, string fBLink)
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("AddFBUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@emailAddress", emailAddress);
                cmd.Parameters.AddWithValue("@lastlogin", DateTime.Now);
                cmd.Parameters.AddWithValue("@local", locale);
                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);
                cmd.Parameters.AddWithValue("@timezoneFromUTC", timezoneFromUTC);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@location", location);
                cmd.Parameters.AddWithValue("@fBUserName", fBUserName);
                cmd.Parameters.AddWithValue("@fBLink", fBLink);
                cmd.ExecuteNonQuery();
                return GetUser(emailAddress);

            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("CreateFBUser", e);
                return null;
            }
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