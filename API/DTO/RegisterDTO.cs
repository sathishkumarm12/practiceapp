using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class RegisterDTO
    {
        [Required] public string username { get; set; }
        [Required] public string KnowAs { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }
        [Required] [StringLength(15, MinimumLength = 4)] public string password { get; set; }
    }
}