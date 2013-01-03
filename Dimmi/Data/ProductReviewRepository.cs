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
    public class ProductReviewRepository : IProductReviewRepository
    {
        public ProductReview Add(ProductReview review)
        {
            SqlConnection conn = null;
            int newIdent = -1;
            try
            {
                //Log.WriteDataToLog("AddProductReviewInDB", "Values IN:  [owner]: " + owner.ToString() + " [productid]: " + productId.ToString() + " [comment]: " + comment + " [rating]: " + rating.ToString());

                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                //Log.WriteDataToLog("AddProductReviewInDB", "connection opened.");
                SqlCommand cmd;
                if (review.reviewId > 0)
                {
                    cmd = new SqlCommand("AddProductReviewInstance", conn);
                    cmd.Parameters.Add(new SqlParameter("@reviewId", review.reviewId));
                }
                else
                {
                    cmd = new SqlCommand("AddProductReview", conn);
                    cmd.Parameters.Add(new SqlParameter("@owner", review.owner));
                }
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@productId", review.productId));
                cmd.Parameters.Add(new SqlParameter("@comment", review.comment.Trim()));

                cmd.Parameters.Add(new SqlParameter("@rating", review.rating));
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

                return Get(newIdent);
            }
            catch (Exception e)
            {
                //Dimmi.Data.Log.WriteDataToLog("AddProductReviewInDB", e);
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

        public IEnumerable<ProductReview> GetByProductId(int productId)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            List<ProductReview> results = new List<ProductReview>();
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
                    ProductReview l = new ProductReview();
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
                        l.image = Convert.ToBase64String((byte[])dr.GetValue(12));
                        l.imageFileType = dr.GetValue(13).ToString();
                    }
                    l.fBFeedPostId = (String)DataUtil.CheckForNullValReturnEmptyString(dr.GetValue(14));
                    l.fBTimelinePostId = (String)DataUtil.CheckForNullValReturnEmptyString(dr.GetValue(15));
                    l.compositeRating = dr.GetDouble(16);
                    

                    results.Add(l);
                }
                return results;
            }
            catch (Exception e)
            {
                //Dimmi.Data.Log.WriteDataToLog("GetProductReviewsByProduct", e);
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

        public IEnumerable<ProductReview> GetByOwner(int userId)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            List<ProductReview> results = new List<ProductReview>();
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetProductReviewsByOwner", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@owner", userId);

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ProductReview l = new ProductReview();
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
                    l.compositeRating = Convert.ToDouble(dr.GetDecimal(10));
                    l.ownerName = dr.GetString(11);
                    l.latestReviewInstanceId = dr.GetInt32(12);
                    object x = dr.GetValue(13);
                    if (x != null)
                    {
                        l.image = Convert.ToBase64String((byte[])dr.GetValue(13));
                        l.imageFileType = dr.GetValue(14).ToString();
                    }
                    l.fBFeedPostId = (String)DataUtil.CheckForNullValReturnEmptyString(dr.GetValue(15));
                    l.fBTimelinePostId = (String)DataUtil.CheckForNullValReturnEmptyString(dr.GetValue(16));

                    results.Add(l);
                }
                return results;
            }
            catch (Exception e)
            {
                //Dimmi.Data.Log.WriteDataToLog("GetProductReviewsByOwner", e);
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

        public IEnumerable<ProductReview> GetByIdByOwner(int productId, int userId)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            List<ProductReview> results = new List<ProductReview>();
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetProductReviewByOwnerByProductId", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@owner", userId);
                cmd.Parameters.AddWithValue("@productId", productId);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    ProductReview l = new ProductReview();
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
                    l.compositeRating = Convert.ToDouble(dr.GetDecimal(10));
                    l.ownerName = dr.GetString(11);
                    l.latestReviewInstanceId = dr.GetInt32(12);
                    object x = dr.GetValue(13);
                    if (x != null)
                    {
                        l.image = Convert.ToBase64String((byte[])dr.GetValue(13));
                        l.imageFileType = dr.GetValue(14).ToString();
                    }
                    object fbfeed = dr.GetValue(15);
                    object fbtime = dr.GetValue(16);
                    if (fbfeed != DBNull.Value && fbtime != DBNull.Value)
                    {
                        l.fBFeedPostId = dr.GetString(15);
                        l.fBTimelinePostId = dr.GetString(16);
                    }

                    results.Add(l);
                }
                return results;
            }
            catch (Exception e)
            {
                //Dimmi.Data.Log.WriteDataToLog("GetProductReviewIdByProductByOwner", e);
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

        public ProductReview Get(int reviewInstanceId)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;
            //List<Lookup> results = new List<Lookup>();
            ProductReview lup = null;
            try
            {
               
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                //Log.WriteDataToLog("GetProductReviewByReviewId", "Opened Connection"); 
                SqlCommand cmd = new SqlCommand("GetProductReviewByReviewInstanceId", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@reviewInstanceId", reviewInstanceId);

                dr = cmd.ExecuteReader();
                //Log.WriteDataToLog("GetProductReviewByReviewId", "Executed Reader");
                while (dr.Read())
                {
                    lup = new ProductReview();
                    //Log.WriteDataToLog("GetProductReviewByReviewId", "Reading row");
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
                        lup.image = Convert.ToBase64String((byte[])dr.GetValue(12));
                        lup.imageFileType = dr.GetValue(13).ToString();
                    }
                    lup.fBFeedPostId = (String)DataUtil.CheckForNullValReturnEmptyString(dr.GetValue(14));
                    lup.fBTimelinePostId = (String)DataUtil.CheckForNullValReturnEmptyString(dr.GetValue(15));
                    object test = dr.GetValue(16);
                    lup.compositeRating = Convert.ToDouble(test);
                    

                   
                }
                return lup;
            }
            catch (Exception e)
            {
               // Dimmi.Data.Log.WriteDataToLog("GetProductReviewByReviewId", e);
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

        public ProductReview Update(ProductReview productReview)
        {
            SqlConnection conn = null;
            List<ProductReview> results = new List<ProductReview>();
            try
            {

                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("UpdateProductReviewInstance", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@reviewInstanceId", productReview.reviewInstanceId);
                cmd.Parameters.AddWithValue("@comment", productReview.comment);
                cmd.Parameters.AddWithValue("@rating", productReview.rating);
                cmd.Parameters.AddWithValue("@fBFeedPostId", productReview.fBFeedPostId);
                cmd.Parameters.AddWithValue("@fBTimelinePostId", productReview.fBTimelinePostId);

                cmd.ExecuteNonQuery();
               
                return Get(productReview.reviewInstanceId);
            }
            catch (Exception e)
            {
                //Dimmi.Data.Log.WriteDataToLog("GetProductReviewsByProduct", e);
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