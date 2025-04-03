using AutoMapper;
using TestWorkForModsen.Models;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Data.Repository;
using TestWorkForModsen.Core.Exceptions;
using System.Net;
using TestWorkForModsen.Repository;
using TestWorkForModsen.Data.Models;

namespace TestWorkForModsen.Services.Services
{
    public class ConnectorEventUserService : IConnectorEventUserService
    {
        private readonly IConnectorEventUserRepository<ConnectorEventUser> _repository;
        private readonly IMapper _mapper;
        private readonly IEventRepository<Event> _eventRepository;
        private readonly IUserRepository<User> _userRepository;

        public ConnectorEventUserService(
            IConnectorEventUserRepository<ConnectorEventUser> repository,
            IMapper mapper,
            IEventRepository<Event> eventRepository,
            IUserRepository<User> userRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<ConnectorEventUserResponseDto>> GetAllAsync()
        {
            var records = await _repository.GetAllAsync();
            return records.Any()
                ? _mapper.Map<IEnumerable<ConnectorEventUserResponseDto>>(records)
                : throw new CustomNotFoundException("Не найдено ни одной записи");
        }

        public async Task<ConnectorEventUserResponseDto> GetByCompositeKeyAsync(int eventId, int userId)
        {
            var record = await _repository.GetByCompositeKeyAsync(eventId, userId);
            if (record == null)
            {
                throw new CustomNotFoundException($"Запись с eventId = {eventId} и userId = {userId} не найдена");
            }
            return _mapper.Map<ConnectorEventUserResponseDto>(record);
        }

        public async Task<IEnumerable<ConnectorEventUserResponseDto>> GetAllByUserIdAsync(int userId)
        {
            var records = await _repository.GetAllByUserIdAsync(userId);
            return records.Any()
                ? _mapper.Map<IEnumerable<ConnectorEventUserResponseDto>>(records)
                : throw new CustomNotFoundException($"Не найдено записей для пользователя с ID {userId}");
        }

        public async Task<IEnumerable<ConnectorEventUserResponseDto>> GetAllByEventIdAsync(int eventId)
        {
            var records = await _repository.GetAllByEventIdAsync(eventId);
            return records.Any()
                ? _mapper.Map<IEnumerable<ConnectorEventUserResponseDto>>(records)
                : throw new CustomNotFoundException($"Не найдено записей для события с ID {eventId}");
        }

        public async Task<ConnectorEventUserResponseDto> CreateAsync(ConnectorEventUserCreateDto dto)
        {
            var existingEvent = await _eventRepository.GetByIdAsync(dto.EventId)
                ?? throw new CustomNotFoundException($"Событие с ID {dto.EventId} не найдено");

            var existingUser = await _userRepository.GetByIdAsync(dto.UserId)
                ?? throw new CustomNotFoundException($"Пользователь с ID {dto.UserId} не найден");

            var existingRelation = await _repository.GetByCompositeKeyAsync(dto.EventId, dto.UserId);
            if (existingRelation != null)
            {
                throw new CustomConflictException($"Связь между событием {dto.EventId} и пользователем {dto.UserId} уже существует");
            }

            var entity = new ConnectorEventUser
            {
                EventId = dto.EventId,
                UserId = dto.UserId,
                AdditionTime = DateTime.UtcNow
            };

            try
            {
                await _repository.AddAsync(entity);
                var createdEntity = await _repository.GetByCompositeKeyAsync(dto.EventId, dto.UserId);
                return _mapper.Map<ConnectorEventUserResponseDto>(createdEntity);
            }
            catch (Exception ex)
            {
                throw new CustomDatabaseException("Ошибка при создании связи", ex);
            }
        }

        public async Task DeleteByCompositeKeyAsync(int eventId, int userId, CancellationToken cancellationToken = default)
        {
            var existingRelation = await _repository.GetByCompositeKeyAsync(eventId, userId)
                ?? throw new CustomNotFoundException($"Связь между событием {eventId} и пользователем {userId} не найдена");

            try
            {
                await _repository.DeleteByCompositeKeyAsync(existingRelation, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new CustomDatabaseException("Ошибка при удалении связи", ex);
            }
        }

        public async Task<IEnumerable<ConnectorEventUserResponseDto>> GetPagedAsync(PaginationDto pagination)
        {
            var records = await _repository.GetPagedAsync(pagination.PageNumber, pagination.PageSize);
            return records.Any()
                ? _mapper.Map<IEnumerable<ConnectorEventUserResponseDto>>(records)
                : throw new CustomNotFoundException("Не найдено записей для указанной страницы");
        }
    }
}