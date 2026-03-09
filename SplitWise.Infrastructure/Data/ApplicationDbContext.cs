using Microsoft.EntityFrameworkCore;
using SplitWise.Domain.Entities;

namespace SplitWise.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #region DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseShare> ExpenseShares { get; set; }
        public DbSet<Settlement> Settlements { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        #endregion

        #region modelBuilder

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<User>()
                .HasKey(u => u.Id);

            builder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(254);

            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(512);

            builder.Entity<User>()
                .Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Entity<User>()
                .Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);


            builder.Entity<Groups>()
                .HasKey(g => g.Id);

            builder.Entity<Groups>()
                .HasMany(g => g.GroupMembers)
                .WithOne(gm => gm.Groups)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.Entity<Groups>()
                .Property(g => g.GroupName)
                .IsRequired()
                .HasMaxLength(100);


            builder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMembers)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.Entity<GroupMember>()
                .HasIndex(gm => new { gm.UserId, gm.GroupId })
                .IsUnique();



            builder.Entity<Expense>()
                .HasOne<Groups>()
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
                .HasOne(es => es.Expense)
                .WithMany(e => e.Shares)
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
                .HasOne<Groups>()
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

            builder.Entity<Friendship>(entity =>
            {
                entity.HasKey(f => f.Id);

                entity.Property(f => f.Id)
                      .ValueGeneratedOnAdd();

                entity.HasIndex(f => new { f.UserId1, f.UserId2 })
                      .IsUnique();

                entity.HasOne(f => f.User1)
                      .WithMany(u => u.FriendshipsInitiated)
                      .HasForeignKey(f => f.UserId1)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(f => f.User2)
                      .WithMany(u => u.FriendshipsReceived)
                      .HasForeignKey(f => f.UserId2)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Friendship_UserId_Order",
                        "[UserId1] < [UserId2]"
                    );
                });
            });
            builder.Entity<EmailVerificationToken>(entity =>
            {
                entity.HasKey(evt => evt.Id);

                entity.Property(evt => evt.Token)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasIndex(evt => evt.Token)
                      .IsUnique();

                entity.HasIndex(evt => evt.UserId);

                entity.HasOne(evt => evt.User)
                      .WithMany(u => u.EmailVerificationTokens)
                      .HasForeignKey(evt => evt.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });
            builder.Entity<PasswordResetToken>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Token)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasIndex(t => t.Token)
                      .IsUnique();

                entity.HasIndex(t => t.UserId);

                entity.Property(t => t.ExpiresAt)
                      .IsRequired();

                entity.HasOne(t => t.User)
                      .WithMany(u => u.PasswordResetTokens)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });
        }

        #endregion
    }
}
