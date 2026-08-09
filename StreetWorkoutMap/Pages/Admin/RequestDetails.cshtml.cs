using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StreetWorkoutMap.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class RequestDetailsModel : PageModel
    {
        public void OnGet(Guid id, bool isUpdate)
        {
        }
    }
}
