using System.Threading.Tasks;
using BlazorChat.Shared;
using Microsoft.AspNetCore.Identity;

namespace Chat.Application.Repositories
{
    public interface IIdentityRepository
    {
        Task<ApplicationUser> GetUserByEmailAsync(string userEmail);
    }
}