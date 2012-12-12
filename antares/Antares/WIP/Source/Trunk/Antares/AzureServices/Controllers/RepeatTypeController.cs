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
    public class RepeatTypeController : ApiController
    {
        private AntaresDBEntities3 db = new AntaresDBEntities3();

        // GET api/RepeatType
        public IEnumerable<RepeatType> GetRepeatTypes()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.RepeatTypes.AsEnumerable();
        }

        // GET api/RepeatType/5
        public RepeatType GetRepeatType(int id)
        {
            RepeatType repeattype = db.RepeatTypes.Find(id);
            if (repeattype == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return repeattype;
        }

        // PUT api/RepeatType/5
        public HttpResponseMessage PutRepeatType(int id, RepeatType repeattype)
        {
            if (ModelState.IsValid && id == repeattype.ID)
            {
                db.Entry(repeattype).State = EntityState.Modified;

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

        // POST api/RepeatType
        public HttpResponseMessage PostRepeatType(RepeatType repeattype)
        {
            if (ModelState.IsValid)
            {
                if (!db.RepeatTypes.Any())
                {
                    repeattype.ID = 0;
                }
                else
                {
                    repeattype.ID = db.RepeatTypes.Max(record => record.ID) + 1;
                }
                db.RepeatTypes.Add(repeattype);
                db.SaveChanges();
                db.Configuration.ProxyCreationEnabled = false;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, repeattype);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = repeattype.ID }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/RepeatType/5
        public HttpResponseMessage DeleteRepeatType(int id)
        {
            RepeatType repeattype = db.RepeatTypes.Find(id);
            if (repeattype == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.RepeatTypes.Remove(repeattype);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, repeattype);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}