using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SportDomain.Identity;

namespace ProjectName.Middleware
{
    public class UserAgreementMiddleware
    {
        private readonly RequestDelegate _next;

        public UserAgreementMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<BetUser> userManager)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var user = await userManager.GetUserAsync(context.User);
                if (user != null && !user.HasAcceptedUserAgreement)
                {
                    if (!context.Request.Path.StartsWithSegments("/Identity/Account/Manage/UserAgreement"))
                    {
                        context.Response.Redirect("/Identity/Account/Manage/UserAgreement");
                        return;
                    }
                }
            }
            await _next(context);
        }
    }
}
