using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using IMCMS.Common.Helpers;
using IMCMS.Web.ViewModels;
using IMCMS.Web.Helpers;

namespace IMCMS.Web.Controllers
{
	[Authorize]
	public class ImageUploaderController : Controller
	{
		private const string UPLOAD_PATH = "/uploads/images/";
		private readonly ImageHelper imageHelper;

		/// <summary>Constructor with dependencies</summary>
		/// <param name="imageHelper"></param>
		public ImageUploaderController(ImageHelper imageHelper)
		{
			this.imageHelper = imageHelper;
		}

		/// <summary>Default Constructor</summary>
		public ImageUploaderController() : this(new ImageHelper()) { }

		//
		// GET: /ImageUploader/

		public ActionResult Index(uint? maxWidth, uint? maxHeight, uint maxSize)
		{
			ImageUploaderViewModel viewModel = new ImageUploaderViewModel()
			{
				MaxWidth = (int?)maxWidth ?? 750,
				MaxHeight = (int?)maxHeight ?? 500,
				MinWidth = 24,
				MinHeight = 24,
				MaxSize = (int?)maxSize ?? 4096
			};
			return View(viewModel);
		}

		// POST: /ImageUploader/UploadFile
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult UploadFile(string name, int maxWidth, int maxHeight)
		{
			if (Request.Files.Count == 1 && Request.Files[0] != null)
			{
				try
				{
					HttpPostedFileBase file = Request.Files[0];
					string path = Server.MapPath(UPLOAD_PATH);
					if (!Directory.Exists(path))
						Directory.CreateDirectory(path);

					string alternativeText = file.FileName;
					string extension = Path.GetExtension(file.FileName) ?? "";
					string fileName = String.Format("{0}{1}", DateTime.Now.ToString("yyMMddHHmmss"), extension);
					string filePath = Path.Combine(path, fileName);
					file.SaveAs(filePath);

					Size? size = imageHelper.GetImageSize(filePath);
					if (!size.HasValue || size.Value.IsEmpty)
					{
						if (System.IO.File.Exists(filePath))
							System.IO.File.Delete(filePath);

						return Json(new { ErrorMessage = "File is not an image" });
					}
					//resize to max width and/or height (This is needed for webkit based browsers that will not upload already resized images).
					if (size.Value.Width > maxWidth || size.Value.Height > maxHeight)
					{
						//Save a copy of original file before resizing to keep original resolution for future cropping //_orig
						file.SaveAs(Path.Combine(path, fileName.Replace(extension, "") + "_orig" + extension));
						size = ResizeProportionallyImage(filePath, maxWidth, maxHeight);
					}
					return Json(new { filePath = Path.Combine(UPLOAD_PATH, fileName), alternativeText, height = size.Value.Height, width = size.Value.Width });
				}
				catch (Exception e)
				{
					Elmah.ErrorSignal.FromCurrentContext().Raise(e);
					return Json(new { ErrorMessage = e.Message });
				}
			}

			return Json(new { ErrorMessage = "No file was uploaded" });
		}

		// POST: /ImageUploader/DeleteFile
		[HttpPost]
		public ActionResult DeleteFile(string filePath)
		{
			try
			{
				Uri uri;
				if (IsExistingLocalFile(filePath) && Uri.TryCreate(filePath, UriKind.RelativeOrAbsolute, out uri))
				{
					filePath = Server.MapPath((!uri.IsAbsoluteUri) ? filePath : uri.PathAndQuery);
					string extension = Path.GetExtension(filePath) ?? "";
					string cropppedFilePath = filePath.Replace(extension, "") + "_crop" + extension;
					string originalFilePath = filePath.Replace(extension, "") + "_orig" + extension;

					if (System.IO.File.Exists(filePath))
						System.IO.File.Delete(filePath);

					if (System.IO.File.Exists(cropppedFilePath))
						System.IO.File.Delete(cropppedFilePath);

					if (System.IO.File.Exists(originalFilePath))
						System.IO.File.Delete(originalFilePath);
				}
				return Json(new { Success = true });
			}
			catch (Exception e)
			{
				Elmah.ErrorSignal.FromCurrentContext().Raise(e);
				return Json(new { ErrorMessage = e.Message });
			}
		}

