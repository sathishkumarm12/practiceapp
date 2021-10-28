using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.GetUserId();
            var userRepo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await userRepo.GetUserByIdAsync(userId);

            user.LastActive = DateTime.Now;

            userRepo.Update(user);
            await userRepo.SaveAllAsync();
        }
    }
}