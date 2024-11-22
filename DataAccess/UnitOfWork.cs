using Microsoft.EntityFrameworkCore;
using QLKhachSanAPI.Models;
using QLKhachSanAPI.Models.DAL;
using QLKhachSanAPI.Models.Domains;

namespace QLKhachSanAPI.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed = false;
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            ApplicationUserRepository = new Repository<ApplicationUser>(context);  
            FeedbackTokenRepository = new Repository<FeedbackToken>(context);
            GuestRepository = new Repository<Guest>(context);
            ReservationRepository = new Repository<Reservation>(context);
            ReservationRoomRepository = new Repository<ReservationRoom>(context);
            RoomRepository = new Repository<Room>(context);
            RoomTypeRepository = new Repository<RoomType>(context);
            BillRepository = new Repository<Bill>(context);
            ServiceRepository = new Repository<Service>(context);
            GuestServiceRepository = new Repository<GuestService>(context);
            ReportRepository = new Repository<Report>(context);
        }

        public IRepository<ApplicationUser> ApplicationUserRepository { get ; set ; }
        
        public IRepository<FeedbackToken> FeedbackTokenRepository { get; set ; }
        public IRepository<Guest> GuestRepository { get ; set; }
        public IRepository<Reservation> ReservationRepository { get ; set; }
        public IRepository<ReservationRoom> ReservationRoomRepository { get; set ; }
        public IRepository<Room> RoomRepository { get ; set ; }
        public IRepository<RoomType> RoomTypeRepository { get; set ; }
        public IRepository<Bill> BillRepository { get; set; }
        public IRepository<Service> ServiceRepository { get; set; }
        public IRepository<GuestService> GuestServiceRepository { get; set; }
        public IRepository<Report> ReportRepository { get; set; }

        //public Task CommitAsync()
        //{
        //    return _context.SaveChangesAsync();
        //}

        // Commit all changes to database:
        public async Task<SaveEntitiesResult> SaveEntitiesAsync()
        {
            try
            {
                int rowsAffected = await _context.SaveChangesAsync(); // returns the number of rows (entities) affected by the database operation
                if (rowsAffected > 0)
                {
                    // Success: Rows were affected in the database
                    return new SaveEntitiesResult
                    {
                        Success = true,
                        RowsAffected = rowsAffected
                    };
                }
                else
                {
                    // Failure: No rows were affected in the database
                    return new SaveEntitiesResult
                    {
                        Success = false,
                        RowsAffected = rowsAffected
                    };
                }
            }
            catch (Exception ex)
            {
                // Exception occurred while saving changes
                // Handle the exception appropriately
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public class SaveEntitiesResult
        {
            public bool Success { get; set; }
            public int RowsAffected { get; set; }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        //  handle exceptions internally and return a success or failure status,
        //  we can change the return type to Task<bool> or Task<Object>
        //  as we did in the SaveEntitiesAsync method above
        public async Task<bool> RollbackAsync()
        {
            try
            {
                // Get all modified or added entities in the current transaction
                var modifiedEntities = _context.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added)
                    .ToList();

                // Reset the state of each entity to its original values
                foreach (var entity in modifiedEntities)
                {
                    entity.State = EntityState.Unchanged;
                }

                // Discard any entities that were marked for deletion
                var deletedEntities = _context.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Deleted)
                    .ToList();

                foreach (var entity in deletedEntities)
                {
                    entity.State = EntityState.Unchanged;
                }

                // Save the changes to the database
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
