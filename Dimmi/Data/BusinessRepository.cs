using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dimmi.DataInterfaces;
using Dimmi.Models;
using System.Data.SqlClient;
using System.Data;

namespace Dimmi.Data
{
    public class BusinessRepository : IBusinessRepository
    {
        public IEnumerable<Business> Get()
        {
            
            SqlConnection conn = null;
            SqlDataReader dr = null;
            List<Business> results = new List<Business>();
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetCompanies", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Business l = new Business();
                    l.id = dr.GetInt32(0);
                    l.name = dr.GetValue(1).ToString();

                    results.Add(l);
                }
                return results;
            }
            catch (Exception e)
            {
                WriteToLog("Get", e);
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

        public Business Get(int companyId)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            Business result = new Business();
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetCompany", conn);
                cmd.Parameters.AddWithValue("@companyId", companyId);
                cmd.CommandType = CommandType.StoredProcedure;

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    
                    result.id = dr.GetInt32(0);
                    result.name = dr.GetValue(1).ToString();
                }
                return result;
                
            }
            catch (Exception e)
            {
                WriteToLog("Get(companyid)", e);
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

        public Business Add(Business business)
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
                cmd.Parameters.AddWithValue("@name", business.name);
                outval.ParameterName = "@id";
                outval.Direction = ParameterDirection.Output;
                outval.Value = newIdent;
                cmd.Parameters.Add(outval);
                cmd.ExecuteNonQuery();
                newIdent = (int)outval.Value;
                cmd.ExecuteNonQuery();
                return Get(newIdent);

            }
            catch (Exception e)
            {
                this.WriteToLog("Add", e);
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

        public Image AddImage(Image image, int companyId)
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
                cmd.Parameters.AddWithValue("@fileType", image.fileType);
                cmd.Parameters.AddWithValue("@imageType", image.type);
                SqlParameter outval = new SqlParameter();
                outval.ParameterName = "@uid";
                outval.Direction = ParameterDirection.Output;
                outval.Value = newIdent;
                cmd.Parameters.Add(outval);
                cmd.ExecuteNonQuery();
                newIdent = (string)outval.Value;
                cmd.ExecuteNonQuery();
                foreach (Image i in GetImages(companyId))
                {
                    if (i.type == image.type)
                    {
                        image = i;
                        break; ;
                    }
                }

                return image;

            }
            catch (Exception e)
            {
                WriteToLog("AddCompanyImage", e);
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

        public IEnumerable<Image> GetImages(int companyId)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            List<Image> results = new List<Image>();
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
                    Image l = new Image();
                    l.uid = dr.GetGuid(0).ToString();

                    Object data = dr.GetValue(1);
                    l.data = Convert.ToBase64String((byte[])data);

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
                WriteToLog("GetCompanyImages", e);
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

        private void WriteToLog(string from, Exception e)
        {
            ILogRepository logRepository = new LogRepository();
            Log log = new Log();
            log.ex = e;
            logRepository.WriteDataToLog(log);
        }
    }
}