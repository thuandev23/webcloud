namespace QLKhachSanAPI.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Domains;
    using DataAccess;
    using Models.DTOs;
    using Services.Interfaces;


    [Route("api/[controller]")]
    [ApiController]
    public class ReservationRoomController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservationRoomService _reservationRoomService;

        public ReservationRoomController(IUnitOfWork unitOfWork, IReservationRoomService reservationRoomService)
        {
            _unitOfWork = unitOfWork;
            _reservationRoomService = reservationRoomService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReservationRoom>>> GetReservation()
        {

            var reserVation = await _unitOfWork.ReservationRoomRepository.GetAsync();
            return Ok(reserVation);
        }


        [HttpGet("[action]")]
        public async Task<ActionResult<ReservationRoom>> GetReservationsByReservationID([FromQuery] string id)
        {
            var reserVation = await _reservationRoomService.GetAllReservationRoomsByReservationID(id);
            if (reserVation == null)
            {
                return NotFound();
            }
            return Ok(reserVation);
        }

        
      
        [HttpPost]
        public async Task<ActionResult> AddReservationRoom([FromBody] ReservationRoomVM model)
        {
            try
            {
                var result = await _reservationRoomService.CreateReservationRoomAsync(model);
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Created" });
                }
                return Ok(new { succeeded = false, message = "RoomID or ReservationID not found" });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }

        

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAllReservationRoomTypes()
        {
            try
            {
                var result = await _reservationRoomService.DeleteAllReservationRoomsAsync();
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Deleted all records !" });
                }
                return Ok(new { succeeded = false, message = "Failed to delete All Reservation Room!" });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }
    }
}