		// POST: /ImageUploader/CropFile
		[HttpPost]
		public ActionResult CropFile(string filePath, int maxWidth, int maxHeight, int currentWidth, int currentHeight, int jcropX, int jcropY, int jcropW, int jcropH, bool overwrite)
		{
			try
			{
				string hostBaseUrl = ConfigurationManager.AppSettings["HostBaseUrl"];
				if (!IsUrlAnInternalExistingFile(filePath, new[] { hostBaseUrl }))
					throw new ArgumentException("File to crop is not local to the server");

				Uri uri;
				if (Uri.TryCreate(filePath, UriKind.RelativeOrAbsolute, out uri))
				{
					string path = Server.MapPath(UPLOAD_PATH);
					if (!Directory.Exists(path))
						Directory.CreateDirectory(path);

					if (uri.IsAbsoluteUri)
						filePath = uri.PathAndQuery;
					string extension = Path.GetExtension(filePath) ?? "";
					string serverPath = Server.MapPath(filePath);
					string fileName = Path.GetFileName(filePath);
					string croppedFilePath = UPLOAD_PATH + fileName.Replace(extension, "") + "_crop.jpg";
					if (!overwrite)
					{
						int sequence = 1;
						while (System.IO.File.Exists(Server.MapPath(croppedFilePath)))
						{
							croppedFilePath = UPLOAD_PATH + fileName.Replace(extension, "") + "_crop" + (sequence++) + ".jpg";
						}
					}
					string serverCroppedFilePath = Server.MapPath(croppedFilePath);

					//Test if original uploaded file exists to crop image from it.
					string originalPath = Server.MapPath(filePath.Replace(extension, "") + "_orig" + extension);
					bool cropFromOriginal = false;
					if (System.IO.File.Exists(originalPath))
					{
						Size? originalSize = imageHelper.GetImageSize(originalPath);
						if (originalSize.HasValue)
						{
							double propWidth = (double)originalSize.Value.Width / currentWidth;
							double propHeight = (double)originalSize.Value.Height / currentHeight;

							//set new values of x,y,w,h, current w and h according original image.
							jcropX = Round(jcropX * propWidth);
							jcropY = Round(jcropY * propHeight);
							jcropW = Round(jcropW * propWidth);
							jcropH = Round(jcropH * propHeight);
							currentWidth = originalSize.Value.Width;
							currentHeight = originalSize.Value.Height;
							serverPath = originalPath;
							cropFromOriginal = true;
						}
					}
					if (System.IO.File.Exists(serverPath))
					{
						using (Image imgBitmap = Image.FromFile(serverPath))
						{
							Image resizedImage = imageHelper.ResizeImage(imgBitmap, new Size(currentWidth, currentHeight));

							if (jcropW > 0 && jcropH > 0)
							{
								Image croppedImage = imageHelper.CropImage(resizedImage, new Rectangle(jcropX, jcropY, jcropW, jcropH));
								resizedImage.Dispose();
								resizedImage = croppedImage;
							}

							//Cropped images always saved as jpg with max quality
							imageHelper.SaveImageAsJpg(resizedImage, serverCroppedFilePath, 100);
							resizedImage.Dispose();
						}
						if (cropFromOriginal)
						{
							//Only if image was cropped from original uploaded image.
							Size? croppedSize = imageHelper.GetImageSize(serverCroppedFilePath);
							if (!croppedSize.HasValue || croppedSize.Value.IsEmpty)
							{
								if (System.IO.File.Exists(serverCroppedFilePath))
									System.IO.File.Delete(serverCroppedFilePath);

								return Json(new { ErrorMessage = "Could not retrieve the size of cropped image" });
							}
							//If needed resize cropped image to max width and/or height.
							if (croppedSize.Value.Width > maxWidth || croppedSize.Value.Height > maxHeight)
							{
								ResizeProportionallyImage(serverCroppedFilePath, maxWidth, maxHeight);
							}
						}
						return Json(new { croppedFilePath });
					}
					throw new ArgumentException("File to crop does not exists");
				}
				throw new ArgumentException("filepath could not be created as relative or absolute path.");
			}
			catch (Exception e)
			{
				Elmah.ErrorSignal.FromCurrentContext().Raise(e);
				return Json(new { ErrorMessage = e.Message });
			}
		}

		// POST: /ImageUploader/IsLocalFile
		[HttpPost]
		public ActionResult IsLocalFile(string filePath)
		{
			return Json(IsExistingLocalFile(filePath));
		}

