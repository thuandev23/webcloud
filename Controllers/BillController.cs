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
    public class BillController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBillService _billService;

        public BillController(IUnitOfWork unitOfWork, IBillService billService)
        {
            _unitOfWork = unitOfWork;
            _billService = billService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Bill>>> GetBill()
        {

            var bills = await _billService.GetAllBillsAsync();
            return Ok(bills);
        }



        [HttpGet("[action]")]
        public async Task<ActionResult<List<Bill>>> GetBillByIDGuest([FromQuery] string id)
        {
            var bills = await _billService.GetBillsByIDGuest(id);
            return Ok(bills);
        }

        

        [HttpPost]
        public async Task<ActionResult> AddBillAsync([FromBody] BillVM model)
        {
            try
            {
                var result = await _billService.CreateBillAsync(model);
                if (result) return Ok(new { succeeded = true, message = "Created" });           
                return Ok(new { succeeded = false, message = "Dont found ID Guest" });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateBillAsync([FromBody] BillVM model)
        {
            try
            {
                // Check if the model is valid
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = await _billService.UpdateBillAsync(model);
                if (result) return Ok(new { succeeded = true, message = "Updated" });
               
                return Ok(new { succeeded = false, message = "Failed to update Bill! Dont found or Guest ID" });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Ok(new { succeeded = false, message = ex.Message });
            }
        }

    }
}
