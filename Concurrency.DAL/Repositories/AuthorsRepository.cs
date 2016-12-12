using System.Collections.Generic;
using System.Data;
using Concurrency.Core.Models;
using Concurrency.DAL.Extensions;
using Concurrency.DAL.Mappers;
using Concurrency.DAL.UnitOfWork;

namespace Concurrency.DAL.Repositories
{
    //Implements optimistic concurrency
    public class AuthorsRepository : BaseRepository<Author>
    {
        private const string AuthorsTableName = "Authors";

        public AuthorsRepository(AdoNetUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IEnumerable<Author> GetAll()
        {
            using (var command = unitOfWork.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {AuthorsTableName}";
                return this.ToList(command, new AuthorMapper());
            }
        }

        public Author Get(int id)
        {
            Author author = null;

            using (var command = unitOfWork.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {AuthorsTableName} WHERE Id = @Id";
                command.AddWithValue("@Id", id);

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        author = new AuthorMapper().MapFromDataRecord(reader, new Author());
                    }
                }

                return author;
            }
        }

        public void Add(Author item)
        {
            using (var command = unitOfWork.CreateCommand())
            {
                command.CommandText = $"INSERT INTO {AuthorsTableName} (firstName, lastName) VALUES (@FirstName, @LastName)";
                command.AddWithValue("@FirstName", item.FirstName);
                command.AddWithValue("@LastName", item.LastName);

                command.ExecuteNonQuery();
                unitOfWork.SaveChanges();
            }
        }

        public bool Delete(Author item)
        {
            using (var command = unitOfWork.CreateCommand())
            {
                command.CommandText = $"DELETE FROM {AuthorsTableName} WHERE Id = @Id " +
                                       "AND [FirstName] = @FirstName " +
                                       "AND [LastName] = @LastName";
                command.AddWithValue("@Id", item.Id);
                command.AddWithValue("@FirstName", item.FirstName);
                command.AddWithValue("@LastName", item.LastName);

                int rowsAffected = command.ExecuteNonQuery();
                unitOfWork.SaveChanges();

                return rowsAffected != 0;
            }
        }

        public bool Update(Author originalItem, Author updatedItem)
        {
            using (var command = unitOfWork.CreateCommand())
            {
                command.CommandText =
                    $"UPDATE {AuthorsTableName} SET [FirstName] = @FirstName, [LastName] = @LastName  " +
                    "WHERE [Id] = @Id AND [FirstName] = @original_FirstName " +
                    "AND [LastName] = @original_LastName";
                command.AddWithValue("@Id", originalItem.Id);
                command.AddWithValue("@FirstName", updatedItem.FirstName);
                command.AddWithValue("@LastName", updatedItem.LastName);
                command.AddWithValue("@original_FirstName", originalItem.FirstName);
                command.AddWithValue("@original_LastName", originalItem.LastName);
                
                int rowsAffected = command.ExecuteNonQuery();
                unitOfWork.SaveChanges();

                return rowsAffected != 0;
            }
        }
    }
}