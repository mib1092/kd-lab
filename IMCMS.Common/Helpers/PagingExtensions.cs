using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace IMCMS.Common.Helpers
{
    public static class PagingExtensions
    {
        public static MvcHtmlString Pager(this HtmlHelper helper, int currentPage, int pageSize, int totalItemCount, object routeValues, string containerClass)
        {
            // how many pages to display in each page group const  	
            int cGroupSize = 5;
            var pageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);

            // cleanup any out bounds page number passed  	
            currentPage = Math.Max(currentPage, 1);
            currentPage = Math.Min(currentPage, pageCount);

            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
            var container = new TagBuilder("div");
            if (containerClass != null)
                container.AddCssClass(containerClass);
            else
                container.AddCssClass("pager");

            var actionName = helper.ViewContext.RouteData.GetRequiredString("Action");

            // calculate the last page group number starting from the current page  	
            // until we hit the next whole divisible number  	
            var lastGroupNumber = currentPage;
            while ((lastGroupNumber % cGroupSize != 0)) lastGroupNumber++;

            // correct if we went over the number of pages  	
            var groupEnd = Math.Min(lastGroupNumber, pageCount);

            // work out the first page group number, we use the lastGroupNumber instead of  	
            // groupEnd so that we don't include numbers from the previous group if we went  	
            // over the page count  	
            var groupStart = lastGroupNumber - (cGroupSize - 1);

            if (pageCount == 1 || totalItemCount == 0) return MvcHtmlString.Create(string.Empty);

            // if we are past the first page  	
            if (currentPage > 1)
            {
                var previous = new TagBuilder("a");
                previous.SetInnerText("<< Previous");
                previous.AddCssClass("previous");
                var routingValues = new RouteValueDictionary(routeValues);
                routingValues.Add("p", currentPage - 1);
                previous.MergeAttribute("href", urlHelper.Action(actionName, routingValues));
                container.InnerHtml += previous.ToString();
            }

            // if we have past the first page group  	
            if (currentPage > cGroupSize)
            {
                var firstPage = new TagBuilder("a");
                firstPage.SetInnerText("1");
                firstPage.AddCssClass("pn");
                var routingValues = new RouteValueDictionary(routeValues);
                routingValues.Add("p", 1);
                firstPage.MergeAttribute("href", urlHelper.Action(actionName, routingValues));
                container.InnerHtml += firstPage.ToString();

                var previousDots = new TagBuilder("p");
                previousDots.SetInnerText("...");
                container.InnerHtml += previousDots.ToString();
            }

            for (var i = groupStart; i <= groupEnd; i++)
            {
                var pageNumber = new TagBuilder("a");
                pageNumber.AddCssClass("pn");
                pageNumber.AddCssClass(((i == currentPage)) ? "current" : String.Empty);
                pageNumber.SetInnerText((i).ToString());
                var routingValues = new RouteValueDictionary(routeValues);
                routingValues.Add("p", i);
                pageNumber.MergeAttribute("href", urlHelper.Action(actionName, routingValues));
                container.InnerHtml += pageNumber.ToString();
            }

            // if there are still pages past the end of this page group  	
            if (pageCount > groupEnd)
            {
                var nextDots = new TagBuilder("p");
                nextDots.SetInnerText("...");
                container.InnerHtml += nextDots.ToString();

                var lastPage = new TagBuilder("a");
                lastPage.SetInnerText((pageCount).ToString());
                lastPage.AddCssClass("pn");
                var routingValues = new RouteValueDictionary(routeValues);
                routingValues.Add("p", pageCount);
                lastPage.MergeAttribute("href", urlHelper.Action(actionName, routingValues));
                container.InnerHtml += lastPage.ToString();
            }

            // if we still have pages left to show  	
            if (currentPage < pageCount)
            {
                var next = new TagBuilder("a");
                next.SetInnerText("next >>");
                next.AddCssClass("next");
                var routingValues = new RouteValueDictionary(routeValues);
                routingValues.Add("p", currentPage + 1);
                next.MergeAttribute("href", urlHelper.Action(actionName, routingValues));
                container.InnerHtml += next.ToString();
            }

            return MvcHtmlString.Create(container.ToString());
        }
    }
}
