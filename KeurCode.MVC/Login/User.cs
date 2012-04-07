using System;
using System.ComponentModel.DataAnnotations;

namespace KeurCode.MVC.Login
{
    public class User
    {
        public static string EMAIL_COOKIE_KEY = "Email";
        public static string REMEMBER_ME_KEY = "RememberMe";

        public static int COOKIE_TIMEOUT = 30;
        public static int COOKIE_TIMEOUT_MAX = 525600;  // Timeout in minutes, 525600 = 365 days 


        public Guid ID { get; set; }
        public string Email { get; set; }
        public Guid Password { get; set; }
        public DateTime CreationDate { get; set; }

        [NotMapped]
        public bool IsAdmin { get; set; }

        public static Guid HashPassword(string email, string password)
        {
            using (System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                return new Guid(md5.ComputeHash(System.Text.ASCIIEncoding.Default.GetBytes(String.Format("{0},{1}", email.Trim().ToUpper(), password))));
            }
        }

        public void SetPassword(string password)
        {
            Password = HashPassword(Email, password);
        }

        public bool ValidatePassword(string password)
        {
            if (Password == HashPassword(Email, password))
                return true;

            return false;
        }
    }
}
