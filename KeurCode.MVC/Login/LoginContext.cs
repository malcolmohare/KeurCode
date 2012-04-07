using System.Data.Entity;
using System.Linq;


namespace KeurCode.MVC.Login
{
    public class LoginContext : DbContext
    {
        public LoginContext() : base("Login") { }

        public DbSet<User> Users { get; set; }

        public User GetUserByEmail(string email)
        {
            return (from u in Users
                    where u.Email == email
                    select u).FirstOrDefault();
        }
    }
}
