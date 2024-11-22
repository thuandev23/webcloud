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
    public class GuestServiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGuestServiceService _guestServiceService;

        public GuestServiceController(IUnitOfWork unitOfWork, IGuestServiceService guestServiceService)
        {
            _unitOfWork = unitOfWork;
            _guestServiceService = guestServiceService;
        }

        [HttpGet]
        public async Task<ActionResult<List<GuestService>>> GetAllGuestServices()
        { 
            var guestService = await _guestServiceService.GetAllGuestServiceAsync();
            return Ok(guestService);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<GuestService>>> GetGuestServicesByGuestID([FromQuery] string id)
        {
            var guestService = await _unitOfWork.GuestServiceRepository.GetAsync(d=>d.GuestID==id);
            return Ok(guestService);
        }

        

        [HttpPost]
        public async Task<ActionResult> AddGuestService([FromBody] GuestServiceVM model)
        {
            try
            {
                var result = await _guestServiceService.CreateGuestServiceAsync(model);
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Created" });
                }
                return Ok(new { succeeded = false, message = "GuestID or ServiceID not found" });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }

        

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAllGuestServices()
        {
            try
            {
                var result = await _guestServiceService.DeleteAllGuestServiceAsync();
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Deleted all records !" });
                }
                return Ok(new { succeeded = false, message = "Failed to delete !" });
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
