using System;
using System.Data;
using Concurrency.Core.Models;
using Concurrency.DAL.Interfaces;

namespace Concurrency.DAL.Mappers
{
    class AuthorMapper : IMapper<Author>
    {
        public Author MapFromDataRecord(IDataRecord record, Author item)
        {
            item.Id = Convert.ToInt32(record["Id"]);
            item.FirstName = Convert.ToString(record["FirstName"]);
            item.LastName = Convert.ToString(record["LastName"]);

            return item;
        }
    }
}