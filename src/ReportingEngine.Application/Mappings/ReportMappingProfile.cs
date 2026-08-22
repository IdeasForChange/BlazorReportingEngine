using AutoMapper;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.Application.Mappings;

public class ReportMappingProfile : Profile
{
    public ReportMappingProfile()
    {
        CreateMap<ReportMaster, ReportMasterDto>().ReverseMap();

        CreateMap<ReportTemplate, ReportTemplateDto>().ReverseMap();
        CreateMap<ReportMetric, ReportMetricDto>().ReverseMap();
        CreateMap<ReportParameter, ReportParameterDto>().ReverseMap();
    }
}
