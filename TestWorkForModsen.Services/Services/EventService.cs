using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Models;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Data.Repository.BasicInterfaces;
using TestWorkForModsen.Data.Repository;

namespace TestWorkForModsen.Services.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository<Event> _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<EventCreateDto> _createValidator;
        private readonly IValidator<EventUpdateDto> _updateValidator;

        public EventService(
            IEventRepository<Event> repository,
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
            if (eventEntity == null)
            {
                throw new Exception($"Событие с ID {id} не найдено");
            }
            return eventEntity == null ? null : _mapper.Map<EventResponseDto>(eventEntity);
        }

        public async Task<EventResponseDto> CreateAsync(EventCreateDto dto)
        {
            try
            {
                await _createValidator.ValidateAndThrowAsync(dto);

                var eventEntity = _mapper.Map<Event>(dto);
                await _repository.AddAsync(eventEntity);

                return _mapper.Map<EventResponseDto>(eventEntity);
            }
            catch (Exception ex) 
            {
                throw new Exception($"Ошибка при создании события, {ex.Message}");
            }
        }

        public async Task UpdateAsync(EventUpdateDto dto)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);

            var eventEntity = await _repository.GetByIdAsync(dto.Id);
            if (eventEntity == null) throw new KeyNotFoundException("Событие не найдено");

            _mapper.Map(dto, eventEntity);
            await _repository.UpdateAsync(eventEntity);
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при удалении события, {ex.Message}");
            }
        }
        public async Task<IEnumerable<EventResponseDto>> GetPagedAsync(PaginationDto pagination)
        {
            var events = await _repository.GetPagedAsync(pagination.PageNumber, pagination.PageSize);
            return _mapper.Map<IEnumerable<EventResponseDto>>(events);
        }
    }
}
