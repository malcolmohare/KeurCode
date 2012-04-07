using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Configuration;
using System.Configuration.Provider;

namespace KeurCode.MVC.Login
{
    public class MembershipProvider : System.Web.Security.MembershipProvider
    {

        public static MembershipProvider Instance { get; set; }

        #region Declerations

        private string connectionString;
        private string pApplicationName;
        private string pCookieDomain;
        private bool pEnablePasswordReset;
        private bool pEnablePasswordRetrieval;
        private bool pRequiresQuestionAndAnswer;
        private bool pRequiresUniqueEmail;
        private int pMaxInvalidPasswordAttempts;
        private int pPasswordAttemptWindow;
        private MembershipPasswordFormat pPasswordFormat;

        #endregion

        #region Properties

        public string ConnectionString
        {
            get { return connectionString; }
        }

        public string CookieDomain
        {
            get { return pCookieDomain; }
        }

        public override string ApplicationName
        {
            get { return pApplicationName; }
            set { pApplicationName = value; }
        }

        public override bool EnablePasswordReset
        {
            get { return pEnablePasswordReset; }
        }


        public override bool EnablePasswordRetrieval
        {
            get { return pEnablePasswordRetrieval; }
        }


        public override bool RequiresQuestionAndAnswer
        {
            get { return pRequiresQuestionAndAnswer; }
        }


        public override bool RequiresUniqueEmail
        {
            get { return pRequiresUniqueEmail; }
        }


        public override int MaxInvalidPasswordAttempts
        {
            get { return pMaxInvalidPasswordAttempts; }
        }


        public override int PasswordAttemptWindow
        {
            get { return pPasswordAttemptWindow; }
        }


        public override MembershipPasswordFormat PasswordFormat
        {
            get { return pPasswordFormat; }
        }

        private int pMinRequiredNonAlphanumericCharacters;

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return pMinRequiredNonAlphanumericCharacters; }
        }

        private int pMinRequiredPasswordLength;

        public override int MinRequiredPasswordLength
        {
            get { return pMinRequiredPasswordLength; }
        }

        private string pPasswordStrengthRegularExpression;

        public override string PasswordStrengthRegularExpression
        {
            get { return pPasswordStrengthRegularExpression; }
        }


        #endregion

        public MembershipProvider()
        {
            Instance = this;
        }

        public override MembershipUser CreateUser(string username,
                                                  string password,
                                                  string email,
                                                  string passwordQuestion,
                                                  string passwordAnswer,
                                                  bool isApproved,
                                                  object providerUserKey,
                                                  out MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && GetUserByEmail(email) != null)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            MembershipUser u = null;

            if (u == null)
            {
                DateTime createDate = DateTime.Now;

                if (providerUserKey == null)
                    providerUserKey = Guid.NewGuid();
                else
                    if (!(providerUserKey is Guid))
                    {
                        status = MembershipCreateStatus.InvalidProviderUserKey;
                        return null;
                    }

                User newUser = new User();
                newUser.ID = (Guid)providerUserKey;
                newUser.Email = email;
                newUser.SetPassword(password);
                newUser.IsAdmin = false;
                newUser.CreationDate = DateTime.Now;

                try
                {
                    using (LoginContext ctx = new LoginContext())
                    {
                        ctx.Users.Add(newUser);
                        ctx.SaveChanges();

                    }
                    status = MembershipCreateStatus.Success;
                }
                catch (Exception e)
                {
                    status = MembershipCreateStatus.ProviderError;
                }

                return GetUser(providerUserKey, false);



            }
            else status = MembershipCreateStatus.DuplicateEmail;

            return null;

        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            using (LoginContext ctx = new LoginContext())
            {
                User user = ctx.GetUserByEmail(username);
                if (user != null)
                {
                    if (!user.ValidatePassword(oldPassword))
                        return false;

                    user.SetPassword(newPassword);
                    ctx.SaveChanges();

                    return true;
                }
            }
            return false;

        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }



        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            //
            // Initialize values from web.config.
            //

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "OsteoMembershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "MyPass Membership provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            pApplicationName = GetConfigValue(config["applicationName"], string.Empty);
            pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            pCookieDomain = GetConfigValue(config["cookieDomain"], null);
            //pWriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));

            string temp_format = config["passwordFormat"];
            if (temp_format == null)
            {
                temp_format = "Hashed";
            }

            switch (temp_format)
            {
                case "Hashed":
                    pPasswordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    pPasswordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    pPasswordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }

            //
            // Initialize OdbcConnection.
            //

            ConnectionStringSettings ConnectionStringSettings = ConfigurationManager.ConnectionStrings["Login"];

            if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim() == "")
            {
                throw new ProviderException("Connection string cannot be blank.");
            }

            connectionString = ConnectionStringSettings.ConnectionString;


            // Get encryption and decryption key information from the configuration.
            //Configuration cfg = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            //machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

            //if (machineKey.ValidationKey.Contains("AutoGenerate"))
            //    if (PasswordFormat != MembershipPasswordFormat.Clear)
            //        throw new ProviderException("Hashed or Encrypted passwords " +
            //                                    "are not supported with auto-generated keys.");

        }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            Guid uID = (Guid)providerUserKey;
            MembershipUser u = null;

            using (LoginContext ctx = new LoginContext())
            {
                User ctxUser = ctx.Users.Where(x => x.ID == uID).FirstOrDefault();

                if (ctxUser != null)
                    u = new MembershipUser(this.Name, "", providerUserKey, ctxUser.Email, "", "", true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            }

            return u;
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        private User GetUserByEmail(string email)
        {
            using (LoginContext ctx = new LoginContext())
            {
                return ctx.GetUserByEmail(email);
            }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            // verify 
            using (LoginContext ctx = new LoginContext())
            {
                User user = ctx.GetUserByEmail(username);
                if (user == null) return false;
                return user.ValidatePassword(password);

            }
        }

    }
}
