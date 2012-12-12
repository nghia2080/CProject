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
    public class ProjectMemberController : ApiController
    {
        private AntaresDBEntities3 db = new AntaresDBEntities3();

        // GET api/ProjectMember
        public IEnumerable<ProjectMember> GetProjectMembers()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var projectmembers = db.ProjectMembers.Include(p => p.Project).Include(p => p.User);
            return projectmembers.AsEnumerable();
        }

        // GET api/ProjectMember/5
        public IEnumerable<ProjectMember> GetProjectMembers(string id)
        {
            if (id.Contains("pid$$"))
            {
                var projectid = Convert.ToInt32(id.Split(new[] { "pid$$" }, StringSplitOptions.RemoveEmptyEntries)[0]);

                db.Configuration.ProxyCreationEnabled = false;
                var query = from projectmember in db.ProjectMembers
                            where projectmember.ProjectID == projectid
                            select projectmember;

                if (!query.Any())
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }

                return query.ToList();
            }
            else if (id.Contains("uid$$"))
            {
                var userid = Convert.ToInt32(id.Split(new[] { "uid$$" }, StringSplitOptions.RemoveEmptyEntries)[0]);

                db.Configuration.ProxyCreationEnabled = false;
                var query = from projectmember in db.ProjectMembers
                            where projectmember.UserID == userid
                            select projectmember;

                if (!query.Any())
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }

                return query.ToList();
            }

            return null;
        }

        // PUT api/ProjectMember/5
        public HttpResponseMessage PutProjectMember(int id, ProjectMember projectmember)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projectmember).State = EntityState.Modified;

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

        // POST api/ProjectMember
        public HttpResponseMessage PostProjectMember(ProjectMember projectmember)
        {
            if (ModelState.IsValid)
            {
                db.Configuration.ProxyCreationEnabled = false;
                var query = from pm in db.ProjectMembers
                            where pm.ProjectID == projectmember.ProjectID
                            select pm;

                if (query.ToList().FirstOrDefault(p => p.UserID == projectmember.UserID) != null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                db.ProjectMembers.Add(projectmember);
                db.SaveChanges();
                db.Configuration.ProxyCreationEnabled = false;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, projectmember);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = projectmember.ProjectID }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/ProjectMember/5
        public HttpResponseMessage DeleteProjectMember(int projectid, int userid)
        {
            var projectmember = db.ProjectMembers.FirstOrDefault(p => (p.ProjectID == projectid && p.UserID == userid));

            if (projectmember == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.ProjectMembers.Remove(projectmember);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, projectmember);
        }

        // DELETE api/ProjectMember/5
        public HttpResponseMessage DeleteProjectMember(int id)
        {       
            var projectmember = db.ProjectMembers.FirstOrDefault(p => (p.ID == id));

            if (projectmember == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.ProjectMembers.Remove(projectmember);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, projectmember);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}