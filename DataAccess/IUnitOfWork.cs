namespace QLKhachSanAPI.DataAccess
{
    using Models;
    using Models.Domains;
    using static QLKhachSanAPI.DataAccess.UnitOfWork;

    public interface IUnitOfWork
    {
        IRepository<ApplicationUser> ApplicationUserRepository { get; set; }
    
        IRepository<FeedbackToken> FeedbackTokenRepository { get; set; }
        IRepository<Guest> GuestRepository { get; set; }
        IRepository<Reservation> ReservationRepository { get; set; }
        IRepository<ReservationRoom> ReservationRoomRepository { get; set; }
        IRepository<Room> RoomRepository { get; set; }
        IRepository<RoomType> RoomTypeRepository { get; set; }
        IRepository<Bill> BillRepository { get; set; }
        IRepository<Service> ServiceRepository { get; set; }
        IRepository<GuestService> GuestServiceRepository { get; set; }
        IRepository<Report> ReportRepository { get; set; }
        //Task CommitAsync();
        Task<SaveEntitiesResult> SaveEntitiesAsync();
        Task<bool> RollbackAsync(); // with status as return
    }
}
