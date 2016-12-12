﻿using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Concurrency.Core.Models;
using Concurrency.DAL.Repositories;
using Concurrency.DAL.UnitOfWork;

namespace Concurrency.Web.APIControllers
{
    public class AuthorsController : ApiController
    {
        private readonly AuthorsRepository _authorsRepository;

        public AuthorsController()
        {
            _authorsRepository = new AuthorsRepository(UnitOfWorkFactory.Create());
        }

        // GET: api/Authors
        public IEnumerable<Author> Get()
        {
            using (_authorsRepository)
            {
                var authors = _authorsRepository.GetAll();
                return authors;
            }
        }

        // GET: api/Authors/5
        public Models.Author Get(int id)
        {
            using (_authorsRepository)
            {
                Models.Author author = new Models.Author();
                author.ToWebModel(_authorsRepository.Get(id));
                return author;
            }
        }

        // POST: api/Authors
        public void Post([FromBody] Author author)
        {
            using (_authorsRepository)
            {
                _authorsRepository.Add(author);
            }
        }

        // PUT: api/Authors
        public HttpResponseMessage Put([FromBody] Models.Author author)
        {
            var originalAuthor = new Author()
            {
                Id = author.Id,
                FirstName = author.OriginalFirstName,
                LastName = author.OriginalLastName
            };

            var updatedAuthor = new Author()
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName
            };

            using (_authorsRepository)
            {
                bool isSuccess = _authorsRepository.Update(originalAuthor, updatedAuthor);

                if (!isSuccess)
                {
                    var actualAuthor = _authorsRepository.Get(author.Id);

                    var response = Request.CreateResponse(HttpStatusCode.Conflict, actualAuthor, "application/json");
                    response.ReasonPhrase = "Concurrency occured";
                    return response;
                }

                return Request.CreateResponse(204);
            }
        }

        // DELETE: api/Authors
        public HttpResponseMessage Delete([FromBody]Author author)
        {
            using (_authorsRepository)
            {
                bool isSuccess = _authorsRepository.Delete(author);

                if (!isSuccess)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, "Concurrency occured", "application/json");
                }

                return Request.CreateResponse(204);
            }
        }
    }
}