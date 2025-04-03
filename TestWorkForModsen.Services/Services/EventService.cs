using AutoMapper;
using FluentValidation;
using TestWorkForModsen.Core.Exceptions;
using TestWorkForModsen.Data.Models;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Data.Repository;
using TestWorkForModsen.Models;

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
            return events.Any()
                ? _mapper.Map<IEnumerable<EventResponseDto>>(events)
                : throw new CustomNotFoundException("События не найдены");
        }

        public async Task<EventResponseDto> GetByIdAsync(int id)
        {
            var eventEntity = await _repository.GetByIdAsync(id);
            return eventEntity != null
                ? _mapper.Map<EventResponseDto>(eventEntity)
                : throw new CustomNotFoundException($"Событие с ID {id} не найдено");
        }

        public async Task<EventResponseDto> CreateAsync(EventCreateDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            try
            {
                var eventEntity = _mapper.Map<Event>(dto);
                await _repository.AddAsync(eventEntity);
                return _mapper.Map<EventResponseDto>(eventEntity);
            }
            catch (Exception ex)
            {
                throw new CustomDatabaseException("Ошибка при создании события", ex);
            }
        }

        public async Task UpdateAsync(EventUpdateDto dto, CancellationToken cancellationToken = default)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);

            var eventEntity = await _repository.GetByIdAsync(dto.Id)
                ?? throw new CustomNotFoundException($"Событие с ID {dto.Id} не найдено");

            try
            {
                _mapper.Map(dto, eventEntity);
                await _repository.UpdateAsync(eventEntity);
            }
            catch (Exception ex)
            {
                throw new CustomDatabaseException("Ошибка при обновлении события", ex);
            }
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var eventExists = await _repository.GetByIdAsync(id);
            if (eventExists == null)
            {
                throw new CustomNotFoundException($"Событие с ID {id} не найдено");
            }

            try
            {
                await _repository.DeleteAsync(eventExists, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new CustomDatabaseException("Ошибка при удалении события", ex);
            }
        }

        public async Task<IEnumerable<EventResponseDto>> GetPagedAsync(PaginationDto pagination)
        {
            if (pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                throw new CustomBadRequestException("Некорректные параметры пагинации");
            }

            var events = await _repository.GetPagedAsync(pagination.PageNumber, pagination.PageSize);
            return events.Any()
                ? _mapper.Map<IEnumerable<EventResponseDto>>(events)
                : throw new CustomNotFoundException("События не найдены для указанной страницы");
        }
    }
}