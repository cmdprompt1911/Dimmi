using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dimmi.Data;
using Dimmi.DataInterfaces;
using Dimmi.Models;
using System.Data.SqlClient;
using System.Data;
using MongoDB.Bson;

namespace Dimmi
{
    public partial class Migrate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;

            try
            {
                UserRepository ur = new UserRepository();
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();

                SqlCommand cmd = new SqlCommand("Select * from [dbo].[User]", conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    User user = new User();
                    user.emailAddress = dr.GetString(1);
                    user.lastLogin = dr.GetDateTime(2);
                    user.locale = dr.GetString(3);
                    user.firstName = dr.GetString(4);
                    user.lastName = dr.GetString(5);
                    user.timezoneFromUTC = dr.GetInt32(6);
                    user.name = dr.GetString(7);
                    if (dr.GetValue(8) != DBNull.Value)
                        user.gender = dr.GetString(8);
                    if (dr.GetValue(9) != DBNull.Value && dr.GetString(9).Trim().Length > 0)
                        user.location = dr.GetString(9);
                    if (dr.GetValue(10) != DBNull.Value && dr.GetString(10).Trim().Length > 0)
                        user.fBUsername = dr.GetString(10);
                    if (dr.GetValue(11) != DBNull.Value && dr.GetString(11).Trim().Length > 0)
                        user.fBLink = dr.GetString(11);

                    user = (User)ur.Add(user);
                    string fbstuff = user.fBLink;
                }

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;

            try
            {
                ReviewableRepository br = new ReviewableRepository();
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();

                SqlCommand cmd = new SqlCommand("Select * from [dbo].[Company]", conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Reviewable r = new Reviewable();

                    r.name = dr.GetString(1);
                    r.reviewableType = "business";
                    r.images = new List<Models.Image>();
                    r.issuerCountryCode = "en_us";
                    r.issuerCountry = "United States";
                    r.createdDate = DateTime.UtcNow;

                    br.Add(r, Guid.Empty);
                }
                dr.Close();

                IEnumerable<Reviewable> bizs = br.Get();
                foreach (Reviewable biz in bizs)
                {
                    SqlDataReader dr2 = null;
                    string query = @"SELECT top 1 i.uid, i.data, i.fileType, lkift.FileTypeName as imageTypeName, c.Name, 1 as Type, i.dateCreated FROM Image i
	                    INNER JOIN lk_Image_FileType lkift on lkift.id = i.fileType
	                    INNER JOIN Company_Image ci on i.uid = ci.ImageId
	                    INNER JOIN Company c on c.id = ci.CompanyId
	                    WHERE c.name = '" + biz.name + "' ORDER by i.dateCreated DESC";
                    SqlCommand cmd2 = new SqlCommand(query, conn);

                    dr2 = cmd2.ExecuteReader();
                    while (dr2.Read())
                    {
                        Models.Image l = new Models.Image();
                        Guid newId = Guid.NewGuid();
                        l.id = newId;
                        Object data = dr2.GetValue(1);
                        l.data = (byte[])data;

                        l.fileType = dr2.GetString(3);
                        l.category = "company";
                        l.description = dr2.GetValue(4).ToString();
                        l.type = dr2.GetInt32(5);
                        l.dateCreated = dr2.GetDateTime(6);

                        biz.images.Add(l);

                    }
                    br.Update(biz, Guid.Empty);
                    dr2.Close();

                }



            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;

            try
            {
                ReviewableRepository pr = new ReviewableRepository();
                
                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();

                SqlCommand cmd = new SqlCommand("GetProductsByName", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", "");
                cmd.Parameters.AddWithValue("@userId", 1);

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Reviewable prod = new Reviewable();
                    prod.name = dr.GetString(1);
                    if (dr.GetValue(2) != DBNull.Value)
                        prod.description = dr.GetString(2);
                    if (dr.GetValue(3) != DBNull.Value && dr.GetString(3).Trim().Length != 0)
                        prod.outsideCode = dr.GetString(3);
                    if (dr.GetValue(4) != DBNull.Value && dr.GetString(4).Trim() != "None")
                        prod.outsideCodeType = dr.GetString(4).Trim();
                    if (dr.GetValue(5) != DBNull.Value)
                        prod.issuerCountryCode = "en_us"; // dr.GetString(5);
                    if (dr.GetValue(6) != DBNull.Value)
                        prod.issuerCountry = dr.GetString(6);
                    //if (dr.GetValue(7) != DBNull.Value)
                        //prod.manufacturerid = dr.GetString(7);
                    //if (dr.GetValue(8) != DBNull.Value)
                        //prod.modelNum = dr.GetString(8);

                    if (dr.GetValue(10) != DBNull.Value)
                    {
                        Reviewable biz = pr.GetByName(dr.GetString(10), Guid.Empty).First();
                        prod.parentReviewableId = biz.id;
                        prod.parentName = biz.name;
                    }
                    prod.reviewableType = "product";
                    prod.images = new List<Models.Image>();
                    prod.createdDate = DateTime.UtcNow;
                    pr.Add(prod, Guid.Empty);

                }
                dr.Close();

                IEnumerable<Reviewable> prods = pr.Get();
                foreach (Reviewable prod in prods)
                {
                    SqlDataReader dr2 = null;
                    string query = @"SELECT top 1 i.uid, i.data, i.fileType, lkift.FileTypeName as imageTypeName, p.Name, 2 as Type, i.dateCreated FROM Image i
	                    INNER JOIN lk_Image_FileType lkift on lkift.id = i.fileType
	                    INNER JOIN Product_Image pi on i.uid = pi.ImageId
	                    INNER JOIN Product p on p.id = pi.ProductId
	                    WHERE p.name = '" + prod.name + "' ORDER by i.dateCreated DESC";
                    SqlCommand cmd2 = new SqlCommand(query, conn);

                    dr2 = cmd2.ExecuteReader();
                    while (dr2.Read())
                    {
                        Models.Image l = new Models.Image();
                        Guid newId = Guid.NewGuid(); //.GenerateNewId();
                        l.id = newId;
                        Object data = dr2.GetValue(1);
                        l.data = (byte[])data;

                        l.fileType = dr2.GetString(3);
                        l.category = "tile";
                        l.description = dr2.GetValue(4).ToString();
                        l.type = dr2.GetInt32(5);
                        l.dateCreated = dr2.GetDateTime(6);

                        prod.images.Add(l);

                    }
                    pr.Update(prod, Guid.Empty);
                    dr2.Close();

                }



            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            SqlConnection conn = null;
            SqlDataReader dr = null;

            try
            {
                ReviewRepository pr = new ReviewRepository();
                ReviewableRepository rr = new ReviewableRepository();
                UserRepository ur = new UserRepository();

                conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataServices"].ConnectionString);
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT * from vCurrentProductReviews", conn);

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Review prod = new Review();
                    prod.name = dr.GetString(4);
                    if (dr.GetValue(5) != DBNull.Value)
                        prod.description = dr.GetString(5);
                    if (dr.GetValue(6) != DBNull.Value)
                        prod.createdDate = dr.GetDateTime(6);
                    if (dr.GetValue(7) != DBNull.Value)
                        prod.lastModified = dr.GetDateTime(7);
                    if (dr.GetValue(8) != DBNull.Value)
                        prod.text = dr.GetString(8);
                    if (dr.GetValue(9) != DBNull.Value)
                        prod.rating = dr.GetDouble(9);
                    if (dr.GetValue(10) != DBNull.Value)
                    {
                        prod.userName = dr.GetString(10);
                        prod.user = ur.GetByName(prod.userName).id;
                    }
                    if (dr.GetValue(14) != DBNull.Value)
                        prod.fBFeedPostId = dr.GetString(14);
                    if (dr.GetValue(15) != DBNull.Value)
                        prod.fBTimelinePostId = dr.GetString(15);


                    if (dr.GetValue(4) != DBNull.Value)
                    {
                        Reviewable x = rr.GetByName(dr.GetString(4), Guid.Empty).First();
                        prod.parentReviewableId = x.id;
                        prod.parentName = x.name;


                        prod.outsideCode = x.outsideCode;
                        prod.outsideCodeType = x.outsideCodeType;
                        prod.issuerCountryCode = x.issuerCountryCode;
                        prod.issuerCountry = x.issuerCountry;
                        prod.providedByBizId = x.parentReviewableId;
                        prod.providedByBizName = x.parentName;

                    }
                    prod.reviewableType = "product";

                    pr.Add(prod);

                }
                dr.Close();
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            string x = Guid.NewGuid().ToString();
            var operations = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("parentReviewableId", "79ffe6e1-05f0-438a-a928-467e6119c460" )),
                //new BsonDocument("$project", new BsonDocument { { "_id", 1 }, { "words", 1 } }),
                //new BsonDocument("$unwind", "$words"),
                //new BsonDocument("$group", new BsonDocument { { "_id", new BsonDocument("tags", "$words") }, { "count", new BsonDocument("$sum", 1) } }),
                //new BsonDocument("$sort", new BsonDocument("count", 1)),
                //new BsonDocument("$limit", 5)
                //new BsonDocument("$project", new BsonDocument { { "_id", 1 }, { "type", 1 }, { "dateCreated", 1 } }),
                //new BsonDocument("$group", new BsonDocument { { "type", "$type" } }),
                //new BsonDocument("$sort", new BsonDocument { { "type", -1 }, { "dateCreated", -1 } }),
                new BsonDocument("$group", new BsonDocument { { "_id", new BsonDocument("product", "$parentName") }, { "parentId", new BsonDocument("$first", "$parentReviewableId") }, { "numReviews", new BsonDocument("$sum", 1) }, { "composite", new BsonDocument("$avg", "$rating") }}),
                //new BsonDocument("$project", new BsonDocument { { "id", 1 }, { "type", 1 }, { "dateCreated", 1 } }),
                //new BsonDocument("$sort", new BsonDocument("dateCreated", -1)),
                //new BsonDocument("$limit", 5)
            };

            DBRepository.MongoRepository<Models.ReviewData> _reviewsRepository = new DBRepository.MongoRepository<Models.ReviewData>("Reviews");
            DBRepository.MongoRepository<Models.ReviewableData> _reviewablesRepository = new DBRepository.MongoRepository<Models.ReviewableData>("Reviewables");

            var results = _reviewsRepository.Collection.Aggregate(operations);
            Label1.Text = results.ResultDocuments.ToJson();
           
            foreach (BsonDocument doc in results.ResultDocuments)
            {
                BsonValue val; // = new BsonObjectId(;
                doc.TryGetValue("parentId", out val);
                Guid o = Guid.Parse(val.AsString);
                ReviewableRepository rr = new ReviewableRepository();
                Reviewable r = rr.Get(o, Guid.Empty);
                doc.TryGetValue("numReviews", out val);
                r.numReviews = val.AsInt32;
                doc.TryGetValue("composite", out val);
                r.compositRating = val.AsDouble;
                rr.Update(r, Guid.Empty);
            }
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            string x = Guid.NewGuid().ToString();
            ReviewStatisticsRepository rsp = new ReviewStatisticsRepository();
            rsp.Recalculate();
        }
    }
}