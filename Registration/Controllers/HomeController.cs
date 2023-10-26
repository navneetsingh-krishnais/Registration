using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Registration.Models;

namespace Registration.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(tblRegistration obj)
        {
            if (ModelState.IsValid)
            {
                RegMVCEntities db = new RegMVCEntities();
                obj.Password = WebSecurity.GetMD5(obj.Password);
                db.tblRegistrations.Add(obj);
                db.SaveChanges();
            }
            return View(obj);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(tblRegistration objUser)
        {
            if (ModelState.IsValid)
            {
                using (RegMVCEntities db = new RegMVCEntities())
                {
                    var MD5Password = WebSecurity.GetMD5(objUser.Password);
                    var obj = db.tblRegistrations.Where(a => a.UserName.Equals(objUser.UserName) && a.Password.Equals(MD5Password)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserID"] = obj.Id.ToString();
                        Session["UserName"] = obj.UserName.ToString();
                        return RedirectToAction("UserDashBoard");
                    }
                }
            }
            return View(objUser);
        }

        public ActionResult UserDashBoard()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string UserName)
        {
            if (ModelState.IsValid)
            {
                if (WebSecurity.UserExists(UserName))
                {
                    string To = UserName, UserID, Password, SMTPPort, Host;
                    string token = Guid.NewGuid().ToString();
                    if (token == null)
                    {
                        // If user does not exist or is not confirmed.
                        return View("Index");
                    }
                    else
                    {
                        WebSecurity.SaveToken(UserName, token);
                        //Create URL with above token
                        var lnkHref = "<a href='" + Url.Action("ResetPassword", "Home", new { email = UserName, code = token }, "https") + "'>Reset Password</a>";
                        //HTML Template for Send email
                        string subject = "Your changed password";
                        string body = "<b>Please find the Password Reset Link. </b><br/>" + lnkHref;
                        //Get and set the AppSettings using configuration manager.
                        EmailManager.AppSettings(out UserID, out Password, out SMTPPort, out Host);
                        //Call send email methods.
                        EmailManager.SendEmail(UserID, subject, body, To, UserID, Password, SMTPPort, Host);
                    }
                }
            }
            return View();
        }

        public ActionResult ResetPassword(string code  , string email)
        {
            tblRegistration model = new tblRegistration();
            model.Token = code;
            return View(model);
        }
        [HttpPost]
        public ActionResult ResetPassword(tblRegistration model)
        {
            if (ModelState.IsValid)
            {
                bool resetResponse = WebSecurity.ResetPassword(model.Token, model.Password);
                if (resetResponse)
                {
                    ViewBag.Message = "Successfully Changed";
                }
                else
                {
                    ViewBag.Message = "Something went horribly wrong!";
                }
            }
            return View(model);
        }
    }
}