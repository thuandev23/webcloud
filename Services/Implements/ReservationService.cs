namespace QLKhachSanAPI.Services.Implements
{
    using Microsoft.EntityFrameworkCore;
    using DataAccess;
    using Models.DAL;
    using Models.Domains;
    using Models.DTOs;
    using Services.Interfaces;
    using Extensions;
    using Microsoft.AspNetCore.Http.HttpResults;

    public class ReservationService : IReservationService

    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _dbContext;
        private readonly IRoomService _roomService;
        private readonly IBillService _billService;

        //private readonly IMemoryCache _memoryCache;
        //public string getAllRoomTypeCacheKey = "ListRoomTypes";

        public ReservationService(IUnitOfWork unitOfWork, AppDbContext dbContext, IRoomService roomService, IBillService billService)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _roomService = roomService;
            _billService = billService;
        }

      

        public async Task<bool> ReserveRoomsAsync(ReservationViewModel reservationvm)  //booking
        {
            RoomType rt = await _unitOfWork.RoomTypeRepository.GetSingleAsync(d => d.RoomTypeID == reservationvm.RoomTypeId);
            var check = await _roomService.CheckRoom(reservationvm.StartTime,reservationvm.EndTime, rt, reservationvm.NumberOfRooms);
            if (!check)
            {
                return false;
            }

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    // nếu Email đã tồn tại thì ko tạo ra khách nữa
                    var checkEmail = await _unitOfWork.GuestRepository.GetSingleAsync(d => d.Email == reservationvm.GuestEmail);
                    var guest = new Guest();

                    if (checkEmail == null)
                    {
                         guest.FullName = reservationvm.GuestFullName;
                         guest.PhoneNumber = reservationvm.GuestPhoneNumber;
                         guest.Email  = reservationvm.GuestEmail;
                         await _unitOfWork.GuestRepository.InsertAsync(guest);
                         await _unitOfWork.SaveEntitiesAsync();
                    }
                    else
                    {
                        guest.GuestID = checkEmail.GuestID;
                        guest.FullName = reservationvm.GuestFullName;
                        guest.PhoneNumber = reservationvm.GuestPhoneNumber;
                        guest.Email = reservationvm.GuestEmail;
                    }

                    //Add reservation
                    //if exist reservation , not add
                    var checkReservation = await _unitOfWork.ReservationRepository.GetSingleAsync(d => d.GuestID == guest.GuestID);
                    var reservation = new Reservation();
                    if (checkReservation == null)
                    {
                        reservation.GuestID = guest.GuestID;
                        reservation.IsConfirmed = false;
                        reservation.StartTime = reservationvm.StartTime;
                        reservation.EndTime = reservationvm.EndTime;
                        await _unitOfWork.ReservationRepository.InsertAsync(reservation);
                        await _unitOfWork.SaveEntitiesAsync();
                    }
                    else {
                        reservation.ReservationID = checkReservation.ReservationID;
                    }

                    // Create ReservationRoom records


                    var rooms = await _roomService.GetRoomNotReser(reservationvm.StartTime, reservationvm.EndTime, rt);
                   
                    for(int i = 0; i < reservationvm.NumberOfRooms; i++)
                    {
                        var reservationRoom = new ReservationRoom
                        {
                            RoomID = rooms[i].RoomID,
                            ReservationID = reservation.ReservationID
                        };
                       await  _unitOfWork.ReservationRoomRepository.InsertAsync(reservationRoom);
                    }

                    await _unitOfWork.SaveEntitiesAsync();


                    //Bill
                    double differenceInDays = (reservationvm.EndTime - reservationvm.StartTime).TotalDays;

                    var checkBill = await _unitOfWork.BillRepository.GetSingleAsync(d => (d.IDGuest == guest.GuestID && d.Status == false));
                   
                    if (checkBill == null)
                    {
                        Bill b = new Bill();
                        b.IDGuest = guest.GuestID;
                        b.Sum = reservationvm.NumberOfRooms * rt.DailyPrice * differenceInDays;
                        b.Status = false;
                        await _unitOfWork.BillRepository.InsertAsync(b);
                        await _unitOfWork.SaveEntitiesAsync();
                    }
                    else
                    {
                        BillVM b = new BillVM();
                        b.IdToUpdate = checkBill.ID;
                        b.Sum = checkBill.Sum + reservationvm.NumberOfRooms * rt.DailyPrice * differenceInDays;
                        b.Status = checkBill.Status;
                        b.IDGuest = checkBill.IDGuest;

                       

                        await _billService.UpdateBillAsync(b);
                       
                    }
                   

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    // Handle any errors or exceptions
                    transaction.Rollback();
                    return false;
                }
            }
        }
        public async Task<bool> CreateReservationAsync(ReservationVM model)
        {
            var guestId = await _unitOfWork.GuestRepository.GetSingleAsync(model.GuestID);

            if (guestId != null )
            {
                var reservation = new Reservation
                {
                    GuestID = model.GuestID,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    IsConfirmed = false,
                   
                };
                await _unitOfWork.ReservationRepository.InsertAsync(reservation);
                await _unitOfWork.SaveEntitiesAsync();
                return true;
            }
            return false;


        }

        // nếu chỉ removerange reservation: Err: The DELETE statement conflicted with the REFERENCE constraint "FK_ReservationRooms_Reservations_ReservationID". The conflict occurred in database "QLKhachSan2", table "dbo.ReservationRooms", column 'ReservationID'.
        public async Task<bool> DeleteAllReservationsAsync()
        {
            var reservations = await _unitOfWork.ReservationRepository.GetAsync();

            foreach (var reservation in reservations)
            {
                // Remove related ReservationRooms
                var reservationRooms = await _dbContext.ReservationRooms
                    .Where(rr => rr.ReservationID == reservation.ReservationID)
                    .ToListAsync();

                _dbContext.RemoveRange(reservationRooms);

                // Update the status of related rooms to 0 (available)
                foreach (var room in reservationRooms)
                {
                    var relatedRoom = await _dbContext.Rooms.FindAsync(room.RoomID);
                    if (relatedRoom != null)
                    {
                        relatedRoom.IsAvaiable = false; // Set status to 0 (available)
                    }
                }
            }

            _dbContext.RemoveRange(reservations);

            await _unitOfWork.SaveEntitiesAsync();

            return true;
        }

        public async Task<List<object>> GetAllReservationsAsyncLinQ()
        {
            var reservations = await _dbContext.Reservations
                .Include(re => re.ReservationRooms)
                .ThenInclude(ro => ro.Room)
                .Include(re => re.Guest)
                .Select(re => new
                {
                    ReservationID = re.ReservationID,
                    StartTime = re.StartTime,
                    EndTime = re.EndTime,
                    IsConfirm = re.IsConfirmed,
                  
                    Guest = new
                    {
                        Id = re.Guest.GuestID,
                        FullName = re.Guest.FullName,
                      
                        DateCreated = TimeHelper.ConvertToUserFriendlyDateTimeFormat(re.Guest.DateCreated)
                    },
                    ReservatedRooms = re.ReservationRooms.Select(ro => new
                    {
                        Id = ro.Room.RoomID,
                        RoomNumber = ro.Room.RoomNumber,
                        RoomTypeId = ro.Room.RoomTypeID,
                        RoomTypeName = ro.Room.RoomType.Name
                    }).ToList()

                })
                .ToListAsync(); // awaiting the completion !!!

            return reservations.Cast<object>().ToList();
        }

