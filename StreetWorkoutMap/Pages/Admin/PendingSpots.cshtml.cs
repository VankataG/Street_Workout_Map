using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreetWorkoutMap.DTOs.WorkoutSpot;
using StreetWorkoutMap.Services.Contrancts;

namespace StreetWorkoutMap.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class PendingSpotsModel : PageModel
    {
        private readonly IWorkoutSpotService workoutSpotService;

        public PendingSpotsModel(IWorkoutSpotService workoutSpotService)
        {
            this.workoutSpotService = workoutSpotService;
        }


        public ICollection<PendingSpotDto> PendingSpots { get; set; } = [];

        public async Task OnGet()
        {
            PendingSpots = await workoutSpotService.GetPendingSpotsAsync();
        }
    }
}
