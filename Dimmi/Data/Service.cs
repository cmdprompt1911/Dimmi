using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dimmi.Interfaces.ServiceService;
using Dimmi.Interfaces.ImageService;

namespace Dimmi.Data
{
    public static class Service
    {
        public static List<ServiceItem> GetServicesByName(string searchPattern, int userId)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            List<ServiceItem> results = new List<ServiceItem>();
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetServicesByName", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", searchPattern);
                cmd.Parameters.AddWithValue("@userId", userId);

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ServiceItem l = new ServiceItem();
                    l.id = dr.GetInt32(0);
                    l.name = dr.GetValue(1).ToString();
                    l.description = dr.GetValue(2).ToString();
                    l.code = dr.GetValue(3).ToString();
                    l.codeType = dr.GetValue(4).ToString();
                    l.issuerCountryCode = dr.GetValue(5).ToString();
                    l.issuerCountry = dr.GetValue(6).ToString();
                    
                    //l.manufacturerid = dr.GetValue(7).ToString();
                    //l.modelNum = dr.GetValue(8).ToString();
                    l.status = "success";
                    int cId = -1;
                    int.TryParse(dr.GetValue(7).ToString(), out cId);
                    if (cId > 0)
                        l.companyId = cId;
                    l.companyName = dr.GetValue(8).ToString();
                    l.numReviews = dr.GetInt32(9);
                    l.compositRating = dr.GetDouble(10);
                    l.hasReviewed = Convert.ToBoolean(dr.GetInt32(11));
                    l.hasReviewedId = dr.GetInt32(12);
                    results.Add(l);
                }
                return results;
            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("GetServicesByName", e);
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

        public static string AddServiceImage(byte[] image, int fileType, int serviceId)
        {
            SqlConnection conn = null;
            string newIdent = "";
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("AddServiceImage", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@image", image);
                cmd.Parameters.AddWithValue("@serviceId", serviceId);
                cmd.Parameters.AddWithValue("@fileType", fileType);
                SqlParameter outval = new SqlParameter();
                outval.ParameterName = "@uid";
                outval.Direction = ParameterDirection.Output;
                outval.SqlDbType = SqlDbType.UniqueIdentifier;
                outval.Value = newIdent;
                
                cmd.Parameters.Add(outval);
                cmd.ExecuteNonQuery();
                newIdent = outval.Value.ToString();
                return newIdent;

            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("AddServiceImage", e);
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

        public static List<ImageItem> GetServiceImages(int serviceId)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            List<ImageItem> results = new List<ImageItem>();
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetServiceImages", conn);
                cmd.Parameters.AddWithValue("@serviceId", serviceId);
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
                Dimmi.Data.Log.WriteDataToLog("GetServiceImages", e);
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