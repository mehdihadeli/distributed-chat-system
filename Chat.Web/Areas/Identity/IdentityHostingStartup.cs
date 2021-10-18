using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Chat.Web.Areas.Identity.IdentityHostingStartup))]

namespace Chat.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {

            });
        }
    }
}