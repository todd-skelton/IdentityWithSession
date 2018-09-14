using IdentityWithSession.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityWithSession
{
    public class ValidSessionHandler : AuthorizationHandler<ValidSessionRequirement>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ValidSessionHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidSessionRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
                return;

            var user = await _userManager.GetUserAsync(context.User);

            var claims = await _userManager.GetClaimsAsync(user);

            var serverSession = claims.First(e => e.Type == "session");

            var clientSession = context.User.FindFirst("session");

            if (serverSession?.Value == clientSession?.Value)
            {
                context.Succeed(requirement);
            }

            return;
        }
    }
}
