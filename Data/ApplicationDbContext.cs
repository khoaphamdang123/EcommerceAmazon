using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Product.Models;

namespace Ecommerce_Product.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

 protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Override default Identity model configuration for MySQL/MariaDB
        builder.Entity<IdentityRole>(entity =>
        {
            entity.Property(m => m.Id).HasMaxLength(128).IsUnicode(false);
            entity.Property(m => m.NormalizedName).HasMaxLength(256).IsUnicode(false);
        });

        builder.Entity<IdentityUser>(entity =>
        {
            entity.Property(m => m.Id).HasMaxLength(128).IsUnicode(false);
            entity.Property(m => m.NormalizedEmail).HasMaxLength(256).IsUnicode(false);
            entity.Property(m => m.NormalizedUserName).HasMaxLength(256).IsUnicode(false);
        });
    }
    }
}