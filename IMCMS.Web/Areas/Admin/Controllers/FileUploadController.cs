using IMCMS.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMCMS.Web.Helpers;
using IMCMS.Common.Controllers;
using System.Text;

namespace IMCMS.Web.Areas.Admin.Controllers
{
    // does not inherit from admincontrollerbase because of issues with csrf token and ckeditor
    [Authorize]
    public class FileUploadController : Controller
    {
        List<string> acceptableExtensions = new List<string> { ".xls", ".xlsx", ".doc", ".docx", ".xml", ".pptx", ".ppt", ".docx", ".pdf", ".jpg", ".png", ".mp4", ".webm", ".ogg", ".ogv", ".psd", ".zip", ".ai" };
        string mappedDirectory = null;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            mappedDirectory = requestContext.HttpContext.Server.MapPath(Constants.FileUploadPath);
        }
        public ActionResult Upload()
        {
            var guid = Guid.NewGuid().ToString("N").Substring(0, 16);
            var fullDirectory = Path.Combine(mappedDirectory, guid);

            if (Request.Files.Count == 0)
                return null;

            var file = Request.Files[0];

            var extension = Path.GetExtension(file.FileName);

            if (!acceptableExtensions.Contains(extension))
                return new HttpStatusCodeResult(409, "File extension not allowed");

            if (!Directory.Exists(fullDirectory))
                Directory.CreateDirectory(fullDirectory);


            var newFilename = RemoveSpecialCharacters(file.FileName);

            file.SaveAs(Path.Combine(fullDirectory, newFilename));

            return Json(new { status = 0, filename = guid + "/" + newFilename, display = newFilename, fullurl = newFilename.UploadFilePath() });
        }

        [HttpPost]
        public ActionResult QuickUpload(HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                string extension = Path.GetExtension(upload.FileName);

                if (acceptableExtensions.Contains(extension))
                {
                    var guid = Guid.NewGuid().ToString("N").Substring(0, 16);

                    var folderPath = Path.Combine(mappedDirectory, guid);

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string cleanFileName = RemoveSpecialCharacters(upload.FileName);
                    upload.SaveAs(Path.Combine(folderPath, cleanFileName));

                    return
                        Content(String.Format(
                            "<script>window.parent.CKEDITOR.tools.callFunction( {0}, '{1}');</script>",
                            Request.QueryString["CKEditorFuncNum"], Constants.FileUploadPath.Trim('~') + "/" + guid + "/" + cleanFileName));
                }
                else
                {
                    return
                    Content(String.Format("<script>window.parent.CKEDITOR.tools.callFunction( {0}, '', '{1}');</script>",
                        Request.QueryString["CKEditorFuncNum"], "Sorry that is a permitted file extension. Allowed extensions are " + String.Join(", ", acceptableExtensions)));
                }
            }

            return
                    Content(String.Format("<script>window.parent.CKEDITOR.tools.callFunction( {0}, '', '{1}');</script>",
                        Request.QueryString["CKEditorFuncNum"], "Unexpected issue with file upload."));
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

    }
}
