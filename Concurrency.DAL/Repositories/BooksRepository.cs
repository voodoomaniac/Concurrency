using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Concurrency.Core.Models;
using Concurrency.DAL.Extensions;
using Concurrency.DAL.Mappers;
using Concurrency.DAL.UnitOfWork;

namespace Concurrency.DAL.Repositories
{
    //Implements pessimistic concurrency
    public class BooksRepository : BaseRepository<Book>
    {
        private const string BooksTableName = "Books";

        public BooksRepository(AdoNetUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IEnumerable<Book> GetAll()
        {
            using (var command = unitOfWork.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {BooksTableName}";
                return this.ToList(command, new BookMapper());
            }
        }

        public Book Get(int id, bool isForEdit = false, string userName = "")
        {
            Book book = null;

            using (var command = unitOfWork.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {BooksTableName} WHERE [Id] = @Id ";
                command.AddWithValue("@Id", id);

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        book = new BookMapper().MapFromDataRecord(reader, new Book());
                    }
                }
            }

            if (book != null && isForEdit)
            {
                // if book is locked by current user = return book instance for edit
                if (book.IsLocked && book.LockedBy == userName)
                {
                    return book;
                }

                // if book is not locked set the lock for the book by user that requested it
                if (book.IsLocked == false)
                {
                    using (var command = unitOfWork.CreateCommand())
                    {
                        command.CommandText =
                            $"UPDATE {BooksTableName} SET [IsLocked] = @IsLocked, [LockedBy] = @LockedBy  " +
                             "WHERE [Id] = @Id";
                        command.AddWithValue("@Id", id);
                        command.AddWithValue("@IsLocked", true);
                        command.AddWithValue("@LockedBy", userName);

                        command.ExecuteNonQuery();
                        unitOfWork.SaveChanges();
                    }

                    return book;
                }

                // if book is locked by other user return null
                return null;
            }

            return book;

        }

        public void Add(Book item)
        {

            using (var command = unitOfWork.CreateCommand())
            {
                command.CommandText = $"INSERT INTO {BooksTableName} (Name, PublishDate) VALUES (@Name, @PublishDate) ";
                command.AddWithValue("@Name", item.Name);
                command.AddWithValue("@PublishDate", item.PublishDate);

                command.ExecuteNonQuery();
                unitOfWork.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (var command = unitOfWork.CreateCommand())
            {
                command.CommandText = $"DELETE FROM {BooksTableName} WHERE Id = @Id ";
                command.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
                unitOfWork.SaveChanges();
            }
        }

        public bool Update(Book item, string userName = "")
        {

            using (var command = unitOfWork.CreateCommand())
            {
                command.CommandText =
                    $"UPDATE {BooksTableName} SET [Name] = @Name, [PublishDate] = @PublishDate, [IsLocked] = null, [LockedBy] = null " +
                     "WHERE [Id] = @Id AND [IsLocked] = @IsLocked AND [LockedBy] = @LockedBy ";
                command.AddWithValue("@Id", item.Id);
                command.AddWithValue("@Name", item.Name);
                command.AddWithValue("@PublishDate", item.PublishDate);
                command.AddWithValue("@IsLocked", true);
                command.AddWithValue("@LockedBy", userName);

                int rowsAffected = command.ExecuteNonQuery();
                unitOfWork.SaveChanges();

                return rowsAffected != 0;
            }
        }
    }
}
