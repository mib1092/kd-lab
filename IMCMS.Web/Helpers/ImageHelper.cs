using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace IMCMS.Web.Helpers
{
	public class ImageHelper : Base.IImageHelper
	{
		/// <summary>Get width and height of an image from file system.</summary>
		/// <param name="imagePath">relative image path.</param>
		/// <returns>Size structure with image dimensions.</returns>
		public Size? GetImageSize(string imagePath)
		{
			Size? size = null;
			string absolutePath = (Path.IsPathRooted(imagePath)) ? imagePath : HttpContext.Current.Server.MapPath(imagePath);

			if (File.Exists(absolutePath))
			{
				using (Image imgBitmap = Image.FromFile(absolutePath))
				{
					size = imgBitmap.Size;
				}
			}

			return size;
		}

		/// <summary>Resize an image given a specific size.</summary>
		/// <param name="imageToResize">Image to be resized.</param>
		/// <param name="size">Size to be applied.</param>
		/// <param name="resizeProportionally">Wheter or not force to resize proportionally.</param>
		/// <returns>Resized imaged</returns>
		public Image ResizeImage(Image imageToResize, Size size, bool resizeProportionally = false)
		{
			if (resizeProportionally)
			{
				float nPercentW = ((float)size.Width / (float)imageToResize.Width);
				float nPercentH = ((float)size.Height / (float)imageToResize.Height);

				float nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;

				//Replace new resize values to make it 
				size.Width = (int)(imageToResize.Width * nPercent);
				size.Height = (int)(imageToResize.Height * nPercent);
			}
			Bitmap resizedImage = new Bitmap(size.Width, size.Height);
			using (Graphics graphics = Graphics.FromImage(resizedImage))
			{
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				graphics.DrawImage(imageToResize, new Rectangle(0, 0, size.Width, size.Height), new Rectangle(0, 0, imageToResize.Width, imageToResize.Height), GraphicsUnit.Pixel);
				return resizedImage;
			}
		}

		/// <summary>Crop an image.</summary>
		/// <param name="imageToCrop">Image to be cropped.</param>
		/// <param name="cropArea">Rectangule with coordiantes to be used to crop image.</param>
		/// <returns>Cropped image.</returns>
		public Image CropImage(Image imageToCrop, Rectangle cropArea)
		{
			using (Bitmap bmpImage = new Bitmap(imageToCrop))
			{
				Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
				return bmpCrop;
			}
		}

		/// <summary>Save and image into specific path.</summary>
		/// <param name="imageToSave">Image to be saved.</param>
		/// <param name="imagePath">Path where to save image.</param>
		public void SaveImage(Image imageToSave, string imagePath)
		{
			string absolutePath = (Path.IsPathRooted(imagePath)) ? imagePath : HttpContext.Current.Server.MapPath(imagePath);
			string extension = Path.GetExtension(absolutePath) ?? "";

			ImageFormat imgFormat;
			switch (extension.ToLower())
			{
				case ".gif":
					imgFormat = ImageFormat.Gif;
					break;
				case ".jpeg":
				case ".jpg":
					imgFormat = ImageFormat.Jpeg;
					break;
				case ".png":
					imgFormat = ImageFormat.Png;
					break;
				case ".bmp":
					imgFormat = ImageFormat.Bmp;
					break;
				default:
					throw new FormatException("Format do not supported.");
			}

			imageToSave.Save(absolutePath, imgFormat);
		}

		/// <summary>Save and image into specific path.</summary>
		/// <param name="imageToSave">Image to be saved.</param>
		/// <param name="imagePath">Path where to save image.</param>
		/// <param name="qualityLevel">range between 1 - 100</param>
		public void SaveImageAsJpg(Image imageToSave, string imagePath, long qualityLevel)
		{
			if ((Path.GetExtension(imagePath) ?? "").ToLowerInvariant() != ".jpg")
				throw new ArgumentException("imagePath must specify a path with .jpg extension");

			if (qualityLevel <= 0 || qualityLevel > 100)
				throw new ArgumentException("qualityLevel must be between 1 - 100.");

			string absolutePath = (Path.IsPathRooted(imagePath)) ? imagePath : HttpContext.Current.Server.MapPath(imagePath);

			ImageCodecInfo imageCodecInfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(t => t.MimeType == "image/jpeg");

			EncoderParameters myEncoderParameters = new EncoderParameters(1);
			EncoderParameter myEncoderParameter = new EncoderParameter(Encoder.Quality, qualityLevel);
			myEncoderParameters.Param[0] = myEncoderParameter;
			imageToSave.Save(absolutePath, imageCodecInfo, myEncoderParameters);
		}

	}
}
