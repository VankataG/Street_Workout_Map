using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreetWorkoutMap.DTOs.WorkoutSpot;
using StreetWorkoutMap.Services.Contrancts;

namespace StreetWorkoutMap.Pages.Spots
{


    [Authorize]
    public class EditModel : PageModel
    {
        [BindProperty]
        public EditSpotDto Input { get; set; } = new();

        private readonly IWorkoutSpotService workoutSpotService;

        public EditModel(IWorkoutSpotService workoutSpotService)
        {
            this.workoutSpotService = workoutSpotService;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var dto = await workoutSpotService.GetForEditAsync(id, User);

            if (dto is null)
            {
                return NotFound();
            }

            Input = dto;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await workoutSpotService.EditAsync(Input, User);

            TempData["SuccessMessage"] = "Workout spot updated successfully.";

            return RedirectToPage("/Spots/Details", new { id = Input.Id });
        }
    }
}
