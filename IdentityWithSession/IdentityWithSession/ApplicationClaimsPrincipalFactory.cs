using IdentityWithSession.Data;
using IdentityWithSession.Models;
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
            var claims = await _userManager.GetClaimsAsync(user);

            var session = claims.Where(e => e.Type == "session");

            await _userManager.RemoveClaimsAsync(user, session);

            await _userManager.AddClaimAsync(user, new Claim("session", Guid.NewGuid().ToString()));

            var principal = await base.CreateAsync(user);

            return principal;
        }
    }
}
