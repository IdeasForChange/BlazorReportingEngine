using AutoMapper;
using Smbc.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Application.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.Application.Mappings;

public class SystemParameterTypeMappingProfile : Profile
{
    public SystemParameterTypeMappingProfile()
    {
        CreateMap<SystemParameterType, SystemParameterTypeDto>();
        //CreateMap<CreateSystemParameterTypeDto, SystemParameterType>();
        //CreateMap<UpdateSystemParameterTypeDto, SystemParameterType>();
    }
}
