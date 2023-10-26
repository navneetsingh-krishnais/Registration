using Registration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Registration
{
    public class WebSecurity
    {
        public static bool UserExists(string UserName)
        {
            using (RegMVCEntities db = new RegMVCEntities())
            {
                var obj = db.tblRegistrations.Where(a => a.UserName.Equals(UserName)).FirstOrDefault();
                if (obj != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool ResetPassword(string Token, string Password)
        {
            using (RegMVCEntities db = new RegMVCEntities())
            {
                var obj = db.tblRegistrations.Where(a => a.Token.Equals(Token)).FirstOrDefault();
                if (obj != null)
                {
                    obj.Password = GetMD5(Password);
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static void SaveToken(string userName, string token)
        {
            using (RegMVCEntities db = new RegMVCEntities())
            {
                var obj = db.tblRegistrations.Where(a => a.UserName.Equals(userName)).FirstOrDefault();
                if (obj != null)
                {
                    obj.Token = token;
                    db.SaveChanges();
                }
            }
        }

        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
    }
}