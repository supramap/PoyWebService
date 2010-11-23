<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Loading.aspx.cs" Inherits="PoyRunner.Loading" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     
    <asp:Literal ID="ScriptCode" runat="server" Text=""/>
</head>
<body>
  <%--  <img alt="" src="ajax-loader.gif" id="loaderGif" runat="server" />--%>
    Book mark this page and try to come back in a few hours to get your kml file or hit your browsers refresh button.
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />  
    <div>
     
      

    </div>
    </form>
</body>
</html>
