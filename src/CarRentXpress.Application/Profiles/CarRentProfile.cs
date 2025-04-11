using AutoMapper;
using CarRentXpress.Application.Entities;
using CarRentXpress.Data.Entities;

namespace CarRentXpress.Profiles;

public class CarRentProfile : Profile
{
    public CarRentProfile()
    {
        CreateMap<CarRentDto, CarRent>();
        CreateMap<CarRent, CarRentDto>();
    }
}