<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ProStudCreator.Account.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
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

        .title-box {
            margin-bottom: 12px;
        }

        h2 {
            font-size: 1.5em;
            font: bold;
        }

        .no-bottom {
            margin-bottom: 0px;
        }

        .set-marginTop {
            margin-top: 45px;
        }
    </style>
</head>
<body>

    <form id="form1" runat="server">
        <div class="form-signin">
            <div class="title-box">
                <img alt="Fachhochschule Nordwestschweiz" src="/pictures/Logo.png" />
            </div>
            <div class="well" style="background-color: #f5f5f5">
                <h2>Registrieren ProStudCreator</h2>
                <p class="control-group">
                    Bitte geben Sie Ihre FHNW E-Mail Adresse und das dazugehörige Passwort ein, um sich zu registrieren.
                    Sie erhalten eine E-Mail, damit Sie sich authentifizieren können.
                </p>
                <div class="form-group no-bottom">
                    <label for="EnterEMail">E-Mail Adresse</label>
                </div>
                <div class="form-group">
                    <asp:TextBox ID="NewUserEmail" CssClass="form-control col-sm-3" runat="server" Width="345px" placeholder="vorname.nachname"></asp:TextBox>
                    <asp:TextBox ID="FixEmailEnding" CssClass="form-control col-sm-3" runat="server" Enabled="false" Width="90px" Text="@fhnw.ch"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="NewUserEmail" Display="Dynamic" CssClass="text-danger" ErrorMessage="Das E-Mail-Feld ist erforderlich." />
                </div>
                <div class="form-group no-bottom set-marginTop">
                    <label for="EnterPassword">Passwort</label>
                </div>
                <div class="form-group">
                    <asp:TextBox ID="NewUserPassword" CssClass="form-control" TextMode="Password" runat="server" placeholder="Passwort"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="NewUserPassword" Display="Dynamic" CssClass="text-danger" ErrorMessage="Das Kennwortfeld ist erforderlich." />
                </div>
                <div class="form-group no-bottom">
                    <label for="EnterPassword">Passwort erneut eingeben</label>
                </div>
                <div class="form-group">
                    <asp:TextBox ID="NewUserPasswordValidate" CssClass="form-control" TextMode="Password" runat="server" placeholder="Passwort erneut eingeben"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="NewUserPasswordValidate"
                        CssClass="text-danger" Display="Dynamic" ErrorMessage="Das Feld zum Bestätigen des Kennworts ist erforderlich." />
                    <asp:CompareValidator ID="ComparePasswortsValidator" CssClass="text-danger" Display="Dynamic" ControlToCompare="NewUserPassword" ControlToValidate="NewUserPasswordValidate" runat="server" ErrorMessage="Diese Passwörter stimmen nicht überein."></asp:CompareValidator>
                </div>
                <p class="text-danger">
                    <asp:Literal runat="server" ID="ErrorMessage" />
                </p>
                <asp:Button runat="server" ID="CreateUser" CssClass="btn btn-default" Text="Registrieren" Width="113px" OnClick="CreateUser_Click"></asp:Button>
                <a class="btn btn-default" href="login.aspx">Abbrechen</a>
            </div>
        </div>
    </form>
</body>
</html>
