using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dimmi.Interfaces.ProductReviewService;


namespace Dimmi.Data
{
    public static class ProductReview
    {
        public static int AddProductReviewInDB(int owner, int productId, string comment, double rating)
        {
            SqlConnection conn = null;
            int newIdent = -1;
            try
            {
                //Log.WriteDataToLog("AddProductReviewInDB", "Values IN:  [owner]: " + owner.ToString() + " [productid]: " + productId.ToString() + " [comment]: " + comment + " [rating]: " + rating.ToString());

                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                //Log.WriteDataToLog("AddProductReviewInDB", "connection opened.");

                SqlCommand cmd = new SqlCommand("AddProductReview", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@owner", owner));
                cmd.Parameters.Add(new SqlParameter("@productId", productId));
                cmd.Parameters.Add(new SqlParameter("@comment", comment.Trim()));

                cmd.Parameters.Add(new SqlParameter("@rating", rating));
                //Log.WriteDataToLog("AddProductReviewInDB", "parameters added to cmd object.");
                SqlParameter outval = new SqlParameter();
                outval.ParameterName = "@id";
                outval.Direction = ParameterDirection.Output;
                outval.Value = newIdent;
                cmd.Parameters.Add(outval);
                //Log.WriteDataToLog("AddProductReviewInDB", "parameters added to cmd object.");
                cmd.ExecuteNonQuery();
                //Log.WriteDataToLog("AddProductReviewInDB", "query executed.");
                newIdent = (int)outval.Value;
                //Log.WriteDataToLog("AddProductReviewInDB", "retrieve new id.");

                return newIdent;
            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("AddProductReviewInDB", e);
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
        public static int AddProductReviewInstanceInDB(int productId, int reviewId, string comment, double rating)
        {
            SqlConnection conn = null;
            int newIdent = -1;
            try
            {
                
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("AddProductReviewInstance", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.Parameters.Add(new SqlParameter("@productId", productId));
                cmd.Parameters.Add(new SqlParameter("@reviewId", reviewId));
                cmd.Parameters.Add(new SqlParameter("@comment", comment.Trim()));

                cmd.Parameters.Add(new SqlParameter("@rating", rating));
                SqlParameter outval = new SqlParameter();
                outval.ParameterName = "@id";
                outval.Direction = ParameterDirection.Output;
                outval.Value = newIdent;
                cmd.Parameters.Add(outval);
                cmd.ExecuteNonQuery();
                newIdent = (int)outval.Value;
                return newIdent;
            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("AddProductReviewInstanceInDB", e);
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
        public static void UpdateProductReviewInstance(int reviewInstanceId, string comment, double rating)
        {
            SqlConnection conn = null;
            try
            {

                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();


                SqlCommand cmd = new SqlCommand("UpdateProductReviewInstance", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@reviewInstanceid", reviewInstanceId));
                cmd.Parameters.Add(new SqlParameter("@comment", comment.Trim()));
                cmd.Parameters.Add(new SqlParameter("@rating", rating));
                
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("UpdateProductReviewInstance", e);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// Use this method to determine if a user has already reviewed a specific product.  
        /// </summary>
        /// <param name="ownerid"></param>
        /// <param name="productId"></param>
        /// <returns>review id > 0 if yes,  -1 if no.</returns>
        public static int GetProductReviewIdByProductByOwner(int ownerid, int productId)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            int reviewIdOut = -1;
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetProductReviewByOwnerByProductId", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@owner", ownerid);
                cmd.Parameters.AddWithValue("@productId", productId);

                dr = cmd.ExecuteReader();
               
                while (dr.Read())
                {
                    reviewIdOut = dr.GetInt32(0);
                }
                return reviewIdOut;
            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("GetProductReviewIdByProductByOwner", e);
                return reviewIdOut;
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

        public static List<Lookup> GetProductReviewsByOwner(int ownerid)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            List<Lookup> results = new List<Lookup>();
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetProductReviewsByOwner", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@owner", ownerid);

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Lookup l = new Lookup();
                    l.reviewId = dr.GetInt32(0);
                    l.reviewInstanceId = dr.GetInt32(1);
                    l.owner = dr.GetInt32(2);
                    l.productId = dr.GetInt32(3);
                    l.productName = dr.GetValue(4).ToString();
                    l.productDescription = dr.GetValue(5).ToString();
                    l.created = dr.GetDateTime(6);
                    l.lastUpdated = dr.GetDateTime(7);
                    l.comment = dr.GetValue(8).ToString();
                    l.rating = Convert.ToInt32(dr.GetByte(9));
                    l.ownerName = dr.GetString(10);
                    l.latestReviewInstanceId = dr.GetInt32(11);
                    object x = dr.GetValue(12);
                    if (x != null)
                    {
                        l.image = (byte[])dr.GetValue(12);
                        l.imageFileType = dr.GetValue(13).ToString();
                    }
                    

                    results.Add(l);
                }
                return results;
            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("GetProductReviewsByOwner", e);
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

        public static List<Lookup> GetProductReviewsByProduct(int productId)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            List<Lookup> results = new List<Lookup>();
            try
            {

                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetProductReviewsByProduct", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@productid", productId);

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Lookup l = new Lookup();
                    l.reviewId = dr.GetInt32(0);
                    l.reviewInstanceId = dr.GetInt32(1);
                    l.owner = dr.GetInt32(2);
                    l.productId = dr.GetInt32(3);
                    l.productName = dr.GetString(4);
                    l.productDescription = dr.GetString(5);
                    l.created = dr.GetDateTime(6);
                    l.lastUpdated = dr.GetDateTime(7);
                    l.comment = dr.GetString(8);
                    l.rating = (double)dr.GetDouble(9);
                    l.ownerName = dr.GetString(10);
                    l.latestReviewInstanceId = dr.GetInt32(11);
                    object x = dr.GetValue(12);
                    if (x != null)
                    {
                        l.image = (byte[])dr.GetValue(12);
                        l.imageFileType = dr.GetValue(13).ToString();
                    }
                    l.compositeRating = (double)dr.GetDouble(14);
                    

                    results.Add(l);
                }
                return results;
            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("GetProductReviewsByProduct", e);
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

        public static Lookup GetProductReviewByReviewId(int reviewId)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            //List<Lookup> results = new List<Lookup>();
            Lookup lup = new Lookup();
            try
            {
               
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                Log.WriteDataToLog("GetProductReviewByReviewId", "Opened Connection"); 
                SqlCommand cmd = new SqlCommand("GetProductReviewByReviewId", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@reviewid", reviewId);

                dr = cmd.ExecuteReader();
                Log.WriteDataToLog("GetProductReviewByReviewId", "Executed Reader");
                while (dr.Read())
                {
                    Log.WriteDataToLog("GetProductReviewByReviewId", "Reading row");
                    lup.reviewId = dr.GetInt32(0);
                    lup.reviewInstanceId = dr.GetInt32(1);
                    lup.owner = dr.GetInt32(2);
                    lup.productId = dr.GetInt32(3);
                    lup.productName = dr.GetString(4);
                    lup.productDescription = dr.GetString(5);
                    lup.created = dr.GetDateTime(6);
                    lup.lastUpdated = dr.GetDateTime(7);
                    lup.comment = dr.GetString(8);
                    lup.rating = (double)dr.GetDouble(9);
                    lup.ownerName = dr.GetString(10);
                    lup.latestReviewInstanceId = dr.GetInt32(11);
                    object x = dr.GetValue(12);
                    if (x != null)
                    {
                        lup.image = (byte[])dr.GetValue(12);
                        lup.imageFileType = dr.GetValue(13).ToString();
                    }
                    lup.compositeRating = (double)dr.GetDouble(14);
                    

                   
                }
                return lup;
            }
            catch (Exception e)
            {
                Dimmi.Data.Log.WriteDataToLog("GetProductReviewByReviewId", e);
                return lup;
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

    }
}