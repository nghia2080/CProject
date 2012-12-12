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
    public class ProjectController : ApiController
    {
        private AntaresDBEntities3 db = new AntaresDBEntities3();

        // GET api/Project
        public IEnumerable<Project> GetProjects()
        {
            db.Configuration.ProxyCreationEnabled = false;  
            return db.Projects.AsEnumerable();
        }

        // GET api/Project
        public IEnumerable<Project> GetProjects(string username)
        {
            db.Configuration.ProxyCreationEnabled = false;  
            var u = from user in db.Users
                        where user.Username == username
                        select user;

            if(!u.Any())
            {
                return null;
            }
            var userID = !u.Any() ? -1 : u.ToList()[0].UserID;

            var projectIDs = from project in db.ProjectMembers
                        where project.UserID == userID
                        select project.ProjectID;

            var userProject = new List<Project>();

            foreach (var id in projectIDs)
            {
                userProject.Add(db.Projects.Find(id));
            }
            return userProject;
        }

        // GET api/Project
        public IEnumerable<Project> GetProjects(int userid)
        {
            db.Configuration.ProxyCreationEnabled = false; 
            var projectIDs = from project in db.ProjectMembers
                             where project.UserID == userid
                             select project.ProjectID;

            var userProject = new List<Project>();

            foreach (var id in projectIDs)
            {
                userProject.Add(db.Projects.Find(id));
            }

            return userProject;
        }

        // GET api/Project/5
        public Project GetProject(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;  
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return project;
        }


        // PUT api/Project/5
        public HttpResponseMessage PutProject(int id, Project project)
        {
            if (ModelState.IsValid && id == project.ID)
            {
                db.Entry(project).State = EntityState.Modified;

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

        // POST api/Project
        public HttpResponseMessage PostProject(Project project)
        {
            if (ModelState.IsValid)
            {
                if (!db.Projects.Any())
                {
                    project.ID = 0;
                }
                else
                {
                    project.ID = db.Projects.Max(record => record.ID) + 1;
                }
                db.Configuration.ProxyCreationEnabled = false;
                db.Projects.Add(project);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, project);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = project.ID }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/Project/5
        public HttpResponseMessage DeleteProject(int id)
        {
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Projects.Remove(project);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, project);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}