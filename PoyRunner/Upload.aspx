<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="PoyRunner.Upload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

 
</head>
<body>
    <form id="form1" runat="server">
    <div>
 This is a simple a supra map centric example of a application that uses the POY web service. 
 Basically you upload a fasta file and a corresponding cvs file with location data and select 
 how much resources you might need. And the program will eventually produce a kml file for you to use with Google earth. See 
   <a href='http://supramap.osu.edu/sm/supramap/home'>supramap </a> for more info.
        
        <br/><br/>
     Fasta File: <input id="FastaFile" type="file" runat="server"/>
     <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required" ControlToValidate="FastaFile"></asp:RequiredFieldValidator>
     <br/><br/>

    Spatial File(csv): <input id="SpatialFile" type="file" runat="server"/>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Required" ControlToValidate="SpatialFile"></asp:RequiredFieldValidator>
    <br/><br/>

    Number Of Nodes:<asp:TextBox ID="TextBoxNumberOfNodes" runat="server" Text="1"></asp:TextBox>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="TextBoxNumberOfNodes" ErrorMessage="must be a number from 1 to 100" ValidationExpression="([1]?[0-9][0-9])|([1-9])"></asp:RegularExpressionValidator>
    <br/>
    Total Time: Hours:<asp:TextBox ID="TextBoxTotalTimeHours" runat="server">0</asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="TextBoxTotalTimeHours" ErrorMessage="must be a number from 0 to 10" ValidationExpression="(10|[0-9])"></asp:RegularExpressionValidator>
     Minutes: <asp:TextBox ID="TextBoxTotalTimeMinutes" runat="server">0</asp:TextBox>
      <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="TextBoxTotalTimeMinutes" ErrorMessage="must be a number from 0 to 60" ValidationExpression="([1-5][0-9])|([0-9])"></asp:RegularExpressionValidator>
    <br/>
    Search Time: Hours:<asp:TextBox ID="TextBoxSearchTimeHours" runat="server">0</asp:TextBox>
    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="TextBoxSearchTimeHours" ErrorMessage="must be a number from 0 to 8" ValidationExpression="[0-5]"></asp:RegularExpressionValidator>
     Minutes:<asp:TextBox ID="TextBoxSearchTimeHoursMinutes" runat="server">0</asp:TextBox> 
     <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="TextBoxSearchTimeHoursMinutes" ErrorMessage="must be a number from 0 to 60" ValidationExpression="([1-5][0-9])|([0-9])"></asp:RegularExpressionValidator>
        <br/>    <br/>
     <asp:Button ID="Submit" runat="server" Text="Submit" onclick="Submit_Click" />
    </div>
  
    </form>
</body>
</html>
