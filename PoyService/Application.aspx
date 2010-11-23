<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Application.aspx.cs" Inherits="PoyService.Application" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div  align="center">
    <h2>Hello and welcome to the Poy Web Service.</h2>
     <h4> Please fill out the following details and after we after we review your application an email will be sent out with further instructions.</h4><br/><br/>
     <table>
     <tr><td><span> Name: </span></td><td> <asp:TextBox ID="Name" runat="server"></asp:TextBox> </td><td> <asp:RequiredFieldValidator ControlToValidate="Name" ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required"/></td></tr>
      <tr><td><span> Email: </span> </td><td><asp:TextBox ID="Email" runat="server"></asp:TextBox></td><td><asp:RegularExpressionValidator ControlToValidate="Email" ID="RegularExpressionValidator1" runat="server" ValidationExpression="[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}" ErrorMessage="Unsupported Email Format"/></td></tr>
      <tr><td><span> Organzation: </span></td><td> <asp:TextBox ID="Organzation" runat="server"></asp:TextBox> </td><td> <asp:RequiredFieldValidator ControlToValidate="Organzation" ID="RequiredFieldValidator2" runat="server" ErrorMessage="Required"/></td></tr>
     </table>
     <span> Give a brief description of the application you want to build with the Poy Web Service (optional). </span><br/>
     <asp:TextBox ID="description" runat="server" Height="100px" Width="800px" Wrap="true" TextMode="MultiLine"  ></asp:TextBox><br/>
        <asp:Button ID="SubmitApplication" runat="server" Text="Submit" 
            onclick="SubmitApplication_Click" />
    </div>
    </form>
</body>
</html>
