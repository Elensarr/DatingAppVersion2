using API.Extentions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        // what passes context and next?
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {


            //??
            var resultContext = await next();
            
            // how we get there 
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return;
            }

            var userId = resultContext.HttpContext.User.GetUserId();
            //what is repo?
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            
            var user = await repo.GetUserByIdAsync(userId);

            user.LastActive = DateTime.Now;

            await repo.SaveAllAsync();
        }
    }
}