/*        public async Task<bool> ConfirmReservationAsync(ConfirmReservationVM model) // true to confirm
        {
            Reservation reservation = await _unitOfWork.ReservationRepository.GetSingleAsync(model.ReservationId);
            reservation.IsConfirmed = model.Confirm;
            var res = await _unitOfWork.SaveEntitiesAsync();
            return res.Success;
        }*/

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            return await _unitOfWork.ReservationRepository.GetAsync();
        }

        public async Task<List<Reservation>> GetReservationsByDate(DateTime StartTime, DateTime EndTime)
        {
            return await _unitOfWork.ReservationRepository.GetAsync(d => (DateTime.Compare(StartTime, d.StartTime) > 0 && DateTime.Compare(d.StartTime, StartTime) > 0 ) || (DateTime.Compare(EndTime, d.EndTime) > 0 && DateTime.Compare(d.EndTime, EndTime) > 0) || (DateTime.Compare(d.StartTime, StartTime) >= 0 && DateTime.Compare(d.EndTime, EndTime) <= 0));
        }
        public async Task<Reservation> GetReservationsByID(string Id)
        {
            return await _unitOfWork.ReservationRepository.GetSingleAsync(d => d.ReservationID == Id);
        }

        public async Task<List<Reservation>> GetReservationsByGuestID(string Id)
        {
            return await _unitOfWork.ReservationRepository.GetAsync(d => d.GuestID == Id);

        }

       
       
        public async Task<List<Reservation>> GetReservationsByWasConfirm(bool isConfirm)
        {
            return await _unitOfWork.ReservationRepository.GetAsync(d => d.IsConfirmed == isConfirm);
        }
        public async Task<List<object>> GetReservationsByWasConfirmLinQ(bool isConfirm)
        {
            var reservations = await _dbContext.Reservations
                .Include(re => re.ReservationRooms)
                .ThenInclude(ro => ro.Room)
                .Include(re => re.Guest)
                .Where(re => re.IsConfirmed == isConfirm)
                .Select(re => new
                {
                    ReservationID = re.ReservationID,
                    StartTime = re.StartTime,
                    EndTime = re.EndTime,
                   
                    Guest = new
                    {
                        Id = re.Guest.GuestID,
                        FullName = re.Guest.FullName,
                       
                        DateCreated = TimeHelper.ConvertToUserFriendlyDateTimeFormat(re.Guest.DateCreated).ToString()
                    },
                    ReservatedRooms = re.ReservationRooms.Select(ro => new
                    {
                        Id = ro.Room.RoomID,
                        RoomNumber = ro.Room.RoomNumber,
                        RoomTypeId = ro.Room.RoomTypeID,
                        RoomTypeName = ro.Room.RoomType.Name
                    }).ToList()

                })
                .ToListAsync(); // awaiting the completion !!!

            return reservations.Cast<object>().ToList();
        }

        public async Task<bool> UpdateReservationAsync(ReservationVM model)
        {
            var reservation = await _unitOfWork.ReservationRepository.GetSingleAsync(model.IdToUpdate);

            if (reservation == null)
            {
                // Throw an exception or handle the case where the drink is not found
                throw new Exception("Reservation not found");
            }

            var guestId = await _unitOfWork.GuestRepository.GetSingleAsync(model.GuestID);

            if (guestId == null)
            {
                return false;
            }

            reservation.GuestID = model.GuestID;
            
            reservation.StartTime = model.StartTime;
            reservation.EndTime = model.EndTime;
            reservation.IsConfirmed = model.IsConfirmed;
            reservation.ConfirmationTime = model.ConfirmationTime;

            _unitOfWork.ReservationRepository.Update(reservation);
            await _unitOfWork.SaveEntitiesAsync();

            // Remove the cache entry for the modified data
            //_memoryCache.Remove(getAllDrinkTypeCacheKey);

            // Return true if the drink type was successfully added
            return true;
        }

        public async Task<bool> CheckIn(string IDReservation)
        {

            var reservation = await _unitOfWork.ReservationRepository.GetSingleAsync(IDReservation);
            if (reservation == null)
            {
                return false;
            }

            reservation.IsConfirmed = true;
            reservation.ConfirmationTime = GetCurrentTimeInDesiredTimeZone();
            await _unitOfWork.SaveEntitiesAsync();

            var reservationRooms = await _unitOfWork.ReservationRoomRepository.GetAsync(d=>d.ReservationID == IDReservation);

            //update room
            foreach (var reservationRoom in reservationRooms)
            {
                var room = await _unitOfWork.RoomRepository.GetSingleAsync(reservationRoom.RoomID);
                room.IsAvaiable = false;
                await _unitOfWork.SaveEntitiesAsync();  
            }
            return true ;
        }

        public async Task<bool> CheckOut(string IDReservation)
        {
            var reservation = await _unitOfWork.ReservationRepository.GetSingleAsync(IDReservation);
            if (reservation == null || reservation.IsConfirmed==false)
            {
                return false;
            }

            //update bill
            Bill bill = await _unitOfWork.BillRepository.GetSingleAsync(d => (d.IDGuest == reservation.GuestID && d.Status == false));
            bill.Status = true;
            bill.DateCreated = GetCurrentTimeInDesiredTimeZone();
            await _unitOfWork.SaveEntitiesAsync();

            //update room
            var reservationRooms = await _unitOfWork.ReservationRoomRepository.GetAsync(d => d.ReservationID == IDReservation);
            foreach (var reservationRoom in reservationRooms)
            {
                var room = await _unitOfWork.RoomRepository.GetSingleAsync(reservationRoom.RoomID);
                room.IsAvaiable = true;
                await _unitOfWork.SaveEntitiesAsync();
            }

            //delete reservationroom
            var ReservationRooms = await _unitOfWork.ReservationRoomRepository.GetAsync(d => d.ReservationID == reservation.ReservationID);
            foreach(var ReservationRoom in ReservationRooms)
            {
                 _dbContext.Set<ReservationRoom>().Remove(ReservationRoom);
                 await _dbContext.SaveChangesAsync();
            }

            //delete reservation
            await _unitOfWork.ReservationRepository.DeleteAsync(reservation.ReservationID);
            await _unitOfWork.SaveEntitiesAsync();

            //delete GuestService
            var guestServices = await _unitOfWork.GuestServiceRepository.GetAsync(d => (d.GuestID == reservation.GuestID));
            foreach (var guestService in guestServices)
            {
                _dbContext.Set<Models.Domains.GuestService>().Remove(guestService);
                await _dbContext.SaveChangesAsync();
            }
           
            return true;
        }

        public async Task<bool> Cancel(string IDReservation)
        {
            var reservation = await _unitOfWork.ReservationRepository.GetSingleAsync(IDReservation);
            if (reservation == null || reservation.IsConfirmed == true)
            {
                return false;
            }
            var bill = await _unitOfWork.BillRepository.GetSingleAsync(d => d.Status == false && d.IDGuest == reservation.GuestID);

            //delete bill
            _dbContext.Set<Bill>().Remove(bill);
            await _unitOfWork.SaveEntitiesAsync();

            //delete reservationRoom
            var reservationRooms = await _unitOfWork.ReservationRoomRepository.GetAsync(d => (d.ReservationID == IDReservation));
            foreach (var reservationRoom in reservationRooms)
            {
                _dbContext.Set<ReservationRoom>().Remove(reservationRoom);
                await _dbContext.SaveChangesAsync();
            }

            //delete Reservation
            _dbContext.Set<Reservation>().Remove(reservation);
            await _unitOfWork.SaveEntitiesAsync();

            return true;
        }

        public async Task<Reservation> GetReservationByRoom(string IDRoom)
        {
            ReservationRoom reservationRoom = await _unitOfWork.ReservationRoomRepository.GetSingleAsync(d => d.RoomID == IDRoom);
            if (reservationRoom == null)
            {
                return null;
            }
            return await _unitOfWork.ReservationRepository.GetSingleAsync(d => (d.ReservationID == reservationRoom.ReservationID && d.IsConfirmed == true));
        }

        private static DateTime GetCurrentTimeInDesiredTimeZone()
        {
            TimeZoneInfo desiredTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // ((GMT+07:00) Bangkok, Hanoi, Jakarta)

            return TimeZoneInfo.ConvertTime(DateTime.Now, desiredTimeZone);
        }
    }
}
