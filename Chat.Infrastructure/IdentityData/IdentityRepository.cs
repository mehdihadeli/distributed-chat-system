using System.Threading.Tasks;
using BlazorChat.Shared;
using Chat.Application.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Chat.Infrastructure.IdentityData
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string userEmail)
        {
            return await _userManager.FindByEmailAsync(userEmail);
        }
    }
}