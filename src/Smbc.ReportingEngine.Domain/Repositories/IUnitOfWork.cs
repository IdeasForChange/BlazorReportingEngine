using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smbc.ReportingEngine.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<EnvironmentConfig> EnvironmentConfigs { get; }
    Task<int> CompleteAsync();
}