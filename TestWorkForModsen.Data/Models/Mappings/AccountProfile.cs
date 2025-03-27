using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TestWorkForModsen.Models;
using TestWorkForModsen.Data.Models.DTOs;
using AutoMapper;

namespace TestWorkForModsen.Data.Models.Mappings
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            // CreateMap<Source, Destination>()
            CreateMap<AccountDto, Account>()
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<Account, AccountResponseDto>();
        }
    }
}
