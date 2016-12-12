using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Concurrency.Core.Models;
using Concurrency.DAL.Repositories;
using Concurrency.DAL.UnitOfWork;

namespace Concurrency.Web.APIControllers
{
    public class BooksController : ApiController
    {
        private readonly BooksRepository _booksRepository;
        // replace with real user
        private const string userName = "user1";

        public BooksController()
        {
            _booksRepository = new BooksRepository(UnitOfWorkFactory.Create());
        }

        // GET: api/Books
        public IEnumerable<Book> Get()
        {
            using (_booksRepository)
            {
                var books = _booksRepository.GetAll();
                return books;
            }
        }

        // GET: api/Books/5
        public HttpResponseMessage Get(int id, bool isForEdit)
        {
            using (_booksRepository)
            {
                Book book = null;

                if (isForEdit)
                {
                    book = _booksRepository.Get(id, true, userName);

                    if (book == null)
                    {
                        var response = Request.CreateResponse(HttpStatusCode.Conflict, book, "application/json");
                        response.ReasonPhrase = "locked for edit";
                        return response;
                    }
                }

                book = _booksRepository.Get(id);
                return Request.CreateResponse(HttpStatusCode.OK, book, "application/json");
            }
        }

        // POST: api/Books
        public void Post([FromBody]Book book)
        {
            using (_booksRepository)
            {
                _booksRepository.Add(book);
            }
        }

        // PUT: api/Books
        public HttpResponseMessage Put([FromBody]Book book)
        {
            using (_booksRepository)
            {
                bool isSuccess = _booksRepository.Update(book, userName);

                if (!isSuccess)
                {
                    var response = Request.CreateResponse(HttpStatusCode.Conflict);
                    response.ReasonPhrase = "No records were updated";
                    return response;
                }

                return Request.CreateResponse(204);
            }
        }

        // DELETE: api/Books/5
        public void Delete(int id)
        {
            using (_booksRepository)
            {
                _booksRepository.Delete(id);
            }
        }
    }
}
