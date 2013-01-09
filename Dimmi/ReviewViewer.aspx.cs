using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dimmi.Models;
using Dimmi.Data;
using System.Web.UI.HtmlControls;
using Dimmi.DataInterfaces;


namespace Dimmi
{
    public partial class ReviewViewer : System.Web.UI.Page
    {
        static readonly ILogRepository logRepository = new LogRepository();
        static readonly IReviewRepository reviewRepository = new ReviewRepository();
        protected void Page_Load(object sender, EventArgs e)
        {
            string reviewId = "";

            reviewId = Request.QueryString["ReviewId"];
            
            if (reviewId.Length > 0)
            {
                //Log log = new Log();
                try
                {

                    Review review = reviewRepository.Get(Guid.Parse(reviewId));

                    //log.message = "In Page Load";
                    //log.caller = this.ToString();
                    //logRepository.WriteDataToLog(log);

                    lblProductName.Text = review.name;
                    lblProductDesc.Text = review.description;
                    lblComments.Text = review.text;
                    lblCompositeRating.Text = review.compositRating.ToString();
                    lblReviewed.Text = review.createdDate.ToString();
                    lblReviewer.Text = review.userName;
                    lblUserRating.Text = review.rating.ToString();
                    if (review.images.Count > 0 )
                    {
                        string type = review.images[0].fileType;

                        IBProductImg.ImageUrl = "data:image/" + type + ";base64," + review.images[0].data;
                        IBProductImg.Visible = true;
                    }
                    else
                    {
                        IBProductImg.Visible = false;
                    }


                    WriteMetaData(reviewId, review.name, review.description, review.rating, review.text);

                    lbNewerReview.Visible = false;

                }
                catch (Exception ex)
                {
                    //log.message = "";
                    //log.ex = ex;
                    //logRepository.WriteDataToLog(log);
                }



            }
        }
        private void WriteMetaData(string reviewId, String productName, String description, double rating, String comments)
        {
            System.Web.UI.HtmlControls.HtmlMeta meta1 = new HtmlMeta();
            meta1.Name = "og:url";
            meta1.Content = "http://dimmi.apphb.com/ReviewViewer.aspx?reviewId=" + reviewId;
            MetaPlaceHolder.Controls.Add(meta1);
            System.Web.UI.HtmlControls.HtmlMeta meta2 = new HtmlMeta();

            meta2.Name = "og:title";
            meta2.Content = productName + " - " + description;
            MetaPlaceHolder.Controls.Add(meta2);

            System.Web.UI.HtmlControls.HtmlMeta meta3 = new HtmlMeta();
            meta3.Name = "dimmireview:rating";
            meta3.Content = rating.ToString();
            MetaPlaceHolder.Controls.Add(meta3);

            System.Web.UI.HtmlControls.HtmlMeta meta4 = new HtmlMeta();
            meta4.Name = "dimmireview:comments";
            meta4.Content = comments;
            MetaPlaceHolder.Controls.Add(meta4);

            System.Web.UI.HtmlControls.HtmlMeta meta5 = new HtmlMeta();
            meta5.Name = "og:image";
            meta5.Content = "http://dimmi.apphb.com/images/dimmi64x64black.png";
            MetaPlaceHolder.Controls.Add(meta5);

        }

        protected void lbNewerReview_Click(object sender, EventArgs e)
        {

        }
    }
}