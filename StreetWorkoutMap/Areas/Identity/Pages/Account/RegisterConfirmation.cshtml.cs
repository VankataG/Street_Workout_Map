#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StreetWorkoutMap.Data;

namespace StreetWorkoutMap.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterConfirmationModel(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string Email { get; set; }

        public async Task<IActionResult> OnGetAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return RedirectToPage("/Index");
            }

            Email = email;

            return Page();
        }
    }
}