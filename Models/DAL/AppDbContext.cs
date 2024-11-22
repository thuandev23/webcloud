namespace QLKhachSanAPI.Models.DAL
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using QLKhachSanAPI.Models.Domains;

    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        // Model & Relation Mapping:
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // must have this line first, else you will end up getting the error (The entity type 'IdentityUserLogin<string>' requires a primary key to be defined. If you intended to use a keyless entity type, call 'HasNoKey' in 'OnModelCreating')
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Report>()
               .HasKey(rr => new { rr.idGuest });


            #region RoomType - Room's (1-n) relationship:
            modelBuilder.Entity<Room>()
            .HasOne(op => op.RoomType)
            .WithMany(au => au.Rooms)
            .HasForeignKey(op => op.RoomTypeID);

            modelBuilder.Entity<Reservation>()
            .HasOne(op => op.Guest)
            .WithMany(au => au.Reservations)
            .HasForeignKey(op => op.GuestID);

            modelBuilder.Entity<Bill>()
                .HasKey(b => new { b.ID });

            modelBuilder.Entity<Bill>()
                .HasOne(g => g.Guest)
                .WithMany(b => b.Bills)
                .HasForeignKey(b => b.IDGuest)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Rersevation - Room's (n-n) relationship:
            //ReservationRoom
            modelBuilder.Entity<ReservationRoom>()
                .HasKey(rr => new { rr.RoomID, rr.ReservationID });


            modelBuilder.Entity<ReservationRoom>()
                .HasOne(rr => rr.Room)
                .WithMany(r => r.ReservationRooms)
                .HasForeignKey(rr => rr.RoomID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReservationRoom>()
                .HasOne(rr => rr.Reservation)
                .WithMany(res => res.ReservationRooms)
                .HasForeignKey(rr => rr.ReservationID)
                .OnDelete(DeleteBehavior.Restrict);

            
            //GuestService
            modelBuilder.Entity<GuestService>()
                .HasKey(gs => new { gs.ServiceID, gs.GuestID });

            modelBuilder.Entity<GuestService>()
                .HasOne(s => s.Service)
                .WithMany(gs => gs.GuestService)
                .HasForeignKey(gs => gs.ServiceID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GuestService>()
                .HasOne(ls => ls.Guest)
                .WithMany(gs=>gs.GuestService)
                .HasForeignKey(gs => gs.GuestID)
                .OnDelete(DeleteBehavior.Restrict);

            //Bill
            

            

            #endregion


            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6)); // remove first 6 character
                }
            }
        }


        // Users , it can be ApplicationUser in your project!
        public DbSet<ApplicationUser> Users { get; set; } // "Users" will be the name of SQL table
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationRoom> ReservationRooms { get; set; }
        public DbSet<FeedbackToken> FeedbackTokens { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<GuestService> GuestService { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Service> Services { get; set; }

        public DbSet<Report> Reports { get; set; }

    }
}
