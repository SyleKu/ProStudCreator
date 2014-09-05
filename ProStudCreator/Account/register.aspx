<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ProStudCreator.Account.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <title>Registrieren</title>
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
    .loginSize {
        margin: 1em auto 20px; 
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
                <h2>Registrieren ProStudCreator</h2>
                <p class="control-group">
                    Bitte geben Sie Ihre FHNW E-Mail Adresse und das dazugehörige Passwort ein, um sich zu registrieren.
                    Sie erhalten eine E-Mail, damit Sie sich authentifizieren können.
                </p>
                <div class="form-group">
                    <label for="EnterEMail">E-Mail Adresse</label>
                    <asp:TextBox ID="NewUserEmail" TextMode="Email" CssClass="form-control" runat="server" TabIndex="1" placeholder="E-Mail Adresse"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label for="EnterPassword">Passwort</label>
                    <asp:TextBox ID="NewUserPassword" CssClass="form-control" TextMode="Password" runat="server" TabIndex="2" placeholder="Passwort"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label for="EnterPassword">Passwort erneut eingeben</label>
                    <asp:TextBox ID="NewUserPasswordValidate" CssClass="form-control" TextMode="Password" runat="server" TabIndex="3" placeholder="Passwort erneut eingeben"></asp:TextBox>
                </div>
                <asp:Button runat="server" CssClass="btn btn-default" Text="Registrieren" TabIndex="4" Width="113px"></asp:Button>
                <a class="btn btn-default" href="login.aspx" tabindex="5">Abbrechen</a>
            </div>
        </div>
    </form>
</body>
</html>
