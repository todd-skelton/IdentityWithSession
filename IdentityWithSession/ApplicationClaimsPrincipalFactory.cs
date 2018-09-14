using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityWithSession
{
    public class ApplicationClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ApplicationClaimsPrincipalFactory(UserManager<IdentityUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
            _userManager = userManager;
        }

        public async override Task<ClaimsPrincipal> CreateAsync(IdentityUser user)
        {
            // find old sessions and remove
            var claims = await _userManager.GetClaimsAsync(user);

            var session = claims.Where(e => e.Type == "session");

            await _userManager.RemoveClaimsAsync(user, session);

            // add new session claim
            await _userManager.AddClaimAsync(user, new Claim("session", Guid.NewGuid().ToString()));

            // create principal
            var principal = await base.CreateAsync(user);

            return principal;
        }
    }
}
