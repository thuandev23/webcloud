namespace QLKhachSanAPI.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Domains;
    using DataAccess;
    using Models.DTOs;
    using Services.Interfaces;
    using QLKhachSanAPI.Services.Implements;


    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGuestService _guestService;

        public GuestController(IUnitOfWork unitOfWork, IGuestService guestService)
        {
            _unitOfWork = unitOfWork;
            _guestService = guestService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Guest>>> GetGuest() 
        {

            var guest = await _unitOfWork.GuestRepository.GetAsync();
            return Ok(guest);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Guest>> GetGuest([FromRoute]string id)
        {
            var guest = await _unitOfWork.GuestRepository.GetSingleAsync(id);
            if (guest == null)
            {
                return NotFound();
            }
            return Ok(guest);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<Guest>> GetGuestByRoom([FromQuery] string IDRoom)
        {
            var guest = await _guestService.GetGuestByRoom(IDRoom);
            if (guest == null)
            {
                return NotFound();
            }
            return Ok(guest);
        }


        [HttpPost]
        public async Task<ActionResult> AddGuestAsync([FromBody] GuestVM model)
        {
            try
            {
                var result = await _guestService.CreateGuestAsync(model);
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
        public async Task<ActionResult> UpdateGuestAsync([FromBody] GuestVM model)
        {
            try
            {
                // Check if the model is valid
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _guestService.UpdateGuestAsync(model);
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Updated" });
                }
                return Ok(new { succeeded = false, message = "Failed to update Guest!" });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGuest([FromRoute] string id)
        {
            var result = await _unitOfWork.GuestRepository.DeleteAsync(id);     
            if (result)
            {
                await _unitOfWork.SaveEntitiesAsync();
                return Ok(new { succeeded = true, message = "Deleted one record !"});
            }
            return NotFound();
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAllGuests()
        {
            try
            {
                var result = await _guestService.DeleteAllGuestsAsync();
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Deleted all records !" });
                }
                return Ok(new { succeeded = false, message = "Failed to delete All Guests!" });
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
