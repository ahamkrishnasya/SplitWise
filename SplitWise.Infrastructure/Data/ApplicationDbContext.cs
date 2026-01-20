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
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseShare> ExpenseShares { get; set; }
        public DbSet<Settlement> Settlements { get; set; }

        #region modelBuilder

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);



            builder.Entity<Group>()
                .HasKey(g => g.Id);

            builder.Entity<Group>()
                .HasMany(g => g.GroupMembers)
                .WithOne(gm => gm.Group)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.Entity<Group>()
                .Property(g => g.GroupName)
                .IsRequired()
                .HasMaxLength(100);





            builder.Entity<GroupMember>()
                .HasOne<ApplicationUser>()
                .WithMany(u => u.GroupMembers)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.Entity<GroupMember>()
                .HasIndex(gm => new { gm.UserId, gm.GroupId })
                .IsUnique();




            builder.Entity<Expense>()
                .HasOne<Group>()
                .WithMany()
                .HasForeignKey(e => e.GroupId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Expense>()
                .HasOne<GroupMember>()
                .WithMany()
                .HasForeignKey(e => e.PaidByGroupMemberId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);




            builder.Entity<ExpenseShare>()
                .HasKey(es => es.Id);

            builder.Entity<ExpenseShare>()
                .HasOne<Expense>()
                .WithMany()
                .HasForeignKey(es => es.ExpenseId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.Entity<ExpenseShare>()
                .HasOne<GroupMember>()
                .WithMany()
                .HasForeignKey(es => es.GroupMemberId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.Entity<ExpenseShare>()
                .HasIndex(es => new { es.ExpenseId, es.GroupMemberId })
                .IsUnique(); 




            builder.Entity<Settlement>()
                .HasKey(s => s.Id);

            builder.Entity<Settlement>()
                .HasOne<Group>()
                .WithMany()
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.Entity<Settlement>()
                .HasOne<GroupMember>()
                .WithMany()
                .HasForeignKey(s => s.PaidByGroupMemberId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.Entity<Settlement>()
                .HasOne<GroupMember>()
                .WithMany()
                .HasForeignKey(s => s.PaidToGroupMemberId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();



        }
        #endregion
    }
}

