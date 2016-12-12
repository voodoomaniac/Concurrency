using System;
using System.Data;
using Concurrency.Core.Models;

namespace Concurrency.DAL.Map
{
    class DbAuthor : DbEntity<Author>
    {
        public override Author MapFromDataRecord(IDataRecord record, Author item)
        {
            item.Id = Convert.ToInt32(record["Id"]);
            item.FirstName = Convert.ToString(record["FirstName"]);
            item.LastName = Convert.ToString(record["LastName"]);

            return item;
        }
    }
}