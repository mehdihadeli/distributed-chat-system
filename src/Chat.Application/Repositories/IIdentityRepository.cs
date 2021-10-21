using System.Threading.Tasks;
using Chat.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Chat.Application.Repositories
{
    public interface IIdentityRepository
    {
        Task<ApplicationUser> GetUserByEmailAsync(string userEmail);
        Task<ApplicationUser> GetUserByNameAsync(string userName);
    }
}