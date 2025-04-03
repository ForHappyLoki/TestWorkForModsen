using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Data.Models;

namespace TestWorkForModsen.Services.Mappings
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<EventCreateDto, Event>();
            CreateMap<EventUpdateDto, Event>();
            CreateMap<EventDto, Event>();
            CreateMap<Event, EventResponseDto>();
            CreateMap<ConnectorEventUser, ConnectorEventUserResponseDto>()
                .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event));
        }
    }
}
