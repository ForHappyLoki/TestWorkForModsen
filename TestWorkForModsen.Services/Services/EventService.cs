using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Models;
using TestWorkForModsen.Repository;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Services.Services
{
    public class EventService : IEventService
    {
        private readonly IRepository<Event> _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<EventCreateDto> _createValidator;
        private readonly IValidator<EventUpdateDto> _updateValidator;

        public EventService(
            IRepository<Event> repository,
            IMapper mapper,
            IValidator<EventCreateDto> createValidator,
            IValidator<EventUpdateDto> updateValidator)
        {
            _repository = repository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<EventResponseDto>> GetAllAsync()
        {
            var events = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<EventResponseDto>>(events);
        }

        public async Task<EventResponseDto?> GetByIdAsync(int id)
        {
            var eventEntity = await _repository.GetByIdAsync(id);
            return eventEntity == null ? null : _mapper.Map<EventResponseDto>(eventEntity);
        }

        public async Task<EventResponseDto> CreateAsync(EventCreateDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            var eventEntity = _mapper.Map<Event>(dto);
            await _repository.AddAsync(eventEntity);

            return _mapper.Map<EventResponseDto>(eventEntity);
        }

        public async Task UpdateAsync(int id, EventUpdateDto dto)
        {
            if (id != dto.Id) throw new ArgumentException("ID в пути и теле запроса не совпадают");

            await _updateValidator.ValidateAndThrowAsync(dto);

            var eventEntity = await _repository.GetByIdAsync(id);
            if (eventEntity == null) throw new KeyNotFoundException("Событие не найдено");

            _mapper.Map(dto, eventEntity);
            await _repository.UpdateAsync(eventEntity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<EventResponseDto>> GetPagedAsync(PaginationDto pagination)
        {
            var events = await _repository.GetPagedAsync(pagination.PageNumber, pagination.PageSize);
            return _mapper.Map<IEnumerable<EventResponseDto>>(events);
        }
    }
}
