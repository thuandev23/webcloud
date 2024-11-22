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
    public class RoomTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomTypeService _roomTypeService;

        public RoomTypeController(IUnitOfWork unitOfWork, IRoomTypeService roomTypeService)
        {
            _unitOfWork = unitOfWork;
            _roomTypeService = roomTypeService;
        }

        //[HttpGet]
        //public async Task<ActionResult<List<RoomType>>> GetAllRoomTypes() 
        //{

        //    var roomTypes = await _unitOfWork.RoomTypeRepository.GetAsync();
        //    return Ok(roomTypes);
        //}

        [HttpGet]
        public async Task<ActionResult<List<RoomType>>> GetAllRoomTypesLinQ()
        {

            var rooms = await _roomTypeService.GetAllRoomTypesLinQAsync();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomType>> GetRoomType([FromRoute]string id)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.GetSingleAsync(id);
            if (roomType == null)
            {
                return NotFound();
            }
            return Ok(roomType);
        }

       
        [HttpPost]
        public async Task<ActionResult> AddRoomTypeAsync([FromBody] RoomTypeVM model)
        {
            try
            {
                var result = await _roomTypeService.CreateRoomTypeAsync(model);
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Created" });
                }
                return Ok(new { succeeded = false, message = "Failed to add DrinkType!" });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateRoomTypeAsync([FromBody] RoomTypeVM model)
        {
            try
            {
                // Check if the model is valid
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _roomTypeService.UpdateRoomTypeAsync(model);
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Updated" });
                }
                return Ok(new { succeeded = false, message = "Failed to update RoomType!" });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRoomType([FromRoute] string id)
        {
            var result = await _unitOfWork.RoomTypeRepository.DeleteAsync(id);     
            if (result)
            {
                await _unitOfWork.SaveEntitiesAsync();
                return Ok(new { succeeded = true, message = "Deleted one record !"});
            }
            return NotFound();
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAllRoomTypes()
        {
            try
            {
                var result = await _roomTypeService.DeleteAllRoomTypesAsync();
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
