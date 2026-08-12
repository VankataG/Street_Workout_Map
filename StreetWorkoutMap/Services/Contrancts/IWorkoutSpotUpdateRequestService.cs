using StreetWorkoutMap.DTOs.WorkoutSpot;
using StreetWorkoutMap.Models;

namespace StreetWorkoutMap.Services.Contrancts
{
    public interface IWorkoutSpotUpdateRequestService
    {
        public Task SubmitAsync(EditSpotDto dto, WorkoutSpot spot, string userId);

        public Task<ICollection<PendingRequestDto>> GetPendingRequestsAsync();

        public Task<int> GetPendingRequestsCountAsync();

        public Task<SpotDetailsDto?> GetDetailsAsync(Guid id);

        public Task<Guid?> GetOriginalSpotIdAsync(Guid requestId);

        public Task ApproveAsync(Guid requestId);

        public Task RejectAsync(Guid requestId);

    }
}
