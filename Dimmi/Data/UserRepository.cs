using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dimmi.Models;
using Dimmi.DataInterfaces;



namespace Dimmi.Data
{
    public class UserRepository : IUserRepository
    {
        public User Get(string emailAddress)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;

            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("GetUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@email", emailAddress);
                User l = null;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    l = new User();
                    l.id = dr.GetInt32(0);
                    l.emailAddress = dr.GetString(1);
                    l.lastLogin = dr.GetDateTime(2);
                    l.locale = dr.GetString(3);
                    l.firstName = dr.GetString(4);
                    l.lastName = dr.GetString(5);
                    l.timezoneFromUTC = dr.GetInt32(6);
                    l.name = dr.GetString(7);
                    l.gender = dr.GetString(8);
                    l.location = dr.GetString(9);
                    l.fBUsername = dr.GetString(10);
                    l.fBLink = dr.GetString(11);
                }
                return l;

            }
            catch (Exception e)
            {
                //Dimmi.Data.Log.WriteDataToLog("GetUser", e);
                return null;
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public void UpdateLoginTimeStamp(string emailAddress)
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
                //Dimmi.Data.Log.WriteDataToLog("UpdateUserLoginTimeStamp", e);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public User AddFromFBData(User user)
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("AddFBUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@emailAddress", user.emailAddress);
                cmd.Parameters.AddWithValue("@lastlogin", DateTime.Now);
                cmd.Parameters.AddWithValue("@local", user.locale);
                cmd.Parameters.AddWithValue("@firstName", user.firstName);
                cmd.Parameters.AddWithValue("@lastName", user.lastName);
                cmd.Parameters.AddWithValue("@timezoneFromUTC", user.timezoneFromUTC);
                cmd.Parameters.AddWithValue("@name", user.name);
                cmd.Parameters.AddWithValue("@gender", user.gender);
                cmd.Parameters.AddWithValue("@location", user.location);
                cmd.Parameters.AddWithValue("@fBUserName", user.fBUsername);
                cmd.Parameters.AddWithValue("@fBLink", user.fBLink);
                cmd.ExecuteNonQuery();
                return Get(user.emailAddress);

            }
            catch (Exception e)
            {
                //Dimmi.Data.Log.WriteDataToLog("CreateFBUser", e);
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

        public User UpdateFromFBData(User user)
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("UpdateFBUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@emailAddress", user.emailAddress);
                cmd.Parameters.AddWithValue("@local", user.locale);
                cmd.Parameters.AddWithValue("@firstName", user.firstName);
                cmd.Parameters.AddWithValue("@lastName", user.lastName);
                cmd.Parameters.AddWithValue("@timezoneFromUTC", user.timezoneFromUTC);
                cmd.Parameters.AddWithValue("@name", user.name);
                cmd.Parameters.AddWithValue("@gender", user.gender);
                cmd.Parameters.AddWithValue("@location", user.location);
                cmd.Parameters.AddWithValue("@fBUserName", user.fBUsername);
                cmd.Parameters.AddWithValue("@fBLink", user.fBLink);
                cmd.ExecuteNonQuery();



                return Get(user.emailAddress);

            }
            catch (Exception e)
            {
                //Dimmi.Data.Log.WriteDataToLog("UpdateFBUser", e);
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