		// POST: /ImageUploader/FinalProcessing
		[HttpPost]
		public ActionResult FinalProcessing(string filePath, int width, int height)
		{
			try
			{
				Uri uri;
				if (IsExistingLocalFile(filePath) && Uri.TryCreate(filePath, UriKind.RelativeOrAbsolute, out uri))
				{
					string absolutePath = Server.MapPath((!uri.IsAbsoluteUri) ? filePath : uri.PathAndQuery);
					Image resizedImage;
					using (Image imgBitmap = Image.FromFile(absolutePath))
					{
						resizedImage = imageHelper.ResizeImage(imgBitmap, new Size(width, height), true);
					}
					string finalImageName = String.Format("{0}.jpg", DateTime.Now.ToString("yyMMddHHmmss"));
					filePath = UPLOAD_PATH + finalImageName;
					absolutePath = Server.MapPath(filePath);
					//Final images always saved as jpg with average quality
					imageHelper.SaveImageAsJpg(resizedImage, absolutePath, 80);
				}
				return Json(new { finalImageName = filePath });
			}
			catch (Exception e)
			{
				Elmah.ErrorSignal.FromCurrentContext().Raise(e);
				return Json(new { ErrorMessage = e.Message });
			}
		}

		/// <summary>Resize proportionally an image to specific width and height and save in the same path.</summary>
		/// <param name="filePath">Absolute file path of image to be resized.</param>
		/// <param name="width">Width to be resized to.</param>
		/// <param name="height">Height to be resized to.</param>
		/// <returns>Size of resized image.</returns>
		private Size ResizeProportionallyImage(string filePath, int width, int height)
		{
			Image resizedImage;
			using (Image imgBitmap = Image.FromFile(filePath))
			{
				resizedImage = imageHelper.ResizeImage(imgBitmap, new Size(width, height), true);
			}
			imageHelper.SaveImage(resizedImage, filePath);
			resizedImage.Dispose();
			Size? size = imageHelper.GetImageSize(filePath);
			if (!size.HasValue)
				throw new ApplicationException("Could not get image size after resizing it.");
			return size.Value;
		}

		/// <summary>Round a double value to integer.  Ex: 1.5 -> 2,  1.2 -> 1,  1.6 -> 2 </summary>
		/// <param name="value">double value to be rounded</param>
		/// <returns>Rounded integer</returns>
		private static int Round(double value)
		{
			return Convert.ToInt32(Math.Round(value, 0, MidpointRounding.AwayFromZero));
		}

		/// <summary>Determines whether or not a file path is for file that exists in the site.</summary>
		/// <param name="filePath">Path to test.</param>
		/// <returns></returns>
		private bool IsExistingLocalFile(string filePath)
		{
			string hostBaseUrl = ConfigurationManager.AppSettings["HostBaseUrl"];
			return IsUrlAnInternalExistingFile(filePath, new[] { hostBaseUrl });
		}

        /// <summary>Determines if a url is internal using an array of local domains.</summary>
        /// <param name="str">The source text to verify. It should be an absolute url to verify with local domains, otherwise always will return true since it is relative.</param>
        /// <param name="localDomains">Array of domains to verify if url is inside of.</param>
        /// <returns>True if url provided is internal and match with one of the local domains specified in the array.</returns>
        private bool IsUrlInternal(string url, string[] localDomains)
        {
            if (localDomains == null || localDomains.Length == 0)
                throw new ArgumentNullException("localDomains", "domains array cannot be null or empty.");
            if (String.IsNullOrEmpty(url))
                return false;

            Uri uri;
            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
            {
                return !uri.IsAbsoluteUri || localDomains.Any(domain => domain.IndexOf(uri.Host, StringComparison.InvariantCultureIgnoreCase) != -1);
            }
            return false;
        }

        /// <summary>Determines if a url is an internal file that exists using an array of local domains.</summary>
        /// <param name="str">The source text to verify. local domains will only be used if url is absolute, otherwise always it is asumed to be internal since it is relative.</param>
        /// <param name="localDomains">Array of domains to verify if url is inside of.</param>
        /// <returns>True if url is an file that exists in local domains.</returns>
        private bool IsUrlAnInternalExistingFile(string url, string[] localDomains)
        {
            if (localDomains == null || localDomains.Length == 0)
                throw new ArgumentNullException("localDomains", "domains array cannot be null or empty.");
            if (String.IsNullOrEmpty(url))
                return false;

            if (IsUrlInternal(url, localDomains))
            {
                Uri uri;
                if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
                {
                    string serverPath = Server.MapPath((!uri.IsAbsoluteUri) ? url : uri.PathAndQuery);
                    return System.IO.File.Exists(serverPath);
                }
                throw new ArgumentException("input string was internal, but it could not be created as relative or absolute path.");
            }
            return false;
        }
	}
}
