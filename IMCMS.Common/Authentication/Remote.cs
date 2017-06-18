using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Configuration;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;

namespace IMCMS.Common.Authentication
{
    using System.Web;

    public sealed class Remote
    {
        public static void PingAuthentication()
        {
            WebRequest request = HttpWebRequest.Create("https://auth.imagemakers-inc.com/api/ping?version=2");
            ServicePointManager.ServerCertificateValidationCallback += ((sender, certificate, chain, sslPolicyErrors) => true);
            request.Method = "GET";
            request.BeginGetResponse(new AsyncCallback(ResponseReceived), null);
        }

        private static void ResponseReceived(IAsyncResult result)
        {
            return;
        }

        public static RemoteAuthenicationResponse Authenticate(string Username, string Password, string IP = "")
        {
            Configuration.CentralAuthElement ConfigAuth = Configuration.Config.ConfigurationSection.CentralAuth;
            if (String.IsNullOrEmpty(ConfigAuth.ID) || String.IsNullOrEmpty(ConfigAuth.Secret))
                return RemoteAuthenicationResponse.NotConfigured;

            StringBuilder postData = new StringBuilder();
            postData.Append("token=" + HttpUtility.UrlEncode(ConfigAuth.Secret) + "&");
            postData.Append("username=" + HttpUtility.UrlEncode(Username) + "&");
            postData.Append("password=" + HttpUtility.UrlEncode(Password) + "&");
            postData.Append("userip=" + HttpUtility.UrlEncode(IP));

            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] postBytes = ascii.GetBytes(postData.ToString());

            WebRequest request = WebRequest.Create("https://webauth.imagemakers-inc.com/adminauth/api/v3");
            request.Method = "POST";
            request.Timeout = 2000; 
            request.ContentLength = postBytes.Length;
            request.ContentType = "application/x-www-form-urlencoded";

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            StreamReader reader;

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
            }
            catch (Exception)
            {
                // if for whatever reason, we could not a response, tell the calling function we were unable to complete it
                // this should tell the calling function to use another authenciation method if it supports it
                return RemoteAuthenicationResponse.UnableToComplete;
            }

            XDocument xml = XDocument.Parse(reader.ReadToEnd());
            XElement xe = xml.Element("auth");
            
            // if the error element even exists, fail as invalid
            if (xe.Element("error") != null)
            {
                // checks error code to see if the code returned is an IP block
                if (xe.Element("code") != null)
                {
                    if (xe.Element("code").Value == "6")
                        return RemoteAuthenicationResponse.IPLocked;
                }
                return RemoteAuthenicationResponse.Invalid;
            }

            return bool.Parse(xe.Element("authenticated").Value) ? RemoteAuthenicationResponse.Valid : RemoteAuthenicationResponse.Invalid;
        }
    }
}
