using System.Data;
using Concurrency.Core.Models;

namespace Concurrency.DAL.Interfaces
{
    public interface IMapper<TEntity> where TEntity : BaseEntity
    {
        TEntity MapFromDataRecord(IDataRecord record, TEntity item);
    }
}