using System;
using System.Data;

namespace Concurrency.Core.Models
{
    public class Book : BaseEntity
    {
        public string Name { get; set; }

        public DateTime PublishDate { get; set; }

        public bool IsLocked { get; set; }

        public string LockedBy { get; set; }

        public static Book CreateFromDataRecord(IDataRecord record)
        {
            return new Book
            {
                Id = Convert.ToInt32(record["Id"]),
                Name = Convert.ToString(record["Name"]),
                PublishDate = Convert.ToDateTime(record["PublishDate"]),
                IsLocked = record["IsLocked"] == DBNull.Value ? false : Convert.ToBoolean(record["IsLocked"]),
                LockedBy = record["LockedBy"] == DBNull.Value ? string.Empty : Convert.ToString(record["LockedBy"])
            };
        }
    }
}