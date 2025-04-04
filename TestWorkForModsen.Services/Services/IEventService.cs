﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Services.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventResponseDto>> GetAllAsync();
        Task<EventResponseDto?> GetByIdAsync(int id);
        Task<EventResponseDto> CreateAsync(EventCreateDto dto);
        Task UpdateAsync(EventUpdateDto dto, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<EventResponseDto>> GetPagedAsync(PaginationDto pagination);
    }
}
