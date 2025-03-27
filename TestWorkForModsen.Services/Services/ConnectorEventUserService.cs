using AutoMapper;
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
    public class ConnectorEventUserService : IConnectorEventUserService
    {
        private readonly IConnectorEventUserRepository<ConnectorEventUser> _repository;
        private readonly IMapper _mapper;
        private readonly IRepository<Event> _eventRepository; 
        private readonly IRepository<User> _userRepository;

        public ConnectorEventUserService(
            IConnectorEventUserRepository<ConnectorEventUser> repository,
            IMapper mapper,
            IRepository<Event> eventRepository,
            IRepository<User> userRepository)
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
            // Проверка существования события и пользователя
            if (await _eventRepository.GetByIdAsync(dto.EventId) == null)
                throw new KeyNotFoundException($"Событие с ID {dto.EventId} не найдено");

            if (await _userRepository.GetByIdAsync(dto.UserId) == null)
                throw new KeyNotFoundException($"Пользователь с ID {dto.UserId} не найден");

            var entity = new ConnectorEventUser
            {
                EventId = dto.EventId,
                UserId = dto.UserId,
                AdditionTime = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);

            // Получаем полную запись с навигационными свойствами
            var createdEntity = await _repository.GetByCompositeKeyAsync(dto.EventId, dto.UserId);
            return _mapper.Map<ConnectorEventUserResponseDto>(createdEntity);
        }

        public async Task DeleteByCompositeKeyAsync(int eventId, int userId)
        {
            await _repository.DeleteByCompositeKeyAsync(eventId, userId);
        }

        public async Task<IEnumerable<ConnectorEventUserResponseDto>> GetPagedAsync(PaginationDto pagination)
        {
            var records = await _repository.GetPagedAsync(pagination.PageNumber, pagination.PageSize);
            return _mapper.Map<IEnumerable<ConnectorEventUserResponseDto>>(records);
        }
    }
}
