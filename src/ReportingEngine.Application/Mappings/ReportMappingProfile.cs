using AutoMapper;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.Application.Mappings;

public class ReportMappingProfile : Profile
{
    public ReportMappingProfile()
    {
        CreateMap<ReportMaster, ReportMasterDto>().ReverseMap();
        CreateMap<ReportParameter, ReportParameterDto>().ReverseMap();
        CreateMap<ReportMetric, ReportMetricDto>().ReverseMap();
        CreateMap<ReportTemplate, ReportTemplateDto>()
            .ForMember(x => x.FileName, opt => opt.Ignore())
            .ForMember(x => x.FileBytes, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<ReportRunnerQueue, ReportRunnerQueueDto>()
            .ForMember(dest => dest.ReportName, opt => opt.MapFrom(src => src.ReportMaster.Name));

        // DTO / Requests to Entity
        //CreateMap<CreateReportParameterDto, ReportParameter>();
        //CreateMap<EnqueueReportRequestDto, ReportRunnerQueue>()
        //    .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => 1)) // QueueStatus.Pending
        //    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));
    }
}
