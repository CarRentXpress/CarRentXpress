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
        
        CreateMap<IEnumerable<CarRent>, IEnumerable<CarRentDto>>()
            .ConvertUsing((src, dest, context) => src.Select(x => context.Mapper.Map<CarRentDto>(x)).ToList());

        CreateMap<IEnumerable<CarRentDto>, IEnumerable<CarRent>>()
            .ConvertUsing((src, dest, context) => src.Select(x => context.Mapper.Map<CarRent>(x)).ToList());
    }
}