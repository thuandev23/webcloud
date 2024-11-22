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
    public class ReservationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservationService _reservationService;

        public ReservationController(IUnitOfWork unitOfWork, IReservationService reservationService)
        {
            _unitOfWork = unitOfWork;
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Reservation>>> GetAllReservations()
        {
            var reserVation = await _reservationService.GetAllReservationsAsyncLinQ();
            return Ok(reserVation);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation([FromRoute] string id)
        {
            var reserVation = await _unitOfWork.ReservationRepository.GetSingleAsync(id);
            if (reserVation == null)
            {
                return NotFound();
            }
            return Ok(reserVation);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<Reservation>> GetReservationsByGuestID([FromQuery] string id)
        {
            var reserVation = await _reservationService.GetReservationsByGuestID(id);
            if (reserVation == null)
            {
                return NotFound();
            }
            return Ok(reserVation);
        }

        


        // [FromRoute] always make bool = false !!!!, so switched to [FromQuery]
        // https://localhost:7232/api/Reservation/GetReservationsByWasConfirm?confirm=true
        [HttpGet("[action]")]
        public async Task<ActionResult<Reservation>> GetReservationsByWasConfirm([FromQuery] bool confirm)
        {
            var reserVation = await _reservationService.GetReservationsByWasConfirmLinQ(confirm);
            if (reserVation == null)
            {
                return NotFound();
            }
            return Ok(reserVation);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<Reservation>> CheckIn([FromQuery] string IDReservation)
        {
            var reserVation = await _reservationService.CheckIn(IDReservation);
            if (!reserVation)
            {
                return NotFound();
            }
            return Ok(reserVation);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<Reservation>> CheckOut([FromQuery] string IDReservation)
        {
            var reserVation = await _reservationService.CheckOut(IDReservation);
            if (!reserVation)
            {
                return NotFound();
            }
            return Ok(reserVation);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<Reservation>> Cancel([FromQuery] string IDReservation)
        {
            var reserVation = await _reservationService.Cancel(IDReservation);
            if (!reserVation)
            {
                return NotFound();
            }
            return Ok(reserVation);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<Reservation>> GetReservationByRoom([FromQuery] string IDRoom)
        {
            var reserVation = await _reservationService.GetReservationByRoom(IDRoom);
            if (reserVation == null)
            {
                return NotFound();
            }
            return Ok(reserVation);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> NewReserveRoomsAsync([FromBody] ReservationViewModel reservationViewModel)
        {
            try
            {
                var result = await _reservationService.ReserveRoomsAsync(reservationViewModel);
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Rooms reserved successfully." });
                }
                else
                {
                    return BadRequest(new { succeeded = false, message = "Failed to reserve rooms. Not enough available rooms of the specified type." });
                }
            }
            catch (Exception ex)
            {
                // Handle and log the exception
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

       
        [HttpPost]
        public async Task<ActionResult> AddReservation([FromBody] ReservationVM model)
        {
            try
            {
                var result = await _reservationService.CreateReservationAsync(model);
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Created" });
                }
                return Ok(new { succeeded = false, message = "Guest ID or Room ID not found" });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateReservationAsync([FromBody] ReservationVM model)
        {
            try
            {
                // Check if the model is valid
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _reservationService.UpdateReservationAsync(model);
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Updated" });
                }
                return Ok(new { succeeded = false, message = "Failed to update Reservation! Dont found RoomID or GuestID" });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReservation([FromRoute] string id)
        {
            var result = await _unitOfWork.ReservationRepository.DeleteAsync(id);
            if (result)
            {
                await _unitOfWork.SaveEntitiesAsync();
                return Ok(new { succeeded = true, message = "Deleted one record !" });
            }
            return NotFound();
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult> DeleteAllReservations()
        {
            try
            {
                var result = await _reservationService.DeleteAllReservationsAsync();
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Deleted all records !" });
                }
                return Ok(new { succeeded = false, message = "Failed to delete All Reservation!" });
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
