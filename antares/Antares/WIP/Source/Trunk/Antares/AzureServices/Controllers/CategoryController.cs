using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AzureServices.Models;

namespace AzureServices.Controllers
{
    public class CategoryController : ApiController
    {
        private AntaresDBEntities3 db = new AntaresDBEntities3();

        // GET api/Category
        public IEnumerable<Category> GetCategories()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.Categories.AsEnumerable();
        }

        // GET api/Category/5
        public Category GetCategory(int id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return category;
        }

        // PUT api/Category/5
        public HttpResponseMessage PutCategory(int id, Category category)
        {
            if (ModelState.IsValid && id == category.ID)
            {
                db.Entry(category).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // POST api/Category
        public HttpResponseMessage PostCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                if (!db.Categories.Any())
                {
                    category.ID = 0;
                }
                else
                {
                    category.ID = db.Categories.Max(record => record.ID) + 1;
                }

                db.Categories.Add(category);
                db.SaveChanges();
                db.Configuration.ProxyCreationEnabled = false;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, category);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = category.ID }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/Category/5
        public HttpResponseMessage DeleteCategory(int id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Categories.Remove(category);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, category);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}