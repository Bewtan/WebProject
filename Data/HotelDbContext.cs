using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Data
{
    public class HotelDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Reservation> Reservations { get; set; }


        public HotelDbContext()
        {

        }
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=HotelDb;Integrated Security=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reservations)
                .WithOne(res => res.User)
                .HasForeignKey(res => res.UserId);
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Reservation)
                .WithOne(res => res.Room)
                .HasForeignKey<Reservation>(res => res.RoomId);
            modelBuilder.Entity<Reservation>()
                .HasMany(res => res.Clients)
                .WithMany(c => c.Reservations);
        }
    }
}
