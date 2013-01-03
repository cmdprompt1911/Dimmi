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
    public class ImageRepository : IImageRepository
    {
        public Image Get(string uid)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            Image l = null;
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetImage", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@uid", uid);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    l = new Image();
                    l.uid = dr.GetGuid(0).ToString();

                    Object data = dr.GetValue(1);
                    l.data = Convert.ToBase64String((byte[])data);
                    l.fileType = dr.GetInt32(2);
                    l.fileTypeName = dr.GetValue(3).ToString();
                    l.dateCreated = dr.GetDateTime(4);



                }
                return l;
            }
            catch (Exception e)
            {
                //Dimmi.Data.Log.WriteDataToLog("Get1ImageForEachCategory", e);
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

        public IEnumerable<Image> GetCountForEachCategory(int count)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            List<Image> results = new List<Image>();
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetImageForEachCategory", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@count", count);

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
                //Dimmi.Data.Log.WriteDataToLog("Get1ImageForEachCategory", e);
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