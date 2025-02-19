using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuentas_Claras_Client.Permissions
{
    public class ValidateSessionAttribute : ActionFilterAttribute
    {
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{

        //    if (HttpContext.Current.Session["user"] == null)
        //    {
        //        context.Result = new RedirectResult("/Home/Index");
        //    }

        //    base.OnActionExecuting(context);
        //}
    }
}
