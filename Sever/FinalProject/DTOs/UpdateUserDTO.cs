using FinalProject.Models.CustomUser;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.DTOs
{
    public class UpdateUserDTO
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public AddressDTO? Addresss { get; set; }
    }
}
