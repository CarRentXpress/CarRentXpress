using AutoMapper;
using CarRentXpress.Data.Entities;
using CarRentXpress.DTOs;

namespace CarRentXpress.Profiles;

public class CarProfile : Profile
{
    public CarProfile()
    {
        CreateMap<CarDto, Car>();
        CreateMap<Car, CarDto>();
    }
}