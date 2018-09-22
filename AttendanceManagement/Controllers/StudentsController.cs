using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;

namespace AttendanceManagement.Controllers
{
    public class StudentsController : Controller
    {
        private AttendanceManagementDBEntities1 db = new AttendanceManagementDBEntities1();
        private static Student_Studies_Subject student_studies_subject = new Student_Studies_Subject();

        // GET: Students
        public ActionResult Index()
        {

            //    var students = db.Students.Include(s => s.Department).Include(s => s.Subject);
            var students = db.Students.Include(s => s.Department);

            return View(students.ToList());
        }

        // GET: Students/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            ViewBag.Department_DID = new SelectList(db.Departments, "DID", "Name");
            ViewBag.Subject_SubCode = new SelectList(db.Subjects, "SubCode", "Name");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "USN,Name,Section,Sem,Department_DID")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Department_DID = new SelectList(db.Departments, "DID", "Name", student.Department_DID);

           
            return View(student);
        }


        // GET: Students/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);


            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Student student = db.Students.FirstOrDefault(u => u.USN == id);

            var sss = db.Student_Studies_Subject.Where(u => u.USN == id).ToList();
            student_studies_subject.sss = sss;

            for (int i = 0; i < sss.Count; i++)
                db.Student_Studies_Subject.Remove(sss[i]);

            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
