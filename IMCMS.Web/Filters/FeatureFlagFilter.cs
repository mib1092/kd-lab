using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMCMS.Web
{
    public class RequireFeatureFlagAttribute : ActionFilterAttribute
    {
        Conditions _requiredConditions;
        public RequireFeatureFlagAttribute(Conditions requiredCondition)
        {
            _requiredConditions = requiredCondition;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(!Features.IsAvailable(_requiredConditions))
                filterContext.Result = new HttpUnauthorizedResult();

            base.OnActionExecuting(filterContext);
        }
    }
}