using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Models;
using TestWorkForModsen.Repository;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Data.Repository.BasicInterfaces;
using TestWorkForModsen.Data.Repository;

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
            return _mapper.Map<IEnumerable<ConnectorEventUserResponseDto>>(records);
        }

        public async Task<ConnectorEventUserResponseDto?> GetByCompositeKeyAsync(int eventId, int userId)
        {
            var record = await _repository.GetByCompositeKeyAsync(eventId, userId);
            if (record == null)
            {
                throw new Exception($"Запись с eventId={eventId} и userId={userId} не найдена.");
            }
            return record == null ? null : _mapper.Map<ConnectorEventUserResponseDto>(record);
        }

        public async Task<IEnumerable<ConnectorEventUserResponseDto>> GetAllByUserIdAsync(int userId)
        {
            var records = await _repository.GetAllByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<ConnectorEventUserResponseDto>>(records);
        }

        public async Task<IEnumerable<ConnectorEventUserResponseDto>> GetAllByEventIdAsync(int eventId)
        {
            var records = await _repository.GetAllByEventIdAsync(eventId);
            return _mapper.Map<IEnumerable<ConnectorEventUserResponseDto>>(records);
        }

        public async Task<ConnectorEventUserResponseDto> CreateAsync(ConnectorEventUserCreateDto dto)
        {
            if (await _eventRepository.GetByIdAsync(dto.EventId) == null)
                throw new KeyNotFoundException($"Событие с ID {dto.EventId} не найдено");

            if (await _userRepository.GetByIdAsync(dto.UserId) == null)
                throw new KeyNotFoundException($"Пользователь с ID {dto.UserId} не найден");
            try
            {
                var entity = new ConnectorEventUser
                {
                    EventId = dto.EventId,
                    UserId = dto.UserId,
                    AdditionTime = DateTime.UtcNow
                };
                await _repository.AddAsync(entity);
                var createdEntity = await _repository.GetByCompositeKeyAsync(dto.EventId, dto.UserId);
                return _mapper.Map<ConnectorEventUserResponseDto>(createdEntity);
            }
            catch (KeyNotFoundException ex)
            {
                throw new Exception($"Не удалось создать запись, {ex.Message}");
            }
        }

        public async Task DeleteByCompositeKeyAsync(int eventId, int userId)
        {
            try
            {
                await _repository.DeleteByCompositeKeyAsync(eventId, userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Не удалось удалить запись, {ex.Message}");
            }
        }

        public async Task<IEnumerable<ConnectorEventUserResponseDto>> GetPagedAsync(PaginationDto pagination)
        {
            var records = await _repository.GetPagedAsync(pagination.PageNumber, pagination.PageSize);
            return _mapper.Map<IEnumerable<ConnectorEventUserResponseDto>>(records);
        }
    }
}
