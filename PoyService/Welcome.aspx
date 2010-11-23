<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="PoyService.Welcome" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div align="center">
    <h1>Welcome</h1><br/>
    <table style="width: 100%">
    <tr> <th>Existing Users<br /><br />  </th><th> New Users <br /><br /></th> </tr> 
  
  <tr> <th align="center">
    <%--    UserName:<asp:TextBox ID="TextBoxUserName" runat="server"></asp:TextBox><br />
        Password:<asp:TextBox ID="TextBoxPassword" runat="server"></asp:TextBox><br />
        <asp:Button ID="ButtonLogOn" runat="server" Text="Log On" /><br />--%>
        <asp:Login ID="Login1" runat="server" onauthenticate="Login1_Authenticate" >
        </asp:Login>
        </th><th align="center">
        <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" 
              oncreateduser="CreateUserWizard1_CreatedUser">
            <WizardSteps>
                <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                </asp:CompleteWizardStep>
            </WizardSteps>
        </asp:CreateUserWizard>
        </th> </tr > 
          </table>
    </div>
    </form>
</body>
</html>
