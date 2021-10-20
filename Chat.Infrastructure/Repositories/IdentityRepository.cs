using System.Threading.Tasks;
using Chat.Application.Repositories;
using Chat.Core.Entities;
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

        public async Task<ApplicationUser> GetUserByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }
    }
}