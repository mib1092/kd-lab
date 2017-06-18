using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using IMCMS.Common.Utility;
using System.Net.Mail;
using System.Web;
using System.Data.SqlTypes;
using System.Web.Helpers;

namespace IMCMS.Common.Authentication
{
    internal sealed class Local
    {
        /// <summary>
        /// Validate user against database
        /// </summary>
        /// <param name="Username">Username of user to be authenciated</param>
        /// <param name="Password">Password of user to be authenciated</param>
        /// <param name="IncludeRestricted">Include failsafe/restricted Imagemakers user in the event remote authenication fails</param>
        /// <returns></returns>
        internal static AuthenticationResponse ValidateUser(string Username, string Password, string IP, bool IncludeRestricted = false)
        {

            SqlConnection sqlCnn = new SqlConnection(Database.ConnectionString.GetConnectionStringByServer().ConnectionString);
            sqlCnn.Open();

            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) as Count FROM [UserAttempts] WHERE [When] >= @Before AND [When] <= @After AND IPAddress = @IP", sqlCnn))
            {
                command.Parameters.Add(new SqlParameter("@IP", IP));
                command.Parameters.Add(new SqlParameter("@Before", DateTime.Now.AddMinutes(-5)));
                command.Parameters.Add(new SqlParameter("@After", DateTime.Now));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    int Count = int.Parse(reader["Count"].ToString());
                    if(Count > 20)
                        return AuthenticationResponse.IPLocked;
                }
            }

            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) as Count FROM [UserAttempts] WHERE [When] >= @Before AND [When] <= @After AND Username = @Username", sqlCnn))
            {
                command.Parameters.Add(new SqlParameter("@Username", Username));
                command.Parameters.Add(new SqlParameter("@Before", DateTime.Now.AddMinutes(-5)));
                command.Parameters.Add(new SqlParameter("@After", DateTime.Now));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    int Count = int.Parse(reader["Count"].ToString());
                    if (Count > 8)
                        return AuthenticationResponse.AccountLocked;
                }
            }

            using (SqlCommand command = new SqlCommand("SELECT * FROM [Users] WHERE Username = @Username AND Type = @Type", sqlCnn))
            {
                command.Parameters.Add(new SqlParameter("@Username", Username));
                command.Parameters.Add(new SqlParameter("@Type", IncludeRestricted ? 1 : 0));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (Crypto.VerifyHashedPassword(reader["Password"].ToString(), Password))
                        {
                            return AuthenticationResponse.Authorized;
                        }
                    }
                    
                    return AuthenticationResponse.Unauthorized;
                }
            }
        }

        internal static void LogUnauthorizedAttempt(string Username, string IP)
        {
            SqlConnection sqlCnn = new SqlConnection(Database.ConnectionString.GetConnectionStringByServer().ConnectionString);
            sqlCnn.Open();

            using (SqlCommand command1 = new SqlCommand("INSERT INTO [UserAttempts] (IPAddress, Username, [When]) VALUES (@IPAddress, @Username, @When)", sqlCnn))
            {
                command1.Parameters.Add(new SqlParameter("@IPAddress", IP));
                command1.Parameters.Add(new SqlParameter("@Username", Username));
                command1.Parameters.Add(new SqlParameter("@When", DateTime.Now));

                command1.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// If user is found, update with password hash, and date/time, then send email to user
        /// Returns `void` since it could be an attack vector to know if the user was found
        /// </summary>
        /// <param name="Username"></param>
        internal static void ForgotPassword(string Username)
        {
            SqlConnection sqlCnn = new SqlConnection(Database.ConnectionString.GetConnectionStringByServer().ConnectionString);
            sqlCnn.Open();
            using (SqlCommand command = new SqlCommand("UPDATE [Users] SET PasswordResetTime = @Now, PasswordResetHash = @Hash WHERE Username = @Username AND Type = 0", sqlCnn))
            {
                string Hash = RandomString.Generate(24);
                command.Parameters.Add(new SqlParameter("@Username", Username));
                command.Parameters.Add(new SqlParameter("@Now", DateTime.Now));
                command.Parameters.Add(new SqlParameter("@Hash", Hash));

                int NumberRows = command.ExecuteNonQuery();

                if (NumberRows > 0)
                {
                    MailMessage msg = new MailMessage();
                    msg.To.Add(Username);
                    msg.Subject = "Password Reset";
                    msg.IsBodyHtml = true;
                    msg.Body = String.Format(@"<p>Someone has requested to have the password reset at http://{0}.</p>

<p>If you did not request a password reset you do not need to take any action.</p>

<p>If you did, please click the link below to reset the password:<br />
<a href=""{1}"">{1}</a></p>",
        HttpContext.Current.Request.Url.Host,
        "http://" + HttpContext.Current.Request.Url.Host + "/SiteAdmin/Account/ResetPassword/" + HttpContext.Current.Server.UrlEncode(Hash));

                    SmtpClient smtp = new SmtpClient();
                    try
                    {
                        smtp.Send(msg);
                    }
                    catch (Exception) { }
                }

            }
        }


        /// <summary>
        /// Ensures the hash is valid, and has been accessed within the correct amount of time
        /// </summary>
        /// <param name="Hash"></param>
        /// <returns></returns>
        internal static bool ForgotPasswordHashCheck(string Hash)
        {
            SqlConnection sqlCnn = new SqlConnection(Database.ConnectionString.GetConnectionStringByServer().ConnectionString);
            sqlCnn.Open();
            using (SqlCommand command = new SqlCommand("SELECT * FROM [Users] WHERE PasswordResetHash = @Hash AND Type = 0", sqlCnn))
            {
                command.Parameters.Add(new SqlParameter("@Hash", Hash));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if(!reader.HasRows)
                        return false;

                    reader.Read();
                    DateTime ResetDateTime = DateTime.Parse(reader["PasswordResetTime"].ToString());
                    return ResetDateTime.AddHours(24) > DateTime.Now;
                }
            }
            
        }


        /// <summary>
        /// Zero out hash, update password
        /// </summary>
        /// <param name="Password"></param>
        /// <param name="Hash"></param>
        internal static void ForgotPasswordResetPassword(string Password, string Hash)
        {
            // ensure the hash is still valid, and isn't outside the time window
            if (ForgotPasswordHashCheck(Hash))
            {
                SqlConnection sqlCnn = new SqlConnection(Database.ConnectionString.GetConnectionStringByServer().ConnectionString);
                sqlCnn.Open();
                using (SqlCommand command = new SqlCommand("UPDATE [Users] SET PasswordResetTime = @Date, PasswordResetHash = '', Password = @Password WHERE PasswordResetHash = @Hash AND Type = 0", sqlCnn))
                {
                    command.Parameters.Add(new SqlParameter("@Hash", Hash));
                    command.Parameters.Add(new SqlParameter("@Date", SqlDateTime.MinValue));
                    command.Parameters.Add(new SqlParameter("@Password", Crypto.HashPassword(Password)));
                    command.ExecuteNonQuery();
                }
            }
            else
                throw new ArgumentException("Invalid hash");
        }


        internal static void CreateUser(string Username, string Password, bool IsRestricted)
        {
            SqlConnection sqlCnn = new SqlConnection(Database.ConnectionString.GetConnectionStringByServer().ConnectionString);
            sqlCnn.Open();
            using (SqlCommand command = new SqlCommand("INSERT INTO [Users] (Username, Password, Type) VALUES (@Username, @Password, @Type)", sqlCnn))
            {
                command.Parameters.Add(new SqlParameter("@Username", Username));
                command.Parameters.Add(new SqlParameter("@Password", Crypto.HashPassword(Password)));
                command.Parameters.Add(new SqlParameter("@Type", IsRestricted ? 1 : 0));

                command.ExecuteNonQuery();
            }
        }

        internal static void ChangePassword(string Username, string OldPassword, string NewPassword)
        {
            SqlConnection sqlCnn = new SqlConnection(Database.ConnectionString.GetConnectionStringByServer().ConnectionString);
            sqlCnn.Open();
            using (SqlCommand command = new SqlCommand("SELECT * FROM [Users] WHERE Username = @Username AND Type = 0", sqlCnn))
            {
                command.Parameters.Add(new SqlParameter("@Username", Username));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    if (Crypto.VerifyHashedPassword(reader["Password"].ToString(), OldPassword))
                    {
                        using (SqlCommand command2 = new SqlCommand("UPDATE [Users] SET Password = @Password WHERE Username = @Username", sqlCnn))
                        {
                            command2.Parameters.Add(new SqlParameter("@Username", Username));
                            command2.Parameters.Add(new SqlParameter("@Password", Crypto.HashPassword(NewPassword)));
                            command2.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        internal static void CreateDatabaseTables()
        {
            SqlConnection sqlCnn = new SqlConnection(Database.ConnectionString.GetConnectionStringByServer().ConnectionString);
            sqlCnn.Open();
            string Command = @"CREATE TABLE [dbo].[Users](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](255) NOT NULL,
	[Password] [varchar](max) NOT NULL,
	[PasswordResetHash] [varchar](255) NULL,
	[PasswordResetTime] [datetime] NULL,
	[Type] [int] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
";
            SqlCommand cmd = new SqlCommand(Command, sqlCnn);
            cmd.ExecuteNonQuery();

            Command = @"CREATE TABLE [dbo].[UserAttempts](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IPAddress] [varchar](255) NOT NULL,
	[When] [datetime] NOT NULL,
    [Username] [varchar](255) NULL,
 CONSTRAINT [PK_UserAttempts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
";
            cmd = new SqlCommand(Command, sqlCnn);
            cmd.ExecuteNonQuery();
        }

        internal static bool VerifyDatabase()
        {
            try
            {
                SqlConnection sqlCnn = new SqlConnection(Database.ConnectionString.GetConnectionStringByServer().ConnectionString);
                sqlCnn.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM [Users];", sqlCnn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
