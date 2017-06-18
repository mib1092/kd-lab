using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMCMS.Web
{
    public enum ImageSizes
    {
        [PhotoSize("width=200&height=200&crop=auto&format=jpg&quality=80")]
        Crop_200x200,
        [PhotoSize("maxwidth=410&maxheight=410&format=jpg&quality=80")]
        NoCrop_410x410
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class PhotoSize : Attribute
    {
        public PhotoSize(string command)
        {
            this.Command = command;
        }

        public string Command;
    }
}