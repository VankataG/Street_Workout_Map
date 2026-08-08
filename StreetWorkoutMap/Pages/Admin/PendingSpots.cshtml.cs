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

        private readonly IWorkoutSpotUpdateRequestService workoutSpotUpdateRequestService;

        public PendingSpotsModel(IWorkoutSpotService workoutSpotService, IWorkoutSpotUpdateRequestService workoutSpotUpdateRequestService)
        {
            this.workoutSpotService = workoutSpotService;
            this.workoutSpotUpdateRequestService = workoutSpotUpdateRequestService;
        }


        public ICollection<PendingRequestDto> PendingRequests { get; set; } = [];

        public async Task OnGet()
        {
            var newSpots = await workoutSpotService.GetPendingSpotsAsync();

            var updateRequests = await workoutSpotUpdateRequestService.GetPendingRequestsAsync();

            PendingRequests = newSpots
                        .Concat(updateRequests)
                        .OrderBy(request => request.Name)
                        .ToList();
        }
    }
}
