using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ImageResizer;
using IMCMS.Common.Controllers;
using IMCMS.Models.DAL;
using IMCMS.Models.Entities;
using System.Reflection;
using StackExchange.Profiling;

namespace IMCMS.Web.Areas.Admin.Controllers
{
   
    public class PhotosController : BaseAdminController
    {
        public PhotosController(IUnitOfWork UOW) : base(UOW)
        {
        }


        public ActionResult Upload()
        {
            var file = Request.Files[0];

            string FileHash = String.Empty;

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file.InputStream);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            FileHash = sb.ToString();



            Photo photo = _uow.Photos.FirstOrDefault(d => d.Hash == FileHash);
            if (photo != null)
            {
                string Path =
                    Server.MapPath(
                        "~/assets/images/og/" + photo.FileGuid + System.IO.Path.GetExtension(photo.OriginalFilename));

                if (!System.IO.File.Exists(Path)) // if the orignial file wasn't saved or was removed, resave it
                {
                    SaveOriginal(file, photo.FileGuid);
                }

                SaveSizes(Path);
                return Json(new { status = 0, ID = photo.ID, filename = photo.FileGuid });
            }
            else
            {
                SavedOriginal info = SaveOriginal(file);

                using (MiniProfiler.Current.Step("Resizing"))
                {
                    SaveSizes(info.Path);
                }

                Photo newPhoto = null;

                newPhoto = new Photo
                    {
                        FileGuid = info.Guid,
                        Hash = FileHash,
                        OriginalFilename = file.FileName
                    };

                _uow.Photos.Add(newPhoto);
                _uow.Commit();


                return Json(new { status = 0, ID = newPhoto.ID, filename = info.Guid });
            }
        }

        public ActionResult ResizeAllImages()
        {
            string[] files = Directory.GetFiles(Server.MapPath("~/assets/images/og/"));

            foreach (String file in files)
            {
                SaveSizes(file);
            }

            return Json(new { status = 0 });
        }

        protected void SaveSizes(string Path)
        {
            var values = Enum.GetValues(typeof(ImageSizes));

            foreach (ImageSizes ps in values)
            {
                
                    Type type = ps.GetType();
                    var info = type.GetMember(ps.ToString());
                    PhotoSize attr = info[0].GetCustomAttribute<PhotoSize>();

                    if (!Directory.Exists(Server.MapPath("~/assets/images/" + ps.ToString())))
                        Directory.CreateDirectory(Server.MapPath("~/assets/images/" + ps.ToString()));


                    string ImageToBePath = Server.MapPath("~/assets/images/" + ps.ToString() + "/" + System.IO.Path.GetFileNameWithoutExtension(Path));
                if (!System.IO.File.Exists(ImageToBePath))
                    ImageBuilder.Current.Build(new ImageJob(Path, ImageToBePath, new Instructions(attr.Command), false, true));
                
            }
        }


        /// <summary>
        /// Saves the original to the file system
        /// </summary>
        /// <param name="file">The uploaded image file</param>
        /// <returns>Mapped path to the saved original file</returns>
        protected SavedOriginal SaveOriginal(HttpPostedFileBase file, string uid = null)
        {
            string FileNameGuid;

            if (uid == null)
                FileNameGuid = Guid.NewGuid().ToString("N"); // prevent collisions with a guid, and encode the file names to prevent injection
            else
                FileNameGuid = uid;

            if (!Directory.Exists(Server.MapPath("~/assets/images/og/")))
                Directory.CreateDirectory(Server.MapPath(("~/assets/images/og/")));

            string Path = Server.MapPath("~/assets/images/og/" + FileNameGuid + System.IO.Path.GetExtension(file.FileName));

            file.SaveAs(Path);

            return new SavedOriginal { Guid = FileNameGuid, Path = Path };
        }

    }
    public struct SavedOriginal
    {
        public string Guid { get; set; }
        public string Path { get; set; }
    }
}
