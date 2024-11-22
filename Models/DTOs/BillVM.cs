using System.ComponentModel.DataAnnotations;

namespace QLKhachSanAPI.Models.DTOs
{
    public class BillVM
    {
        public string? IdToUpdate { get; set; }
        public double Sum { get; set; }
        public bool Status { get; set; }
        public string IDGuest { get; set; }
       
    }
}


