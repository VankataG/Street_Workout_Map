using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using StreetWorkoutMap.Models;

namespace StreetWorkoutMap.Pages
{
    public class IndexModel : PageModel
    {

        public List<WorkoutSpot> Spots { get; set; } = new();
        public void OnGet()
        {
            
        }
    }
}
