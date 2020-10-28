using AutoMapper;
using CRM.API.Auth;
using CRM.API.Models.InputModels;
using CRM.API.Models.InputModels.AuthInputModels;
using CRM.API.Models.OutputModels;
using CRM.DB.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CRM.API.Configuration
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<City, CityOutputModel>();
            CreateMap<Lead, LeadOutputModel>()
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.RegistrationDate.ToString(@"dd.MM.yyyy")))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.ToString(@"dd.MM.yyyy")))
                .ForMember(dest => dest.LastUpdateDate, opt => opt.MapFrom(src => src.LastUpdateDate.ToString(@"dd.MM.yyyy")));


            CreateMap<LeadInputModel, Lead>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => new City { Id = src.CityId }))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.BirthDate, "dd.MM.yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.Accounts, opt => opt.MapFrom(src => new List<Account> ()))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => Hashing.HashUserPassword(src.Password)));

            CreateMap<LeadSearchInputModel, LeadSearchModel>()
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate == null ? null : (DateTime?)Convert.ToDateTime(src.BirthDate)))
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.RegistrationDate == null ? null : (DateTime?)Convert.ToDateTime(src.RegistrationDate)));

            CreateMap<AccountInputModel, Account>()
                .ForPath(dest => dest.Currency.Id, opt => opt.MapFrom(src => src.CurrencyId ));


            CreateMap<Account, AccountOutputModel>()
                .ForPath(dest => dest.CurrencyId, opt => opt.MapFrom(src => src.Currency.Id))
                 .ForPath(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.Currency.Code));

            CreateMap<GoogleAutnInputModel, LeadSearchModel>()
                .ForPath(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                 .ForPath(dest => dest.FirstName, opt => opt.MapFrom(src => src.UserName));

        }
    }
}
