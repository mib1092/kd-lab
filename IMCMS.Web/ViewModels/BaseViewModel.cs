using System.Collections.Generic;
using IMCMS.Models.Entities;

namespace IMCMS.Web.ViewModels
{
	/// <summary>
	/// Base view model that should house all information to build a view
	/// </summary>
	public class BaseViewModel
	{
		public BaseViewModel()
		{
			// here to prevent null reference exceptions
			AdminBar = new AdminBarViewModel();
		}

		/// <summary>
		/// View model for the admin bar
		/// </summary>
		public AdminBarViewModel AdminBar { get; set; }
	}

	/// <summary>
	/// Base view model that should house all information to build a view with an item
	/// </summary>
	/// <typeparam name="T">Type of item you are displaying</typeparam>
	public class BaseViewModel<T> : BaseViewModel
	{
		/// <summary>
		/// The item you're displaying
		/// </summary>
		public T Item { get; set; }
	}
}