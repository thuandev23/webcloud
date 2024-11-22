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
    public class ReportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportService _reportService;

        public ReportController(IUnitOfWork unitOfWork, IReportService reportService)
        {
            _unitOfWork = unitOfWork;
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Report>>> GetReport() 
        {

            var report = await _reportService.GetAllReportAsync();
            return Ok(report);
        }

       

        [HttpPost]
        public async Task<ActionResult> AddReportAsync([FromBody] Report model)
        {
            try
            {
                var result = await _reportService.CreateReportAsync(model);
                if (result)
                {
                    return Ok(new { succeeded = true, message = "Created" });
                }
                return Ok(new { succeeded = false, message = "Failed to add Report!" });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }

        

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReport([FromRoute] string id)
        {
            var result = await _reportService.DeleteReportAsync(id);  
            if (result)
            {
               
                return Ok(new { succeeded = true, message = "Deleted one record !"});
            }
            return NotFound();
        }

       
    }
}
