namespace QLKhachSanAPI.Services.Implements
{
    using Microsoft.AspNetCore.Identity;
    using Models.DTOs;
    using Models;
    using Services.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using QLKhachSanAPI.DataAccess;

    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }


        public async Task<bool> CreateStaffAccountAsyncEF(CreateEditStaffAccountModel model)
        {
           
            var check = await _unitOfWork.ApplicationUserRepository.GetSingleAsync(d=>d.UserName == model.UserName);
            if(check != null)
            {
                return false;
            }
            List<object> errors = new List<object>(14);

            string defaultRole = "Staff";  // initial default role
            string defaultPassword = "Abc@123";

            // direct assign (use tinymapper if you want)
            var applicationUser = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName
                // DateJoined is automatically initialized in ApplicationUser class
            };

            try
            {
                // create role if not exist:
                bool isRoleExist = await _roleManager.RoleExistsAsync(defaultRole); // take in roleName(string)
                if (!isRoleExist)
                {
                    var newRole = new IdentityRole(defaultRole);
                    IdentityResult rsNewRole = await _roleManager.CreateAsync(newRole);
                    if (!rsNewRole.Succeeded)
                    {
                        errors.AddRange(rsNewRole.Errors);
                    }
                }

                // Try to create the user
                IdentityResult rsRegister = await _userManager.CreateAsync(applicationUser, defaultPassword);

                if (rsRegister.Succeeded)
                {
                    // assign role to user:
                    await _userManager.AddToRoleAsync(applicationUser, defaultRole);

                    #region (Background Job Scheduling) Mail Send
                    //BackgroundJob.Enqueue(() => SendConfirmationEmail(applicationUser));
                    #endregion

                    //_logger.LogInformation($"\nNew user registered (UserName: {applicationUser.UserName})\n");
                }
                else
                {      
                    errors.AddRange(rsRegister.Errors);
                }


                return true;
            }
            catch (Exception ex)
            {
                //throw ex; // cause a mess
                errors.Add(ex.Message);
                Console.WriteLine(errors.ToString());
                return false;
            }

        }

        public async Task<bool> ModifyStaffAccountAsyncEF(CreateEditStaffAccountModel model)
        {
            var check = await _unitOfWork.ApplicationUserRepository.GetSingleAsync(d => d.UserName == model.UserName);
            if (check != null)
            {
                return false;
            }
            var userToModify = await _userManager.FindByIdAsync(model.IdToUpdate!);
            var errors = new List<IdentityError>();

            if (userToModify != null)
            {
                userToModify.UserName = model.UserName;
                userToModify.Email = model.Email;
                userToModify.FullName = model.FullName;

                IdentityResult result = await _userManager.UpdateAsync(userToModify);
                if (result.Succeeded)
                {
                    return true;
                }

                errors.AddRange(result.Errors);
                Console.WriteLine(errors.ToString());

                return false;
            }

            Console.WriteLine("User to be modified not found!");
            return false;
        }


        public async Task<bool> DeleteStaffAccountAsyncEF(string userId)
        {
            var userToDelete = await _userManager.FindByIdAsync(userId);
            var errors = new List<IdentityError>();

            if (userToDelete != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(userToDelete);
                if (result.Succeeded)
                {
                    return true;
                }

                errors.AddRange(result.Errors);
                Console.WriteLine(errors.ToString() );

                return false;
            }

            Console.WriteLine("User to be deleted not found!");
            return false;
        }

        public async Task<List<ApplicationUserViewModel>> GetAllUserWithRole()
        {
            var users = await _userManager.Users.OrderByDescending(u => u.DateJoined).ToListAsync();

            var usersWithRoles = new List<ApplicationUserViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var userWithRoles = new ApplicationUserViewModel
                {
                    // Cập nhật các thuộc tính tương ứng trong ApplicationUserViewModel
                    IdToUpdate=user.Id,
                    FullName=user.FullName,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    Roles = roles.ToList(),
                    DateJoined = user.DateJoined
                };

                usersWithRoles.Add(userWithRoles);
            }

            return usersWithRoles;
        }

        public async Task<ApplicationUserViewModel> GetUser(string id)
        {
            var user= await _unitOfWork.ApplicationUserRepository.GetSingleAsync(d => d.Id == id);
            return new ApplicationUserViewModel
            {
                IdToUpdate=id,
                UserName=user.UserName,
                Email =user.Email,
                FullName= user.FullName,
            };
        }
    }
}
