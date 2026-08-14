using Smbc.ReportingEngine.Domain.Entities;
using Smbc.Risk.Core.Domain.Shared.Repositories;

namespace Smbc.Risk.ReportingEngine.Domain.Repositories;

public interface ISystemParameterTypeRepository : IBaseRepository<SystemParameterType>
{
    Task<bool> ExistsByCode(string code, long? excludeId = null, CancellationToken cancellationToken = default);
}

