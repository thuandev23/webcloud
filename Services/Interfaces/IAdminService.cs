namespace QLKhachSanAPI.Services.Interfaces
{
    using QLKhachSanAPI.Models.DTOs;

    public interface IAdminService
    {
        Task<bool> CreateStaffAccountAsyncEF(CreateEditStaffAccountModel model);
        Task<bool> ModifyStaffAccountAsyncEF(CreateEditStaffAccountModel model);
        Task<bool> DeleteStaffAccountAsyncEF(string userId);
        Task<List<ApplicationUserViewModel>> GetAllUserWithRole();
        Task<ApplicationUserViewModel> GetUser(string id);
    }
}
