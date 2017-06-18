using IMCMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMCMS.Web.ViewModels
{
    public class AdminBarViewModel
    {
        /// <summary>
        /// The URL for the Edit URL links on the admin side bar
        /// </summary>
        public string EditURL { get; set; }

        public ActiveSection? ActiveSection { get; set; }
        public int NewAppCount { get; set; }
    }

    public enum ActiveSection
    {
        Applications,
        Careers,
        Team,
        Portal,
        Forms,
        Users
    }
}