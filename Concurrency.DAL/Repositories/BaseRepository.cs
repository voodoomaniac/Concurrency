using System;
using System.Collections.Generic;
using System.Data;
using Concurrency.Core.Models;
using Concurrency.DAL.Interfaces;
using Concurrency.DAL.UnitOfWork;

namespace Concurrency.DAL.Repositories
{
    public class BaseRepository<TEntity>: IDisposable where TEntity : BaseEntity, new() 
    {
        protected readonly AdoNetUnitOfWork unitOfWork;

        public BaseRepository(AdoNetUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException();
            }

            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<TEntity> ToList(IDbCommand command, IMapper<TEntity> mapper)
        {
            using (var reader = command.ExecuteReader())
            {
                var items = new List<TEntity>();
                while (reader.Read())
                {
                    var item = new TEntity();
                    mapper.MapFromDataRecord(reader, item);
                    items.Add(item);
                }
                return items;
            }
        }

        public void Dispose()
        {
            unitOfWork.Dispose();
        }
    }
}