using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AzureServices.Models;

namespace AzureServices.Controllers
{
    public class TaskController : ApiController
    {
        private AntaresDBEntities3 db = new AntaresDBEntities3();

        // GET api/Task
        public IEnumerable<Task> GetTasks()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var tasks = db.Tasks.Include(t => t.Category1).Include(t => t.Project).Include(t => t.RepeatType1).Include(t => t.User);
            return tasks.AsEnumerable();
        }

        //// GET api/Task/5
        //public Task GetTask(int id)
        //{
        //    db.Configuration.ProxyCreationEnabled = false;
        //    Task task = db.Tasks.Find(id);
        //    if (task == null)
        //    {
        //        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
        //    }

        //    return task;
        //}

        // GET api/Project
        public IEnumerable<Task> GetTasks(string id)
        {
            if (id.Contains("pid$$"))
            {
                var projectid = Convert.ToInt32(id.Split(new[] { "pid$$" }, StringSplitOptions.RemoveEmptyEntries)[0]);

                db.Configuration.ProxyCreationEnabled = false;
                var tasks = from task in db.Tasks
                            where task.ProjectID == projectid
                            select task;
                db.Configuration.ProxyCreationEnabled = false;
                return tasks.ToList();
            }
            if (id.Contains("uid$$"))
            {
                var userid = Convert.ToInt32(id.Split(new[] { "uid$$" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                db.Configuration.ProxyCreationEnabled = false;
                var tasks = from task in db.Tasks
                            where task.UserID == userid
                            select task;
                db.Configuration.ProxyCreationEnabled = false;
                return tasks.ToList();
            }

            return null;
            //if(string.IsNullOrEmpty(id))
            //{
            //    return null;
            //}

            //var i = Convert.ToInt32(id.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries)[1]);

            //if(id.Contains("pid="))
            //{
            //    db.Configuration.ProxyCreationEnabled = false;
            //    var tasks = from task in db.Tasks
            //                where task.ProjectID == i
            //                select task;
            //    db.Configuration.ProxyCreationEnabled = false;
            //    return tasks.ToList();
            //}

            //if(id.Contains("uid="))
            //{

            //}
        }

        // PUT api/Task/5
        public HttpResponseMessage PutTask(int id, Task task)
        {
            if (ModelState.IsValid && id == task.ID)
            {
                db.Entry(task).State = EntityState.Modified;

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
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // POST api/Task
        public HttpResponseMessage PostTask(Task task)
        {
            if (ModelState.IsValid)
            {
                if (!db.Tasks.Any())
                {
                    task.ID = 0;
                }
                else
                {
                    task.ID = db.Tasks.Max(record => record.ID) + 1;
                }

                db.Tasks.Add(task);
                db.SaveChanges();
                db.Configuration.ProxyCreationEnabled = false;

                var response = Request.CreateResponse(HttpStatusCode.Created, task);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = task.ID }));
                return response;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        // DELETE api/Task/5
        public HttpResponseMessage DeleteTask(int id)
        {
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Tasks.Remove(task);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, task);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}