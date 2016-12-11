using System;
using System.Data;

namespace Concurrency.Core.Models
{
    public class Author : BaseEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public static Author CreateFromDataRecord(IDataRecord record)
        {
            return new Author
            {
                Id = Convert.ToInt32(record["Id"]),
                FirstName = Convert.ToString(record["FirstName"]),
                LastName = Convert.ToString(record["LastName"])
            };
        }
    }
}