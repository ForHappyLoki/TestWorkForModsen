using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Services.Services
{
    public interface IConnectorEventUserService
    {
        Task<IEnumerable<ConnectorEventUserResponseDto>> GetAllAsync();
        Task<ConnectorEventUserResponseDto?> GetByCompositeKeyAsync(int eventId, int userId);
        Task<IEnumerable<ConnectorEventUserResponseDto>> GetAllByUserIdAsync(int userId);
        Task<IEnumerable<ConnectorEventUserResponseDto>> GetAllByEventIdAsync(int eventId);
        Task<ConnectorEventUserResponseDto> CreateAsync(ConnectorEventUserCreateDto dto);
        Task DeleteByCompositeKeyAsync(int eventId, int userId, CancellationToken cancellationToken);
        Task<IEnumerable<ConnectorEventUserResponseDto>> GetPagedAsync(PaginationDto pagination);
    }
}
