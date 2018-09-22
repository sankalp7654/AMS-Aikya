using AttendanceManagement.Models;
using AttendanceManagement.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceManagement.Controllers
{
    public class TeacherTeachesStudentController : Controller
    {
        private AttendanceManagementDBEntities1 db = new AttendanceManagementDBEntities1();
        private static TeacherTeachesStudentModel ttsmodel = new TeacherTeachesStudentModel();

        // GET: TeacherTeachesStudent
        public ActionResult Index()
        {
            ViewBag.Department_DID = new SelectList(db.Departments, "DID", "Name");
            ViewBag.Section = new SelectList(db.Students, "Section", "Section");
            ViewBag.Sem = new SelectList(db.Students, "Sem", "Sem");
            ViewBag.Sub_Code = new SelectList(db.Subjects, "SubCode", "SubCode");

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(TeacherTeachesStudentModel classViewModel)
        {
            ttsmodel.Sub_Code = classViewModel.Sub_Code;
            ttsmodel.Department_DID = classViewModel.Department_DID;
            ttsmodel.Sem = classViewModel.Sem;
            ttsmodel.Section = classViewModel.Section;


            AttendanceManagementDBEntities1 db = new AttendanceManagementDBEntities1();


            var customer = db.AspNetUsers.FirstOrDefault(u => u.Email == User.Identity.Name);
            var teacher = db.Teachers.FirstOrDefault(u => u.REFID == customer.Id);



            classViewModel.Students = db.Students.Where(u => u.Section == classViewModel.Section).Where(u => u.Sem == classViewModel.Sem).Where(u => u.Department_DID == classViewModel.Department_DID).ToList();
            for (int i = 0; i < classViewModel.Students.Count; i++)
            {
                Student_Studies_Subject student_studies_subject = new Student_Studies_Subject();
                Teacher_Teaches_Student teacherTeachesStudent = new Teacher_Teaches_Student();
                teacherTeachesStudent.Teacher_TID = teacher.TID;
                teacherTeachesStudent.Subject_SubCode = classViewModel.Sub_Code;
                teacherTeachesStudent.Student_USN = classViewModel.Students[i].USN;
                student_studies_subject.SubCode = classViewModel.Sub_Code;
                student_studies_subject.USN = classViewModel.Students[i].USN;
                db.Teacher_Teaches_Student.Add(teacherTeachesStudent);
                db.Student_Studies_Subject.Add(student_studies_subject);
                db.SaveChanges();

            }

            return RedirectToAction("Index", "TeachersPortal");
        
        }


        public ActionResult Invalid()
        {
            return View();
        }
    }
}