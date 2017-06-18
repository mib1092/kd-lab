using IMCMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMCMS.Web.Helpers
{
    public static class FileUploadHelper
    {
        /// <summary>
        /// Returns the absolute HTTP path of a file upload
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="filename">File name that is stored in the database</param>
        /// <returns></returns>
        public static string UploadFilePath(this HtmlHelper html, string filename)
        {
            if (String.IsNullOrEmpty(filename))
                return filename;

            return VirtualPathUtility.ToAbsolute(String.Format("{0}/{1}", Constants.FileUploadPath, filename));
        }

        public static string UploadFilePath(this String input)
        {
            if (String.IsNullOrEmpty(input))
                return input;

            return VirtualPathUtility.ToAbsolute(String.Format("{0}/{1}", Constants.FileUploadPath, input));
        }
    }
}