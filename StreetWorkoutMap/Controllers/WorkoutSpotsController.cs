using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StreetWorkoutMap.DTOs;
using StreetWorkoutMap.Services;
using StreetWorkoutMap.Services.Contrancts;

namespace StreetWorkoutMap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutSpotsController : ControllerBase
    {

        private readonly IWorkoutSpotService workoutSpotService;

        public WorkoutSpotsController(IWorkoutSpotService workoutSpotService)
        {
            this.workoutSpotService = workoutSpotService;
        }



        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var spots = await workoutSpotService.GetAllApprovedAsync();

            return Ok(spots);
        }


    }
}
