using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreetWorkoutMap.DTOs.WorkoutSpot;

namespace StreetWorkoutMap.Pages.Spots
{
    [Authorize]
    public class CreateModel : PageModel
    {
        [BindProperty]
        public CreateSpotDto Input { get; set; } = new();
        public void OnGet()
        {
        }
    }
}
