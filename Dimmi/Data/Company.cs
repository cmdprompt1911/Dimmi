using Dimmi.Interfaces.CompanyService;
using Dimmi.Interfaces.ImageService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Dimmi.Data
{
    public static class Company
    {
        public static List<CompanyItem> GetCompanies()
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            List<CompanyItem> results = new List<CompanyItem>();
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetCompanies", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    CompanyItem l = new CompanyItem();
                    l.id = dr.GetInt32(0);
                    l.name = dr.GetValue(1).ToString();

                    results.Add(l);
                }
                return results;
            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("GetCompanies", e);
                return results;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public static int AddCompany(string name)
        {
            SqlConnection conn = null;
            int newIdent = -1;
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("AddCompany", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter outval = new SqlParameter();
                outval.ParameterName = "@id";
                outval.Direction = ParameterDirection.Output;
                outval.Value = newIdent;
                cmd.Parameters.Add(outval);
                cmd.ExecuteNonQuery();
                newIdent = (int)outval.Value;
                cmd.ExecuteNonQuery();
                return newIdent;

            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("AddCompany", e);
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

        public static string AddCompanyImage(byte[] image, int fileType, int companyId, int imageType)
        {
            SqlConnection conn = null;
            string newIdent = "";
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("AddCompanyImage", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@image", image);
                cmd.Parameters.AddWithValue("@companyId", companyId);
                cmd.Parameters.AddWithValue("@fileType", fileType);
                cmd.Parameters.AddWithValue("@imageType", imageType);
                SqlParameter outval = new SqlParameter();
                outval.ParameterName = "@uid";
                outval.Direction = ParameterDirection.Output;
                outval.Value = newIdent;
                cmd.Parameters.Add(outval);
                cmd.ExecuteNonQuery();
                newIdent = (string)outval.Value;
                cmd.ExecuteNonQuery();
                return newIdent;

            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("AddCompanyImage", e);
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

        public static List<ImageItem> GetCompanyImages(int companyId)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            List<ImageItem> results = new List<ImageItem>();
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetCompanyImages", conn);
                cmd.Parameters.AddWithValue("@companyId", companyId);
                cmd.CommandType = CommandType.StoredProcedure;

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ImageItem l = new ImageItem();
                    l.uid = dr.GetGuid(0).ToString();

                    l.data = (byte[])dr.GetValue(1);
                    l.fileType = dr.GetInt32(2);
                    l.fileTypeName = dr.GetValue(3).ToString();
                    l.name = dr.GetValue(4).ToString();
                    l.type = dr.GetInt32(5);
                    l.dateCreated = dr.GetDateTime(6);

                    results.Add(l);
                }
                return results;
            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("GetCompanyImages", e);
                return results;
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