using AutoMapper;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.Application.Mappings;

public class DatabaseMappingProfile :  Profile
{
    public DatabaseMappingProfile()
    {
        CreateMap<DatabaseConnection, DatabaseConnectionDto>().ReverseMap();
    }
}
