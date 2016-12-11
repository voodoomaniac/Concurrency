using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Concurrency.Core.Models;
using Concurrency.DAL.Repositories;

namespace Concurrency.Web.APIControllers
{
    public class BooksController : ApiController
    {
        private readonly BooksRepository _booksRepository;
        // replace with real user
        private const string userName = "user1";

        public BooksController()
        {
            _booksRepository = new BooksRepository(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }

        // GET: api/Books
        public IEnumerable<Book> Get()
        {
            var books = _booksRepository.GetAll();

            return books;
        }

        // GET: api/Books/5
        public HttpResponseMessage Get(int id, bool isForEdit)
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

        // POST: api/Books
        public void Post([FromBody]Book book)
        {
            _booksRepository.Add(book);
        }

        // PUT: api/Books
        public HttpResponseMessage Put([FromBody]Book book)
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

        // DELETE: api/Books/5
        public void Delete(int id)
        {
            _booksRepository.Delete(id);
        }
    }
}
