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
    public class UserController : ApiController
    {
        private AntaresDBEntities3 db = new AntaresDBEntities3();

        // GET api/User
        public IEnumerable<User> GetUsers()
        {
            db.Configuration.ProxyCreationEnabled = false;  
            return db.Users.AsEnumerable();
        }

        // GET api/User/5
        public User GetUser(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;  
            var user = db.Users.Find(id);
            if (user == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return user;
        }

        // GET api/User?username=hunglh01011@fpt.edu.vn
        public User GetUser(string username)
        {
            db.Configuration.ProxyCreationEnabled = false;  
            var query = from user in db.Users
                        where user.Username == username
                        select user;

            return !query.Any() ? null : query.ToList()[0];
        }

        // PUT api/User/5
        public HttpResponseMessage PutUser(int id, User user)
        {
            if (ModelState.IsValid && id == user.UserID)
            {
                db.Entry(user).State = EntityState.Modified;

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

        // POST api/User
        public HttpResponseMessage PostUser(User user)
        {
            if (ModelState.IsValid)
            {
                if (!db.Users.Any())
                {
                    user.UserID = 0;
                }
                else
                {
                    user.UserID = db.Users.Max(record => record.UserID) + 1;
                }

                db.Users.Add(user);
                db.SaveChanges();
                db.Configuration.ProxyCreationEnabled = false;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, user);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = user.UserID }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/User/5
        public HttpResponseMessage DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Users.Remove(user);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}