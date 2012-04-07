using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeurCode.MVC.Login
{
    public class RoleProvider : System.Web.Security.RoleProvider
    {

        public static RoleProvider Instance { get; set; }

        public RoleProvider()
        {
            Instance = this;
        }

        public override string[] GetRolesForUser(string username)
        {
            LoginContext ctx = new LoginContext();

            User user = ctx.GetUserByEmail(username);

            return user.IsAdmin ? new string[] { "Admin" } : new string[] { };
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

    }
}
