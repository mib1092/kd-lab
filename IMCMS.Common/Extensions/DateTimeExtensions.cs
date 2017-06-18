using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMCMS.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToOrdinal(int number)
        {
            switch (number % 100)
            {
                case 11:
                case 12:
                case 13:
                    return number.ToString() + "th";
            }

            switch (number % 10)
            {
                case 1:
                    return number.ToString() + "st";
                case 2:
                    return number.ToString() + "nd";
                case 3:
                    return number.ToString() + "rd";
                default:
                    return number.ToString() + "th";
            }
        }
    }
}
