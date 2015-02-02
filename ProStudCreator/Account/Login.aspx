<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ProStudCreator.Account.Login" %>

<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Login</title>
<style type="text/css">
    @import url(/Content/bootstrap.css);
    @import url(/Content/bootstrap.min.css);

    body {
        background-color: #f5f5f5;
        color: #333;
        font-family: "Helvetica Neue",Helvetica,Arial,sans-serif;
        font-size: 14px;
        line-height: 20px;

        padding-top: 50px;
        padding-bottom: 20px;
    }
    input {
    width: 100%;
    }
    .form-signin {
        background-color: #fff;
        border: 1px solid #e5e5e5;
        border-radius: 5px;
        box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
        margin: 1em auto 20px;
        max-width: 38.25em;
        padding: 19px 29px 29px;
    }

    .title-box{
        margin-bottom: 12px;
    }

    h2 {
        font-size: 1.5em;
        font: bold;
    }

</style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="form-signin">
            <div class="title-box" >
                <img alt="Fachhochschule Nordwestschweiz" src="/pictures/Logo.png" />
            </div>
            <div class="well" style="background-color: #f5f5f5">
                <h2>Login ProStudCreator</h2>
                <p class="control-group">
                    Please enter your FHNW e-mail address and the corresponding password and click 'Login' to continue.
                </p>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="Email" >E-mail address</asp:Label>
                    <asp:TextBox runat="server" ID="Email" TextMode="Email" CssClass="form-control" TabIndex="1" placeholder="firstname.lastname@fhnw.ch"></asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="Password">Password</asp:Label>
                    <asp:TextBox runat="server" ID="Password" CssClass="form-control" TextMode="Password" TabIndex="2" placeholder="Password"></asp:TextBox>
                </div>
                <asp:button runat="server" OnClick="LogIn" Text="Login" CssClass="btn btn-default" Width="61px" value="Login" tabindex="3" type="submit"></asp:button>
                <a class="pull-right" href="/Account/Forgot.aspx">Forgot password?</a>
            </div>
            <div>
                <asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled">No account? Sign up now</asp:HyperLink>
            </div>
        </div>
        <div class="checkbox">
            <asp:CheckBox runat="server" ID="RememberMe" Visible="false"/>
            <asp:Label runat="server" Visible="false" AssociatedControlID="RememberMe">Speichern?</asp:Label>
        </div>
        <div class="col-md-4">
            <section id="socialLoginForm">
                <uc:OpenAuthProviders Visible="false" runat="server" ID="OpenAuthLogin" />
            </section>
        </div>
    </form>
</body>
</html>