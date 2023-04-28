using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAppPrac.Logs
{
    public class AopExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            StaticLogger.Error(context.Exception);
            base.OnException(context);
        }

        public override Task OnExceptionAsync(ExceptionContext context)
        {
            StaticLogger.Error(context.Exception);
            return base.OnExceptionAsync(context);
        }
    }
}
