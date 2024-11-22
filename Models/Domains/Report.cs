
namespace QLKhachSanAPI.Models.Domains
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
    public class Report
    {
        [Key]
        public string idGuest { get; set; }

        public string mess {  get; set; }
    }
}
