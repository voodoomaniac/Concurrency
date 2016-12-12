using System;
using System.Data;
using Concurrency.Core.Models;
using Concurrency.DAL.Interfaces;

namespace Concurrency.DAL.Mappers
{
    class BookMapper : IMapper<Book>
    {
        public Book MapFromDataRecord(IDataRecord record, Book item)
        {
            item.Id = Convert.ToInt32(record["Id"]);
            item.Name = Convert.ToString(record["Name"]);
            item.PublishDate = Convert.ToDateTime(record["PublishDate"]);
            item.IsLocked = record["IsLocked"] == DBNull.Value ? false : Convert.ToBoolean(record["IsLocked"]);
            item.LockedBy = record["LockedBy"] == DBNull.Value ? string.Empty : Convert.ToString(record["LockedBy"]);

            return item;
        }
    }
}
