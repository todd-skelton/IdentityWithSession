using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityWithSession
{
    public class ValidSessionHandler : AuthorizationHandler<ValidSessionRequirement>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ValidSessionHandler(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidSessionRequirement requirement)
        {
            // if the user isn't authenticated then no need to check session
            if (!context.User.Identity.IsAuthenticated)
                return;

            // get the user and session claim
            var user = await _userManager.GetUserAsync(context.User);

            var claims = await _userManager.GetClaimsAsync(user);

            var serverSession = claims.First(e => e.Type == "session");

            var clientSession = context.User.FindFirst("session");

            // if the client session matches the server session then the user is authorized
            if (serverSession?.Value == clientSession?.Value)
            {
                context.Succeed(requirement);
            }
            return;
        }
    }
}
