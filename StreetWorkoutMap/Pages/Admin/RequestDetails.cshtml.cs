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

        public SpotDetailsDto? OriginalSpot { get; private set; }

        public bool IsUpdateRequest { get; private set; }


        public async Task<IActionResult> OnGet(Guid id, bool isUpdate)
        {
            IsUpdateRequest = isUpdate;

            if (IsUpdateRequest)
            {
                Request = await workoutSpotUpdateRequestService.GetDetailsAsync(id);

                if (Request is null)
                {
                    return NotFound();
                }

                var originalSpotId = await workoutSpotUpdateRequestService.GetOriginalSpotIdAsync(id);

                if (originalSpotId is null)
                {
                    return NotFound();
                }

                OriginalSpot = await workoutSpotService.GetDetailsAsync(originalSpotId.Value, User);

                if (OriginalSpot is null)
                {
                    return NotFound();
                }
            }
            else
            {
                Request = await workoutSpotService.GetDetailsAsync(id, User);

                if (Request is null)
                {
                    return NotFound();
                }
            }

            

            return Page();
        }


        public async Task<IActionResult> OnPostApproveAsync(Guid id, bool isUpdate)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (isUpdate)
            {
                await workoutSpotUpdateRequestService.ApproveAsync(id);
            }
            else
            {
                await workoutSpotService.ApproveAsync(id, User);
            }

            TempData["SuccessMessage"] = "Заявката беше одобрена успешно.";

            return RedirectToPage("/Admin/PendingSpots");
        }
    }
}
