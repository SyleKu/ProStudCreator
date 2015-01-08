using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using ProStudCreator.Models;
using System.Web.UI.WebControls;

namespace ProStudCreator.Account
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CreateUser_Click(object sender, EventArgs e)
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = new ApplicationUser() { UserName = NewUserEmail.Text + FixEmailEnding.Text, Email = NewUserEmail.Text + FixEmailEnding.Text };
            IdentityResult result = manager.Create(user, NewUserPassword.Text);
            if (result.Succeeded)
            {
                // Weitere Informationen zum Aktivieren der Kontobestätigung und Kennwortzurücksetzung finden Sie unter "http://go.microsoft.com/fwlink/?LinkID=320771".
                string code = manager.GenerateEmailConfirmationToken(user.Id);
                string callbackUrl = IdentityHelper.GetUserConfirmationRedirectUrl(code, user.Id, Request);
                manager.SendEmail(user.Id, "Konto bestätigen", "Bitte bestätigen Sie Ihr Konto. Klicken Sie dazu <a href=\"" + callbackUrl + "\">hier</a>.");

                IdentityHelper.SignIn(manager, user, isPersistent: false);
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            }
            else
            {
                ErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }
    }
}