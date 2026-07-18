using StreetWorkoutMap.DTOs;
using StreetWorkoutMap.DTOs.WorkoutSpot;
using System.Security.Claims;

namespace StreetWorkoutMap.Services.Contrancts
{
    public interface IWorkoutSpotService
    {

        public Task<ICollection<MapSpotDto>> GetAllApprovedAsync();

        public Task CreateAsync(CreateSpotDto dto, ClaimsPrincipal user);


    }
}
