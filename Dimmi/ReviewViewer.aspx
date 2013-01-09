<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReviewViewer.aspx.cs" Inherits="Dimmi.ReviewViewer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server" prefix="og: http://ogp.me/ns# fb: http://ogp.me/ns/fb# dimmireview: http://ogp.me/ns/fb/dimmireview#">
    <title></title>
    <meta property="fb:app_id" content="306768242757799" /> 
    <meta property="og:type" content="dimmireview:product" />
    <asp:PlaceHolder id="MetaPlaceHolder" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
    <div>

        <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/dimmifullbanner800x150.png" />
        <br />

    </div>
    <div>
    
        <asp:ImageButton ID="IBProductImg" runat="server" /><br />
    
        <asp:Label ID="lblProductName" runat="server" Text="ProductName"></asp:Label><br />
        <asp:Label ID="lblProductDesc" runat="server" Text="Description"></asp:Label><br />
        <asp:Label ID="lblCompositeRating" runat="server" Text="Composite Rating"></asp:Label><br />
        <asp:Label ID="lblUserRating" runat="server" Text="User Rating"></asp:Label><br />
        <asp:Label ID="lblComments" runat="server" Text="Comments"></asp:Label><br />
        <asp:Label ID="lblReviewer" runat="server" Text="Reviewer"></asp:Label><br />
        <asp:Label ID="lblReviewed" runat="server" Text="Reviewed"></asp:Label><br />

        <asp:LinkButton ID="lbNewerReview" runat="server" OnClick="lbNewerReview_Click">There is a newer version of this review.  Click here to view it.</asp:LinkButton>

        <br />
    
    </div>
    </form>
</body>
</html>