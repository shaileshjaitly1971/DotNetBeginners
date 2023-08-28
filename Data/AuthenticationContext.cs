using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using InventoryBeginners.Models;

namespace InventoryBeginners.Data
{
    public class AuthenticationContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
        {

        }/// 
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "UserMaster");
                //entity.Property("UserName").IsRequired(true);
                //entity.Property("Email").IsRequired(true);
                //entity.Property("UserFulName").HasMaxLength(200);
            });

            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable(name: "RoleMaster");
                //entity.Property("Name").IsRequired(true);
                //entity.Property("Description").HasMaxLength(200);
            });

            builder.Entity<IdentityUserClaim<int>>(entity =>
            {
                entity.ToTable("UserClaim");
            });

            builder.Entity<IdentityUserLogin<int>>(entity =>
            {
                entity.ToTable("UserLogin");
            });

            builder.Entity<IdentityRoleClaim<int>>(entity =>
            {
                entity.ToTable("RoleClaim");
            });

            builder.Entity<IdentityUserRole<int>>(entity =>
            {
                entity.ToTable("UserRole");
            });

            builder.Entity<IdentityUserToken<int>>(entity =>
            {
                entity.ToTable("UserToken");
            });
        }
    }

    public class ApplicationUser : IdentityUser<int>
    {
        public string? FullName { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? LastLoginDevice { get; set; }
        public bool? IsOnline { get; set; }
        public string? OnlineDevice { get; set; }
        public bool? IsIPRestricted { get; set; }
        public string? AccessibleIP { get; set; }
        public string? UserType { get; set; }
        public int? CompanyId { get; set; }
        public string? BranchCode { get; set; }
    }

    public class ApplicationRole : IdentityRole<int>
    {
        public string? Description { get; set; }
    }
}
