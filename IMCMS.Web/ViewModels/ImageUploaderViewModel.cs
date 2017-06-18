using System;

namespace IMCMS.Web.ViewModels
{
    public class ImageUploaderViewModel
    {
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public int MinWidth { get; set; }
        public int MinHeight { get; set; }
        public int MaxSize { get; set; }
        public string MaxSizeFormattedMB
        {
            get
            {
                return Math.Round(MaxSize / (double)1024, 1) + "MB";
            }
        }
    }
}