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
                <h2>Register ProStudCreator</h2>
                <p class="control-group">
                    Please enter your FHNW e-mail address and the corresponding password and click 'Sign Up' to continue
                </p>
                <div class="form-group no-bottom">
                    <label for="EnterEMail">E-mail address</label>
                </div>
                <div class="form-group">
                    <asp:TextBox ID="NewUserEmail" CssClass="form-control col-sm-3" runat="server" Width="345px" placeholder="firstname.lastname"></asp:TextBox>
                    <asp:TextBox ID="FixEmailEnding" CssClass="form-control col-sm-3" runat="server" Enabled="false" Width="90px" Text="@fhnw.ch"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="NewUserEmail" Display="Dynamic" CssClass="text-danger" ErrorMessage="The e-mail field is required." />
                </div>
                <div class="form-group no-bottom set-marginTop">
                    <label for="EnterPassword">Password</label>
                </div>
                <div class="form-group">
                    <asp:TextBox ID="NewUserPassword" CssClass="form-control" TextMode="Password" runat="server" placeholder="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="NewUserPassword" Display="Dynamic" CssClass="text-danger" ErrorMessage="The password field is required." />
                </div>
                <div class="form-group no-bottom">
                    <label for="EnterPassword">Re-enter password</label>
                </div>
                <div class="form-group">
                    <asp:TextBox ID="NewUserPasswordValidate" CssClass="form-control" TextMode="Password" runat="server" placeholder="Re-enter password"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="NewUserPasswordValidate"
                        CssClass="text-danger" Display="Dynamic" ErrorMessage="The field to confirm the password is required." />
                    <asp:CompareValidator ID="ComparePasswortsValidator" CssClass="text-danger" Display="Dynamic" ControlToCompare="NewUserPassword" ControlToValidate="NewUserPasswordValidate" runat="server" ErrorMessage="The passwords do not match."></asp:CompareValidator>
                </div>
                <p class="text-danger">
                    <asp:Literal runat="server" ID="ErrorMessage" />
                </p>
                <asp:Button runat="server" ID="CreateUser" CssClass="btn btn-default" Text="Sign Up" Width="113px" OnClick="CreateUser_Click"></asp:Button>
                <a class="btn btn-default" href="login.aspx">Cancel</a>
            </div>
        </div>
    </form>
</body>
</html>
