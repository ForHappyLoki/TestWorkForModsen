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
    public class ConnectorEventUserProfile : Profile
    {
        public ConnectorEventUserProfile()
        {
            CreateMap<ConnectorEventUser, ConnectorEventUserResponseDto>();
            CreateMap<Event, EventBriefDto>();
            CreateMap<User, UserBriefDto>();
        }
    }

}
