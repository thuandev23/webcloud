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
    public class RoomController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomService _roomService;

        public RoomController(IUnitOfWork unitOfWork, IRoomService roomService)
        {
            _unitOfWork = unitOfWork;
            _roomService = roomService;
        }

       

        [HttpGet]
        public async Task<ActionResult<List<RoomType>>> GetRoomsLinQ()
        {

            var rooms = await _roomService.GetAllRoomsLinQAsync();
            return Ok(rooms);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<Room>>> GetRoomsIsValid([FromQuery] bool status)
        {

            var rooms = await _roomService.GetRoomsByIsAvailable(status);
            return Ok(rooms);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<Room>>> GetRoomsByTypeName([FromQuery] string roomTypeName)
        {
            var rooms = await _roomService.GetRoomsByTypeName(roomTypeName);
            return Ok(rooms);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<Room>>> GetRoomsByTypeId([FromQuery] string roomTypeId)
        {
            var rooms = await _roomService.GetRoomsByTypeId(roomTypeId);
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoomType([FromRoute] string id)
        {
            var roomType = await _unitOfWork.RoomRepository.GetSingleAsync(id);
            if (roomType == null)
            {
                return NotFound();
            }
            return Ok(roomType);
        }

        [HttpPost]
        public async Task<ActionResult> AddRoomAsync([FromBody] RoomVM model)
        {
            try
            {
                var result = await _roomService.CreateRoomAsync(model);
                if (result) return Ok(new { succeeded = true, message = "Created" });           
                return Ok(new { succeeded = false, message = "Dont found RoomType ID" });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<List<Room>>> GetRoomNotReser([FromBody] ReservationViewModel model)
        {
            RoomType rt = await _unitOfWork.RoomTypeRepository.GetSingleAsync(model.RoomTypeId);
            var result = await _roomService.GetRoomNotReser(model.StartTime, model.EndTime, rt);
            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateRoomAsync([FromBody] RoomVM model)
        {
            try
            {
                // Check if the model is valid
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = await _roomService.UpdateRoomAsync(model);
                if (result) return Ok(new { succeeded = true, message = "Updated" });            
                return Ok(new { succeeded = false, message = "Failed to update Room! Dont found RoomType ID" });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRoom([FromRoute] string id)
        {
            var result = await _unitOfWork.RoomRepository.DeleteAsync(id);
            if (result)
            {
                await _unitOfWork.SaveEntitiesAsync();
                return Ok(new { succeeded = true, message = "Deleted one record !" });
            }
            return NotFound();
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult> DeleteAllRoomTypes()
        {
            try
            {
                var result = await _roomService.DeleteAllRoomsAsync();
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Deleted all records !" });
                }
                return Ok(new { succeeded = false, message = "Failed to delete All RoomTypes!" });
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
