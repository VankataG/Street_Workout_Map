using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreetWorkoutMap.DTOs.WorkoutSpot;
using StreetWorkoutMap.Services.Contrancts;

namespace StreetWorkoutMap.Pages.Spots
{
    public class DetailsModel : PageModel
    {
        private readonly IWorkoutSpotService workoutSpotService;

        public DetailsModel(IWorkoutSpotService workoutSpotService)
        {
            this.workoutSpotService = workoutSpotService;
        }

        public SpotDetailsDto Spot { get; private set; } = null!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var spot = await workoutSpotService.GetDetailsAsync(id, User);

            if (spot is null) return NotFound();

            Spot = spot;

            return Page();
        }


    }
}
