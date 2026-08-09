using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreetWorkoutMap.DTOs.WorkoutSpot;
using StreetWorkoutMap.Services.Contrancts;

namespace StreetWorkoutMap.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class RequestDetailsModel : PageModel
    {

        private readonly IWorkoutSpotService workoutSpotService;

        private readonly IWorkoutSpotUpdateRequestService workoutSpotUpdateRequestService;

        public RequestDetailsModel(IWorkoutSpotService workoutSpotService, IWorkoutSpotUpdateRequestService workoutSpotUpdateRequestService)
        {
            this.workoutSpotService = workoutSpotService;
            this.workoutSpotUpdateRequestService = workoutSpotUpdateRequestService;
        }

        public SpotDetailsDto Request { get; private set; } = null!;

        public bool IsUpdateRequest { get; private set; }


        public async Task<IActionResult> OnGet(Guid id, bool isUpdate)
        {
            IsUpdateRequest = isUpdate;

            if (IsUpdateRequest)
            {
                Request = await workoutSpotUpdateRequestService.GetDetailsAsync(id);
            }
            else
            {
                Request = await workoutSpotService.GetDetailsAsync(id, User);
            }

            if (Request is null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
