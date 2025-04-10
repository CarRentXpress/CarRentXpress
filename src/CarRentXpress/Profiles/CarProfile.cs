using AutoMapper;
using CarRentXpress.Application.Entities;
using CarRentXpress.Data.Entities;
using CarRentXpress.DTOs;

namespace CarRentXpress.Profiles;

public class CarProfile : Profile
{
    public CarProfile()
    {
        CreateMap<CarDto, Car>();
        CreateMap<Car, CarDto>();
        
        CreateMap<CarRentDto, CarRent>();
        CreateMap<CarRent, CarRentDto>();
        
        CreateMap<Car, CarDto>().ReverseMap();
    }
}