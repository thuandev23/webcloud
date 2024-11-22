namespace QLKhachSanAPI.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.DTOs.Pagination;
    using Microsoft.EntityFrameworkCore;
    using Models.DTOs;
    using Services.Interfaces;
    using Azure;

    //[Authorize(Roles = "Admin")]
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAdminService _adminService;

        public AdminController(UserManager<ApplicationUser> userManager, IAdminService adminService)
        {
            _userManager = userManager;
            _adminService = adminService;
        }

        // no pagination GET
        [HttpGet("users")]
        public async Task<IActionResult> GetUsersNoPagination()
        {
            var userWithRole = await _adminService.GetAllUserWithRole();
            return Ok(userWithRole);
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUser([FromRoute] string id)
        {
            var user = await _adminService.GetUser(id);
            return Ok(user);
        }


        // POST : https://localhost:7000/api/Admin/user/getallusers
        [HttpPost("user/getallusers")]
        public async Task<IActionResult> GetAllUsers([FromBody] PaginationModel model)
        {
            try
            {
                var users = await _userManager.Users.ToListAsync(); // later
                var result = await PaginatedList<ApplicationUser>.CreateAsync(users, model.pageNumber, model.itemsPerPage);
                var response = new PaginatedResponse<ApplicationUser>
                {
                    Pages = result.TotalPages,
                    Page = result.PageNumber,
                    Total = result.TotalCount,
                    Data = result,
                    Count = result.Count,
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("user/create-staff-account")]
        public async Task<IActionResult> CreateStaffAccountAsync([FromBody] CreateEditStaffAccountModel model)
        {
            try
            {
                var result = await _adminService.CreateStaffAccountAsyncEF(model);
                if (result)
                    return Ok(new
                    {
                        succeeded = true
                    });

                return Ok(new
                {
                    succeeded = false,
                    mess= "user already exist"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("user/edit-staff-account")]
        public async Task<IActionResult> ModifyStaffAccountAsync([FromBody] CreateEditStaffAccountModel model)
        {
            try
            {
                var result = await _adminService.ModifyStaffAccountAsyncEF(model);
                if (result)
                    return Ok(new
                    {
                        succeeded = true
                    });

                return Ok(new
                {
                    succeeded = false,
                    mess = "user already exist"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("user/remove-staff-account")]
        public async Task<IActionResult> RemoveStaffAccountAsync(string userId)
        {
            try
            {
                var result = await _adminService.DeleteStaffAccountAsyncEF(userId);
                if (result)
                    return Ok(new
                    {
                        succeeded = true
                    });

                return Ok(new
                {
                    succeeded = false
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
