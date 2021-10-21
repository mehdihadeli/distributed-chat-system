using Chat.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Data
{
    public class ApplicationDataContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : base(options)
        {
        }

        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Message);

                entity.HasOne<ApplicationUser>(x => x.FromUser)
                    .WithMany()
                    .HasForeignKey(p => p.FromUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                ;

                entity.HasOne<ApplicationUser>(x => x.ToUser)
                    .WithMany()
                    .HasForeignKey(p => p.ToUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                ;

                entity.Property(x => x.FromUserId).IsRequired();
                entity.Property(x => x.ToUserId).IsRequired();
            });

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}