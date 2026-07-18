using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreetWorkoutMap.DTOs.WorkoutSpot;
using StreetWorkoutMap.Services;
using StreetWorkoutMap.Services.Contrancts;

namespace StreetWorkoutMap.Pages.Spots
{
    [Authorize]
    public class CreateModel : PageModel
    {
        [BindProperty]
        public CreateSpotDto Input { get; set; } = new();


        private readonly IWorkoutSpotService workoutSpotService;

        public CreateModel(IWorkoutSpotService workoutSpotService)
        {
            this.workoutSpotService = workoutSpotService;
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await workoutSpotService.CreateAsync(Input, User);

            TempData["SuccessMessage"] = "Площадката беше изпратена успешно и очаква одобрение.";

            return RedirectToPage("/Index");
        }



        public void OnGet()
        {
        }
    }
}
