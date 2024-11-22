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
    public class ServiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceService _serviceService;

        public ServiceController(IUnitOfWork unitOfWork, IServiceService serviceService)
        {
            _unitOfWork = unitOfWork;
            _serviceService = serviceService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Service>>> GetAllService()
        {
            var services = await _serviceService.GetAllServiceAsync();
            return Ok(services);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService([FromRoute]string id)
        {
            var service = await _unitOfWork.ServiceRepository.GetSingleAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return Ok(service);
        }

       
        [HttpPost]
        public async Task<ActionResult> AddServiceAsync([FromBody] ServiceVM model)
        {
            try
            {
                var result = await _serviceService.CreateServiceAsync(model);
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Created" });
                }
                return Ok(new { succeeded = false, message = "Failed to add Service!" });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateServiceAsync([FromBody] ServiceVM model)
        {
            try
            {
                // Check if the model is valid
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _serviceService.UpdatServiceAsync(model);
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Updated" });
                }
                return Ok(new { succeeded = false, message = "Failed to update Service!" });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteService([FromRoute] string id)
        {
            var result = await _serviceService.DeleteServiceAsync(id);     
            if (result)
            {
                await _unitOfWork.SaveEntitiesAsync();
                return Ok(new { succeeded = true, message = "Deleted one record !"});
            }
            return NotFound();
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAllServices()
        {
            try
            {
                var result = await _serviceService.DeleteAllServiceAsync();
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
