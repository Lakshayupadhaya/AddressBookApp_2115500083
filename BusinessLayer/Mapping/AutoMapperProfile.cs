using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ModelLayer.DTO;
using RepositoryLayer.Entity;

namespace BusinessLayer.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //AutoMapping from AddressBookDto to AddressBookEntity and vice a versa
            CreateMap<AddressBookDTO, AddressBookEntity>().ReverseMap();
            CreateMap<AddressBookEntity, CreateContactDTO>();
        }
    }
}
