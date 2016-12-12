using System.Collections.Generic;
using System.Data.SqlClient;
using Concurrency.Core.Models;
using Concurrency.DAL.Map;

namespace Concurrency.DAL.Repositories
{
    //Implements pessimistic concurrency
    public class BooksRepository
    {
        private const string BooksTableName = "Books";
        private readonly string _connectionString;

        public BooksRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Book> GetAll()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand($"SELECT * FROM {BooksTableName}", connection))
                {
                    var list = new List<Book>();

                    connection.Open();

                    return new DbBook().ToList(command);
                }
            }
        }

        public Book Get(int id, bool isForEdit = false, string userName = "")
        {
            Book book = null;
            var selectSqlCommandforRead = $"SELECT * FROM {BooksTableName} WHERE [Id] = @Id";


            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();


                using (var command = new SqlCommand(selectSqlCommandforRead, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            book = new DbBook().MapFromDataRecord(reader, new Book());
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
                        using (var command = new SqlCommand($"UPDATE {BooksTableName} SET [IsLocked] = @IsLocked, [LockedBy] = @LockedBy  " +
                           "WHERE [Id] = @Id", connection))
                        {
                            command.Parameters.AddWithValue("@Id", id);
                            command.Parameters.AddWithValue("@IsLocked", true);
                            command.Parameters.AddWithValue("@LockedBy", userName);

                            command.ExecuteNonQuery();
                        }

                        return book;
                    }

                    // if book is locked by other user return null
                    return null;
                }

                return book;
            }
        }

        public void Add(Book item)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand($"INSERT INTO {BooksTableName} (Name, PublishDate) VALUES (@Name, @PublishDate)", connection))
                {
                    command.Parameters.AddWithValue("@Name", item.Name);
                    command.Parameters.AddWithValue("@PublishDate", item.PublishDate);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand($"DELETE FROM {BooksTableName} WHERE Id = @Id ", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool Update(Book item, string userName = "")
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand($"UPDATE {BooksTableName} SET [Name] = @Name, [PublishDate] = @PublishDate, [IsLocked] = null, [LockedBy] = null" +
                                                    " WHERE [Id] = @Id AND [IsLocked] = @IsLocked AND [LockedBy] = @LockedBy", connection))
                {
                    command.Parameters.AddWithValue("@Id", item.Id);
                    command.Parameters.AddWithValue("@Name", item.Name);
                    command.Parameters.AddWithValue("@PublishDate", item.PublishDate);
                    command.Parameters.AddWithValue("@IsLocked", true);
                    command.Parameters.AddWithValue("@LockedBy", userName);

                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected != 0;
                }
            }
        }
    }
}
