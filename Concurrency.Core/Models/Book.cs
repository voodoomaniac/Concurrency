using System;

namespace Concurrency.Core.Models
{
    public class Book : BaseEntity
    {
        public string Name { get; set; }

        public DateTime PublishDate { get; set; }

        public bool IsLocked { get; set; }

        public string LockedBy { get; set; }
    }
}