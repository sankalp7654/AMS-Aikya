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
    public class TeachersController : Controller
    {
        private AttendanceManagementDBEntities1 db = new AttendanceManagementDBEntities1();

        // GET: Teachers
        public ActionResult Index()
        {
            var teachers = db.Teachers.Include(t => t.Department);
            return View(teachers.ToList());
        }

        // GET: Teachers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // GET: Teachers/Create
        public ActionResult Create()
        {
            ViewBag.Department_DID = new SelectList(db.Departments, "DID", "Name");
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TID,Name,Department_DID")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                db.Teachers.Add(teacher);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            ViewBag.Department_DID = new SelectList(db.Departments, "DID", "Name", teacher.Department_DID);
            return View(teacher);
        }



        // GET: Teachers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            ViewBag.Department_DID = new SelectList(db.Departments, "DID", "Name", teacher.Department_DID);
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TID,Name,Department_DID")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teacher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Department_DID = new SelectList(db.Departments, "DID", "Name", teacher.Department_DID);
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Teacher teacher = db.Teachers.Find(id);
            Teacher_Teaches_Student teacher_teaches_student = new Teacher_Teaches_Student();
            

            var subjects_taught_by_teacher = db.Teacher_Teaches_Student.Where(u => u.Teacher_TID == id).Select(u=>u.Subject_SubCode).ToList();
            var uniqueSubjects = new HashSet<String>(subjects_taught_by_teacher).ToList() ;

            for (int i = 0; i < uniqueSubjects.Count; i++)
            {
                var subject = uniqueSubjects[i];
                var student = db.Student_Studies_Subject.Where(u => u.SubCode == subject).ToList();
                for(int j=0; j<student.Count; j++)
                {
                    db.Student_Studies_Subject.Remove(student[j]);
                }
            }




            // Removes all the students that are taught by the Teacher with given Teacher Id i.e. id

            var TTS = db.Teacher_Teaches_Student.Where(u => u.Teacher_TID == id).ToList();
            for (int i = 0; i < TTS.Count(); i++)
            {
                db.Teacher_Teaches_Student.Remove(TTS[i]);
            }




            db.Teachers.Remove(teacher);
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
