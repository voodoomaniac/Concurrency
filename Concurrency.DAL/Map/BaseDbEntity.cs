using System.Collections.Generic;
using System.Data;
using Concurrency.Core.Models;

namespace Concurrency.DAL.Map
{
    internal abstract class DbEntity<TEntity> where TEntity: BaseEntity, new()
    {
        public abstract TEntity MapFromDataRecord(IDataRecord record, TEntity item);

        public IEnumerable<TEntity> ToList(IDbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                var items = new List<TEntity>();
                while (reader.Read())
                {
                    var item = new TEntity();
                    MapFromDataRecord(reader, item);
                    items.Add(item);
                }
                return items;
            }
        }
    }
}