using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StreetWorkoutMap.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [PersonalData]
        [MaxLength(50)]
        public string? LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
