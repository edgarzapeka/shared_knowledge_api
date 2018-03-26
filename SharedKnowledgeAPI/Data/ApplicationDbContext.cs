using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedKnowledgeAPI.Models;

namespace SharedKnowledgeAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<CommentLink>()
                .HasOne(au => au.ApplicationUser)
                .WithMany(c => c.Comments)
                .HasForeignKey(fk => fk.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<CommentLink>()
                .HasOne(l => l.Link)
                .WithMany(c => c.Comments)
                .HasForeignKey(fk => fk.LinkId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Link>()
                .HasOne(au => au.ApplicationUser)
                .WithMany(l => l.Links)
                .HasForeignKey(fk => fk.UserId)
                .OnDelete(DeleteBehavior.SetNull);
           
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<CommentLink> CommentLink { get; set; }
        public DbSet<Link> Link { get; set; }

    }
}
