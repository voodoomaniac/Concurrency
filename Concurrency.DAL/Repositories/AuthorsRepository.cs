using System.Collections.Generic;
using System.Data.SqlClient;
using Concurrency.Core.Models;

namespace Concurrency.DAL.Repositories
{
    //Implements optimistic concurrency
    public class AuthorsRepository
    {
        private const string AuthorsTableName = "Authors";
        private readonly string _connectionString;

        public AuthorsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Author> GetAll()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand($"SELECT * FROM {AuthorsTableName}", connection))
                {
                    var list = new List<Author>();

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(Author.CreateFromDataRecord(reader));
                        }
                    }

                    return list;
                }
            }
        }

        public Author Get(int id)
        {
            Author author = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand($"SELECT * FROM {AuthorsTableName} WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            author = Author.CreateFromDataRecord(reader);
                        }
                    }

                    return author;
                }
            }
        }

        public void Add(Author item)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand($"INSERT INTO {AuthorsTableName} (firstName, lastName) VALUES (@FirstName, @LastName)", connection))
                {
                    command.Parameters.AddWithValue("@FirstName", item.FirstName);
                    command.Parameters.AddWithValue("@LastName", item.LastName);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool Delete(Author item)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand($"DELETE FROM {AuthorsTableName} WHERE Id = @Id " +
                            "AND [FirstName] = @FirstName " +
                            "AND [LastName] = @LastName", connection))
                {
                    command.Parameters.AddWithValue("@Id", item.Id);
                    command.Parameters.AddWithValue("@FirstName", item.FirstName);
                    command.Parameters.AddWithValue("@LastName", item.LastName);

                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected != 0;
                }
            }
        }

        public bool Update(Author originalItem, Author updatedItem)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand($"UPDATE {AuthorsTableName} SET [FirstName] = @FirstName, [LastName] = @LastName  " +
                            "WHERE [Id] = @Id AND [FirstName] = @original_FirstName " +
                            "AND [LastName] = @original_LastName", connection))
                {
                    command.Parameters.AddWithValue("@Id", originalItem.Id);
                    command.Parameters.AddWithValue("@FirstName", updatedItem.FirstName);
                    command.Parameters.AddWithValue("@LastName", updatedItem.LastName);
                    command.Parameters.AddWithValue("@original_FirstName", originalItem.FirstName);
                    command.Parameters.AddWithValue("@original_LastName", originalItem.LastName);

                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected != 0;
                }
            }
        }
    }
}