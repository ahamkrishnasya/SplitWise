using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SplitWise.Domain.Entities;
using SplitWise.Infrastructure.Identity;

namespace SplitWise.Infrastructure.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<GroupMember>()
                .HasOne<ApplicationUser>()
                .WithMany(u => u.GroupMembers)
                .HasForeignKey(gm => gm.UserId)
                .IsRequired();

 
            builder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.GroupMembers)
                .HasForeignKey(gm => gm.GroupId)
                .IsRequired();
        }
    }
}

