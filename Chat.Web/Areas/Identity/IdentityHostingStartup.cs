using Chat.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Chat.Web.Areas.Identity.IdentityHostingStartup))]

namespace Chat.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<IdentityDataContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("IdentityDataContextConnection")));

                services.AddDefaultIdentity<IdentityUser>(options =>
                    {
                        options.SignIn.RequireConfirmedAccount = false;
                    })
                    .AddEntityFrameworkStores<IdentityDataContext>();
            });
        }
    }
}