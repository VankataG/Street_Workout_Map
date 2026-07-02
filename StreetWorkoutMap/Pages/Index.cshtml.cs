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
            Spots =
            [
                new()
                {
                    Name = "Марно поле",
                    Latitude = 43.082,
                    Longitude = 25.629
                },

                new()
                {
                    Name = "Картала",
                    Latitude = 43.071,
                    Longitude = 25.640
                }
            ];
        }
    }
}
