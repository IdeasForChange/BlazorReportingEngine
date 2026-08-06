using Smbc.ReportingEngine.Domain.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smbc.ReportingEngine.Domain.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(long id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
}
