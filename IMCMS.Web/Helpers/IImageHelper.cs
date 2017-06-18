using System.Drawing;

namespace IMCMS.Web.Helpers.Base
{
	public interface IImageHelper
	{
		/// <summary>Get width and height of an image from file system.</summary>
		/// <param name="imagePath">relative image path.</param>
		/// <returns>Size structure with image dimensions.</returns>
		Size? GetImageSize(string imagePath);

		/// <summary>Resize an image given a specific size.</summary>
		/// <param name="imageToResize">Image to be resized.</param>
		/// <param name="size">Size to be applied.</param>
		/// <param name="resizeProportionally">Wheter or not force to resize proportionally.</param>
		/// <returns>Resized imaged</returns>
		Image ResizeImage(Image imageToResize, Size size, bool resizeProportionally = false);

		/// <summary>Crop an image.</summary>
		/// <param name="imageToCrop">Image to be cropped.</param>
		/// <param name="cropArea">Rectangule with coordiantes to be used to crop image.</param>
		/// <returns>Cropped image.</returns>
		Image CropImage(Image imageToCrop, Rectangle cropArea);

		/// <summary>Save and image into specific path.</summary>
		/// <param name="imageToSave">Image to be saved.</param>
		/// <param name="imagePath">Path where to save image.</param>
		void SaveImage(Image imageToSave, string imagePath);
	}
}
