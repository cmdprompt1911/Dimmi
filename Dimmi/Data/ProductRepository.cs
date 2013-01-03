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
    public class ProductRepository : IProductRepository
    {
        public IEnumerable<Product> GetByName(string name, int userId)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            List<Product> results = new List<Product>();
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetProductsByName", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@userId", userId);

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Product l = new Product();
                    l.id = dr.GetInt32(0);
                    l.name = dr.GetValue(1).ToString();
                    l.description = dr.GetValue(2).ToString();
                    l.code = dr.GetValue(3).ToString();
                    l.codeType = dr.GetValue(4).ToString();
                    l.issuerCountry = dr.GetValue(6).ToString();
                    l.issuerCountryCode = dr.GetValue(5).ToString();
                    l.manufacturerid = dr.GetValue(7).ToString();
                    l.modelNum = dr.GetValue(8).ToString();
                    l.status = "success";
                    int cId = -1;
                    int.TryParse(dr.GetValue(9).ToString(), out cId);
                    if (cId > 0)
                        l.companyId = cId;
                    l.companyName = dr.GetValue(10).ToString();
                    l.numReviews = dr.GetInt32(11);
                    l.compositRating = dr.GetDouble(12);
                    l.hasReviewed = Convert.ToBoolean(dr.GetInt32(13));
                    l.hasReviewedId = dr.GetInt32(14);
                    Object data = dr.GetValue(15);
                    if (data != DBNull.Value)
                    {
                        l.image = Convert.ToBase64String((byte[])data);
                        l.imageFileType = dr.GetValue(16).ToString();
                    }

                    results.Add(l);
                }
                return results;
            }
            catch (Exception e)
            {
                //Dimmi.Data.Log.WriteDataToLog("LookupProductInDBByName", e);
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

        public Product Get(int productid, int userId)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            //List<Product> results = new List<Product>();
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetProductsById", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@productId", productid);
                cmd.Parameters.AddWithValue("@userId", userId);

                dr = cmd.ExecuteReader();
                Product l = new Product();
                while (dr.Read())
                {
                   
                    l.id = dr.GetInt32(0);
                    l.name = dr.GetValue(1).ToString();
                    l.description = dr.GetValue(2).ToString();
                    l.code = dr.GetValue(3).ToString();
                    l.codeType = dr.GetValue(4).ToString();
                    l.issuerCountry = dr.GetValue(6).ToString();
                    l.issuerCountryCode = dr.GetValue(5).ToString();
                    l.manufacturerid = dr.GetValue(7).ToString();
                    l.modelNum = dr.GetValue(8).ToString();
                    l.status = "success";
                    int cId = -1;
                    int.TryParse(dr.GetValue(9).ToString(), out cId);
                    if (cId > 0)
                        l.companyId = cId;
                    l.companyName = dr.GetValue(10).ToString();
                    l.numReviews = dr.GetInt32(11);
                    l.compositRating = dr.GetDouble(12);
                    l.hasReviewed = Convert.ToBoolean(dr.GetInt32(13));
                    l.hasReviewedId = dr.GetInt32(14);
                    Object data = dr.GetValue(15);
                    if (data != DBNull.Value)
                    {
                        l.image = Convert.ToBase64String((byte[])data);
                        l.imageFileType = dr.GetValue(16).ToString();
                    }
                    
                }
                return l;
            }
            catch (Exception e)
            {
                // Dimmi.Data.Log.WriteDataToLog("LookupProductInDBById", e);
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


        public Product Add(Product product)
        {
            SqlConnection conn = null;
            int newIdent = -1;
            try
            {

                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("AddProduct", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@code", DataUtil.CheckForEmptyStringVal(product.code)));
                cmd.Parameters.Add(new SqlParameter("@Name", product.name.Trim()));
                cmd.Parameters.Add(new SqlParameter("@Description", DataUtil.CheckForEmptyStringVal(product.description)));
                switch (product.code.Trim().Length)
                {
                    case 12:
                        cmd.Parameters.Add(new SqlParameter("@CodeType", 1)); //UPC
                        break;
                    case 13:
                        cmd.Parameters.Add(new SqlParameter("@CodeType", 3)); //EAN
                        break;
                    default:
                        cmd.Parameters.Add(new SqlParameter("@CodeType", 4));//none
                        break;
                }
                cmd.Parameters.Add(new SqlParameter("@CountryCode", product.issuerCountryCode));
                cmd.Parameters.Add(new SqlParameter("@ManufacturerId", DataUtil.CheckForEmptyStringVal(product.manufacturerid)));
                cmd.Parameters.Add(new SqlParameter("@ModelNum", DataUtil.CheckForEmptyStringVal(product.modelNum)));
                SqlParameter outval = new SqlParameter();
                outval.ParameterName = "@id";
                outval.Direction = ParameterDirection.Output;
                outval.Value = newIdent;
                cmd.Parameters.Add(outval);
                cmd.ExecuteNonQuery();
                newIdent = (int)outval.Value;


                return Get(newIdent, 0);
            }
            catch (Exception e)
            {
                //Dimmi.Data.Log.WriteDataToLog("AddProductInDB", e);
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

        public Image AddImage(Image image, int productId)
        {
            SqlConnection conn = null;
            string newIdent = "";
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("AddProductImage", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@image", image.data);
                cmd.Parameters.AddWithValue("@productId", productId);
                cmd.Parameters.AddWithValue("@fileType", image.fileType);
                SqlParameter outval = new SqlParameter();
                outval.ParameterName = "@uid";
                outval.Direction = ParameterDirection.Output;
                outval.SqlDbType = SqlDbType.UniqueIdentifier;
                outval.Value = newIdent;

                cmd.Parameters.Add(outval);
                cmd.ExecuteNonQuery();
                newIdent = outval.Value.ToString();
                //cmd.ExecuteNonQuery();

                return new Image() ;

            }
            catch (Exception e)
            {
                //Dimmi.Data.Log.WriteDataToLog("AddProductImage", e);
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

        public IEnumerable<Image> GetImages(int productId)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            List<Image> results = new List<Image>();
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetProductImages", conn);
                cmd.Parameters.AddWithValue("@productId", productId);
                cmd.CommandType = CommandType.StoredProcedure;

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Image l = new Image();
                    l.uid = dr.GetGuid(0).ToString();

                    l.data = Convert.ToBase64String(((byte[])dr.GetValue(1))); 
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
                //Dimmi.Data.Log.WriteDataToLog("GetProductImages", e);
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