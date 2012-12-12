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
    public class PriorityController : ApiController
    {
        private AntaresDBEntities3 db = new AntaresDBEntities3();

        // GET api/Priority
        public IEnumerable<Priority> GetPriorities()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.Priorities.AsEnumerable();
        }

        // GET api/Priority/5
        public Priority GetPriority(int id)
        {
            Priority priority = db.Priorities.Find(id);
            if (priority == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return priority;
        }

        // PUT api/Priority/5
        public HttpResponseMessage PutPriority(int id, Priority priority)
        {
            if (ModelState.IsValid && id == priority.ID)
            {
                db.Entry(priority).State = EntityState.Modified;

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

        // POST api/Priority
        public HttpResponseMessage PostPriority(Priority priority)
        {
            if (ModelState.IsValid)
            {
                if (!db.Priorities.Any())
                {
                    priority.ID = 0;
                }
                else
                {
                    priority.ID = db.Priorities.Max(record => record.ID) + 1;
                }
                db.Priorities.Add(priority);
                db.SaveChanges();
                db.Configuration.ProxyCreationEnabled = false;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, priority);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = priority.ID }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/Priority/5
        public HttpResponseMessage DeletePriority(int id)
        {
            Priority priority = db.Priorities.Find(id);
            if (priority == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Priorities.Remove(priority);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, priority);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}