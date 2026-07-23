using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreetWorkoutMap.DTOs.WorkoutSpot;
using StreetWorkoutMap.Services.Contrancts;

namespace StreetWorkoutMap.Pages.Spots
{
    [Authorize]
    public class MySpotsModel : PageModel
    {

        private readonly IWorkoutSpotService workoutSpotService;

        public MySpotsModel(IWorkoutSpotService workoutSpotService)
        {
            this.workoutSpotService = workoutSpotService;
        }


        public ICollection<MySpotDto> MySpots { get; set; } = [];


        public async Task<IActionResult> OnGetAsync()
        {
            MySpots = await workoutSpotService.GetMySpotsAsync(User);

            return Page();
        }
    }
}
