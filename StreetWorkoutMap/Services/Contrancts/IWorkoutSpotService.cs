using StreetWorkoutMap.DTOs;
using StreetWorkoutMap.DTOs.WorkoutSpot;
using System.Security.Claims;

namespace StreetWorkoutMap.Services.Contrancts
{
    public interface IWorkoutSpotService
    {

        public Task<ICollection<MapSpotDto>> GetAllApprovedAsync();

        public Task CreateAsync(CreateSpotDto dto, ClaimsPrincipal user);

        public Task<SpotDetailsDto?> GetDetailsAsync(Guid id, ClaimsPrincipal user);

        public Task<EditSpotDto?> GetForEditAsync(Guid id, ClaimsPrincipal user);

        public Task EditAsync(EditSpotDto dto, ClaimsPrincipal user);

        public Task<ICollection<MySpotDto>> GetMySpotsAsync(ClaimsPrincipal user);
    }
}
