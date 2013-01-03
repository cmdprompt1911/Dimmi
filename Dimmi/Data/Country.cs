using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dimmi.Interfaces.ProductService;

namespace Dimmi.Data
{
    public static class Country
    {
        public static int GetCountryCodeId(string countryCodeShortName)
        {
            SqlConnection conn = null;
            int retVal = -1;
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("GetCountryCodeRecordByCode", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@code", countryCodeShortName);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ret = new DataSet();
                da.Fill(ret);
                if (ret.Tables[0].Rows.Count == 1)
                {
                    retVal = (int)ret.Tables[0].Rows[0]["id"];
                }
                return retVal;
            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("GetCountryCodeId", e);
                return retVal;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public static int AddCountryCode(string countryCodeShortName, string countryCodeLongName)
        {
            SqlConnection conn = null;
            int newIdent = -1;
            try
            {
                
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("AddCountryCode", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Code", countryCodeShortName);
                cmd.Parameters.AddWithValue("@FullName", countryCodeLongName);
                SqlParameter outval = new SqlParameter();
                outval.ParameterName = "@id";
                outval.Direction = ParameterDirection.Output;
                outval.Value = newIdent;
                cmd.Parameters.Add(outval);
                cmd.ExecuteNonQuery();
                return newIdent;
            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("AddCountryCode", e);
                return newIdent;
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