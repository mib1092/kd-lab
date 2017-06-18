using System.Configuration;
using System.Web;
using IMCMS.Common.Extensions;

namespace IMCMS.Common.Database
{
    using System;

    public class ConnectionString
    {
        public static ConnectionStringSettings GetConnectionStringByServer()
        {
            return GetConnectionStringByServer(new HttpContextWrapper(HttpContext.Current));
        }
        /// <summary>
        /// Gets the correct connection string for the server running the application
        /// </summary>
        /// <returns>Connection string depending on the server</returns>
        public static ConnectionStringSettings GetConnectionStringByServer(HttpContextBase context)
        {

            return ConfigurationManager.ConnectionStrings[GetConnectionTypeByServer(context)];
        }

        public static string GetConnectionTypeByServer()
        {
            if ((System.Web.Hosting.HostingEnvironment.SiteName != null) && (HttpContext.Current == null))
            {
                var connectionString = "Local";
                if (System.Web.Hosting.HostingEnvironment.SiteName.StartsWith("IMCMS.Web", StringComparison.CurrentCultureIgnoreCase))
                    connectionString = "Local";
                else if (System.Web.Hosting.HostingEnvironment.SiteName.EndsWith("imcrs.com", StringComparison.CurrentCultureIgnoreCase))
                    connectionString = "Staging";
                else
                    connectionString = "Live";

                ValidateConnectionString(connectionString);
                return connectionString;
            }

            if (System.Web.Hosting.HostingEnvironment.SiteName == null && HttpContext.Current == null)
            {
                var connectionStringType = "Local";
                ValidateConnectionString(connectionStringType);
                return connectionStringType;
            }

            return GetConnectionTypeByServer(new HttpContextWrapper(HttpContext.Current));
        }

        public static string GetConnectionTypeByServer(HttpContextBase context)
        {
            var returnValue = "Local";
            if (context != null && context.Request != null)
            {
                var request = context.Request;

                if (request.IsLive()) // we KNOW its a live website, instead of assuming it
                    returnValue = "Live";
                else if (request.IsStaging())
                    returnValue = "Staging";
                else if (context.IsDebuggingEnabled && request.Url != null && request.Url.Host == "localhost")
                    returnValue = "Local";
                else 
                    returnValue = "Live";
            }

            ValidateConnectionString(returnValue);
            return returnValue;
        }

        private static void ValidateConnectionString(string type)
        {
            if (ConfigurationManager.ConnectionStrings[type].ConnectionString.Contains("{database name}"))
                throw new ArgumentException("Connection string cannot contain default '{database name}'");
        }
    }
}
