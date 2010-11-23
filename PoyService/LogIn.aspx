<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogIn.aspx.cs" Inherits="PoyService.LogIn" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      UserName:<asp:TextBox ID="TextBoxUserName" runat="server"></asp:TextBox><br />
      Password:<asp:TextBox ID="TextBoxPassword" runat="server" TextMode="Password" ></asp:TextBox><br />
        
      <asp:Button ID="ButtonLogOn" runat="server" Text="Log On" 
            onclick="ButtonLogOn_Click" /><br />
      <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
    </div>
    </form>
</body>
</html>
