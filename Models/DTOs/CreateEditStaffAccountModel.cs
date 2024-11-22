namespace QLKhachSanAPI.Models.DTOs
{
    public class CreateEditStaffAccountModel
    {
        public string? IdToUpdate { get; set; }
        public string UserName { get; set; } = "Default";
        public string Email { get; set; } = "default@default.com";

        //public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = "A Default User";
    }
}
