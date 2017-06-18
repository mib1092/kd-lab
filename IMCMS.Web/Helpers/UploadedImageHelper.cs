using System;
using System.Web.Mvc;
using IMCMS.Models.Entities;

namespace IMCMS.Web.Helpers
{
	public static class UploadedImageHelper
	{
		public static string GetUploadedImagePath(this HtmlHelper html, ImageSizes size, Photo p)
		{
			if (p == null)
				return String.Empty;

			string folder = size.ToString();
			string extension = folder.IndexOf("png", StringComparison.OrdinalIgnoreCase) != -1 ? ".png" : ".jpg";

			return "/assets/images/" + folder + "/" + p.FileGuid + extension;
		}

		public static string GetUploadedImagePath(this Photo p, ImageSizes size)
		{
			if (p == null)
				return String.Empty;

			string folder = size.ToString();
			string extension = folder.IndexOf("png", StringComparison.OrdinalIgnoreCase) != -1 ? ".png" : ".jpg";

			return "/assets/images/" + folder + "/" + p.FileGuid + extension;
		}

	}
}