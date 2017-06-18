using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMCMS.Web
{
    using System.Web.Mvc;

    public static class SelectList
    {
        public static List<SelectListItem> Departments = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "Leadership", Value = "Leadership" },
            new SelectListItem() { Text = "Superintendent", Value = "Superintendent" }
        };

        public static List<SelectListItem> States = new List<SelectListItem>()
    {
        new SelectListItem() {Text="Alabama", Value="AL"},
        new SelectListItem() { Text="Alaska", Value="AK"},
        new SelectListItem() { Text="Arizona", Value="AZ"},
        new SelectListItem() { Text="Arkansas", Value="AR"},
        new SelectListItem() { Text="California", Value="CA"},
        new SelectListItem() { Text="Colorado", Value="CO"},
        new SelectListItem() { Text="Connecticut", Value="CT"},
        new SelectListItem() { Text="District of Columbia", Value="DC"},
        new SelectListItem() { Text="Delaware", Value="DE"},
        new SelectListItem() { Text="Florida", Value="FL"},
        new SelectListItem() { Text="Georgia", Value="GA"},
        new SelectListItem() { Text="Hawaii", Value="HI"},
        new SelectListItem() { Text="Idaho", Value="ID"},
        new SelectListItem() { Text="Illinois", Value="IL"},
        new SelectListItem() { Text="Indiana", Value="IN"},
        new SelectListItem() { Text="Iowa", Value="IA"},
        new SelectListItem() { Text="Kansas", Value="KS"},
        new SelectListItem() { Text="Kentucky", Value="KY"},
        new SelectListItem() { Text="Louisiana", Value="LA"},
        new SelectListItem() { Text="Maine", Value="ME"},
        new SelectListItem() { Text="Maryland", Value="MD"},
        new SelectListItem() { Text="Massachusetts", Value="MA"},
        new SelectListItem() { Text="Michigan", Value="MI"},
        new SelectListItem() { Text="Minnesota", Value="MN"},
        new SelectListItem() { Text="Mississippi", Value="MS"},
        new SelectListItem() { Text="Missouri", Value="MO"},
        new SelectListItem() { Text="Montana", Value="MT"},
        new SelectListItem() { Text="Nebraska", Value="NE"},
        new SelectListItem() { Text="Nevada", Value="NV"},
        new SelectListItem() { Text="New Hampshire", Value="NH"},
        new SelectListItem() { Text="New Jersey", Value="NJ"},
        new SelectListItem() { Text="New Mexico", Value="NM"},
        new SelectListItem() { Text="New York", Value="NY"},
        new SelectListItem() { Text="North Carolina", Value="NC"},
        new SelectListItem() { Text="North Dakota", Value="ND"},
        new SelectListItem() { Text="Ohio", Value="OH"},
        new SelectListItem() { Text="Oklahoma", Value="OK"},
        new SelectListItem() { Text="Oregon", Value="OR"},
        new SelectListItem() { Text="Pennsylvania", Value="PA"},
        new SelectListItem() { Text="Rhode Island", Value="RI"},
        new SelectListItem() { Text="South Carolina", Value="SC"},
        new SelectListItem() { Text="South Dakota", Value="SD"},
        new SelectListItem() { Text="Tennessee", Value="TN"},
        new SelectListItem() { Text="Texas", Value="TX"},
        new SelectListItem() { Text="Utah", Value="UT"},
        new SelectListItem() { Text="Vermont", Value="VT"},
        new SelectListItem() { Text="Virginia", Value="VA"},
        new SelectListItem() { Text="Washington", Value="WA"},
        new SelectListItem() { Text="West Virginia", Value="WV"},
        new SelectListItem() { Text="Wisconsin", Value="WI"},
        new SelectListItem() { Text="Wyoming", Value="WY"}
    };

        public static List<SelectListItem> Months = new List<SelectListItem>()
        {
            new SelectListItem { Text = "1 - January", Value = "1" },
            new SelectListItem { Text = "2 - February", Value = "2" },
            new SelectListItem { Text = "3 - March", Value = "3" },
            new SelectListItem { Text = "4 - April", Value = "4" },
            new SelectListItem { Text = "5 - May", Value = "5" },
            new SelectListItem { Text = "6 - June", Value = "6" },
            new SelectListItem { Text = "7 - July", Value = "7" },
            new SelectListItem { Text = "8 - August", Value = "8" },
            new SelectListItem { Text = "9 - September", Value = "9" },
            new SelectListItem { Text = "10 - October", Value = "10" },
            new SelectListItem { Text = "11 - November", Value = "11" },
            new SelectListItem { Text = "12 - December", Value = "12" },
        };

        public static List<SelectListItem> Days
        {
            get
            {
                return Enumerable.Range(1, 31).Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();
            }
        }

        public static List<SelectListItem> Years
        {
            get
            {
                return Enumerable.Range(DateTime.Now.Year - 80, 80).Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();
            }
        }
    }
}