using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreetWorkoutMap.DTOs.WorkoutSpot;
using StreetWorkoutMap.Services.Contrancts;

namespace StreetWorkoutMap.Pages.Spots;

[Authorize]
public class EditModel : PageModel
{
    private readonly IWorkoutSpotService workoutSpotService;

    public EditModel(IWorkoutSpotService workoutSpotService)
    {
        this.workoutSpotService = workoutSpotService;
    }

    [BindProperty]
    public EditSpotDto Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var spot = await workoutSpotService.GetForEditAsync(id, User);

        if (spot is null)
        {
            return NotFound();
        }

        Input = spot;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await ReloadExistingImagesAsync();
            return Page();
        }

        try
        {
            await workoutSpotService.EditAsync(Input, User);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await ReloadExistingImagesAsync();
            return Page();
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await ReloadExistingImagesAsync();
            return Page();
        }

        TempData["SuccessMessage"] =
            "Площадката беше редактирана успешно.";

        return RedirectToPage(
            "/Spots/Details",
            new { id = Input.Id });
    }

    private async Task ReloadExistingImagesAsync()
    {
        var originalSpot = await workoutSpotService
            .GetForEditAsync(Input.Id, User);

        if (originalSpot is null)
        {
            return;
        }

        Input.ExistingImages = originalSpot.ExistingImages;

        var validExistingImageIds = originalSpot.ExistingImages
            .Select(image => image.Id)
            .ToHashSet();

        Input.ImagesToDelete = Input.ImagesToDelete
            .Where(validExistingImageIds.Contains)
            .Distinct()
            .ToList();
    }
}
