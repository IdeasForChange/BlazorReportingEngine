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
            .ForMember(dest => dest.FileName, opt => opt.Ignore())
            .ForMember(dest => dest.FileBytes, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<ReportRunnerQueue, ReportRunnerQueueDto>()
            .ForMember(dest => dest.ReportName, opt => opt.MapFrom(src => src.ReportMaster.Name));

        // DTO / Requests to Entity
        CreateMap<CreateReportParameterDto, ReportParameter>()
            .ForMember(dest => dest.ReportMaster, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EntityVersion, opt => opt.Ignore())
            .ForMember(dest => dest.EntityWrittenAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore());

        CreateMap<EnqueueReportRequestDto, ReportRunnerQueue>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => 1)) // QueueStatus.Pending
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.ProgressPercentage, opt => opt.Ignore())
            .ForMember(dest => dest.OutputFilePath, opt => opt.Ignore())
            .ForMember(dest => dest.ErrorMessage, opt => opt.Ignore())
            .ForMember(dest => dest.StartedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.ReportMaster, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EntityVersion, opt => opt.Ignore())
            .ForMember(dest => dest.EntityWrittenAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore()); ;
    }
}
