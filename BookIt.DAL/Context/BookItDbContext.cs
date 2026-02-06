using BookIt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookIt.DAL.Context
{
    public class BookItDbContext : DbContext
    {
        public BookItDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<FavoriteTenant> FavoriteTenants { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceAvailability> ServiceAvailabilities { get; set; }
        public DbSet<ServiceTimeSlot> ServiceTimeSlots { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantConfiguration> TenantConfigurations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WorkingHour> WorkingHours { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // FavoriteTenant - disable cascade delete
            modelBuilder.Entity<FavoriteTenant>()
                .HasOne(ft => ft.User)
                .WithMany(u => u.FavoriteTenants)
                .HasForeignKey(ft => ft.UserId)
                .OnDelete(DeleteBehavior.NoAction);  

            modelBuilder.Entity<FavoriteTenant>()
                .HasOne(ft => ft.Tenant)
                .WithMany()
                .HasForeignKey(ft => ft.TenantId)
                .OnDelete(DeleteBehavior.NoAction);

            // Appointment - disable cascade delete for User and Service
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);  

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            // ServiceAvailability - NoAction for Tenant (will delete via Service)
            modelBuilder.Entity<ServiceAvailability>()
                .HasOne(sa => sa.Tenant)
                .WithMany()
                .HasForeignKey(sa => sa.TenantId)
                .OnDelete(DeleteBehavior.NoAction);

            // ServiceTimeSlot - NoAction za Service (will delete via Service cascade)
            modelBuilder.Entity<ServiceTimeSlot>()
                .HasOne(sts => sts.Service)
                .WithMany(s => s.TimeSlots)
                .HasForeignKey(sts => sts.ServiceId)
                .OnDelete(DeleteBehavior.NoAction);

            // User - Email unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Tenant - Slug unique
            modelBuilder.Entity<Tenant>()
                .HasIndex(t => t.Slug)
                .IsUnique();

            // FavoriteTenant
            modelBuilder.Entity<FavoriteTenant>()
                .HasIndex(ft => new { ft.UserId, ft.TenantId })
                .IsUnique();
        }
    }
}
