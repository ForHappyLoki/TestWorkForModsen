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
