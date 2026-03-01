using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class SmartClassDbContext : DbContext
    {
        public SmartClassDbContext(DbContextOptions<SmartClassDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<RoomMember> RoomMembers => Set<RoomMember>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RoomMember>()
                .HasKey(rm => new { rm.RoomId, rm.UserId });

            modelBuilder.Entity<RoomMember>()
                .HasIndex(rm => new { rm.RoomId, rm.UserId })
                .IsUnique();

            modelBuilder.Entity<RoomMember>()
                .HasOne(rm => rm.Room)
                .WithMany(r => r.RoomMembers)
                .HasForeignKey(rm => rm.RoomId);

            modelBuilder.Entity<RoomMember>()
                .HasOne(rm => rm.User)
                .WithMany(u => u.RoomMembers)
                .HasForeignKey(rm => rm.UserId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.GoogleId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Room>()
                .HasIndex(r => r.Code)
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasKey(rf => rf.Id);

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rf => rf.Token)
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rf => rf.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rf => rf.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
                .Property(r => r.RowVersion)
                .IsRowVersion();

        }
    }
}
