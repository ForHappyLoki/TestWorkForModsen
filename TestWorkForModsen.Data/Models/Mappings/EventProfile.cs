using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkForModsen.Models;
using TestWorkForModsen.Data.Models.DTOs;

namespace TestWorkForModsen.Data.Models.Mappings
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
