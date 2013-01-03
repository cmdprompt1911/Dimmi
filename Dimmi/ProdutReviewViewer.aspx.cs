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
    public partial class ProdutReviewViewer : System.Web.UI.Page
    {
        static readonly ILogRepository logRepository = new LogRepository();
        static readonly IProductReviewRepository reviewRepository = new ProductReviewRepository();
        protected void Page_Load(object sender, EventArgs e)
        {
            int reviewId = 0;

            int.TryParse(Request.QueryString["ReviewId"], out reviewId);
            if (reviewId > 0)
            {
                Log log = new Log();
                try
                {
                    ProductReview review = reviewRepository.Get(reviewId);
                    
                    log.message = "In Page Load";
                    log.caller = this.ToString();
                    logRepository.WriteDataToLog(log);

                    lblProductName.Text = review.productName;
                    lblProductDesc.Text = review.productDescription;
                    lblComments.Text = review.comment;
                    lblCompositeRating.Text = review.compositeRating.ToString();
                    lblReviewed.Text = review.created.ToString();
                    lblReviewer.Text = review.ownerName;
                    lblUserRating.Text = review.rating.ToString();
                    if (review.image != null && review.image != "")
                    {
                        string type = review.imageFileType;

                        IBProductImg.ImageUrl = "data:image/" + type + ";base64," + review.image;
                        IBProductImg.Visible = true;
                    }
                    else
                    {
                        IBProductImg.Visible = false;
                    }
                    

                    WriteMetaData(reviewId, review.productName, review.productDescription, review.rating, review.comment);

                    if (review.latestReviewInstanceId == review.reviewInstanceId)
                    {
                        lbNewerReview.Visible = false;
                    }
                    else
                    {
                        lbNewerReview.PostBackUrl = "http://dimmi.somee.com/ProductReviewViewer.aspx?reviewId=" + review.latestReviewInstanceId;
                    }
                }
                catch (Exception ex)
                {
                    log.message = "";
                    log.ex = ex;
                    logRepository.WriteDataToLog(log);
                }



            }
        }
        private void WriteMetaData(int reviewId, String productName, String description, double rating, String comments)
        {
            System.Web.UI.HtmlControls.HtmlMeta meta1 = new HtmlMeta();
            meta1.Name = "og:url";
            meta1.Content = "http://dimmi.somee.com/ProductReviewViewer.aspx?reviewId=" + reviewId.ToString();
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
            meta5.Content = "http://dimmi.somee.com/images/dimmi64x64black.png";
            MetaPlaceHolder.Controls.Add(meta5);

        }

        protected void lbNewerReview_Click(object sender, EventArgs e)
        {

        }
    }
}