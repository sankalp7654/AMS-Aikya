using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;
using AttendanceManagement.ViewModel;
using System.Data.Entity;
using System.Collections;


namespace AttendanceManagement.Controllers
{
    public class ManageAttendanceController : Controller
    {
        private AttendanceManagementDBEntities1 db = new AttendanceManagementDBEntities1();
        
        private static AttendanceViewModel attendanceViewModel = new AttendanceViewModel();


        // GET: ManageAttendance/Index
        public ActionResult Index()
        {

            // new SelectList(db.students, "field1", "field2")
            // field1 denotes which field from db has to be selected
            // field2 denotes which attribute corresponding to field1 has to be passed to controller URL

            ViewBag.Department_DID = new SelectList(db.Departments, "DID", "Name");
            ViewBag.Section = new SelectList(db.Students, "Section", "Section");
            ViewBag.Sem = new SelectList(db.Students, "Sem", "Sem");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ManageAttendanceModel classViewModel)
        {
            return RedirectToAction("AddAttendance", "ManageAttendance", new { departmentID = classViewModel.Department_DID, Semester = classViewModel.Sem, section = classViewModel.Section, slot = classViewModel.Slot, date = classViewModel.Date });
        }

        [HttpGet]
        public ActionResult AddAttendance(string departmentID, int Semester, string section, string slot, DateTime date)
        {
            attendanceViewModel.Slot = slot;
            attendanceViewModel.Date = date.ToShortDateString(); ;
            var students = db.Students.Where(s => s.Department_DID == (departmentID)).Where(s => s.Sem == (Semester)).Where(s => s.Section == (section)).ToList();
            attendanceViewModel.Students = students;
            //var subjectCode = students[0].Subject_SubCode;
            //var query = db.Teacher_Teaches_Student.Where(s => s.Subject_SubCode == subjectCode).Select(t => t.Teacher_TID).ToList();


            
            var customer = db.AspNetUsers.FirstOrDefault(u => u.Email == User.Identity.Name);

            var teacher = db.Teachers.FirstOrDefault(u => u.REFID == customer.Id);
            
            attendanceViewModel.TeacherId = teacher.TID;
            //Changes are made
            /*var teacherList = db.Teacher_Teaches_Student.Where(u => u.Teacher_TID == teacher.TID).ToList();
            var subject = teacherList.Where(s => s.Student_USN == students[0].USN).FirstOrDefault();*/
            string usn = students[0].USN;
            var tts = db.Teacher_Teaches_Student.Where(u => u.Teacher_TID == teacher.TID).FirstOrDefault(u => u.Student_USN == usn);

            attendanceViewModel.SubjectCode = tts.Subject_SubCode;
            return View(attendanceViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAttendance([Bind(Include = "IsPresent")] AttendanceViewModel model)
        {
            int countOfStudents = attendanceViewModel.Students.Count;
            var checkBoxes = model.IsPresent.ToList();
            var students = attendanceViewModel.Students;
            for (int i = 0; i < countOfStudents; i++)
            {

                Attendance attendance = new Attendance();
                attendance.Date = attendanceViewModel.Date;
                attendance.Slot = attendanceViewModel.Slot;
                attendance.Subject_SubCode = attendanceViewModel.SubjectCode;
                attendance.Teacher_TID = attendanceViewModel.TeacherId;
                attendance.Teacher = db.Teachers.Find(attendance.Teacher_TID);
                attendance.Subject = db.Subjects.Find(attendance.Subject_SubCode);
                attendance.Student_USN = students[i].USN;
                if (checkBoxes[i])
                    attendance.IsPresent = 1;
                else
                    attendance.IsPresent = 0;
                //attendance.Student = db.Students.Find(students[i].USN);
                if (ModelState.IsValid)
                {
                    db.Attendances.Add(attendance);
                    db.SaveChanges();
                }
                /*     if (ModelState.IsValid)
                     {
                         db.Attendances.Add(attendance);
                         db.SaveChanges();
                         return RedirectToAction("Index");*/
            }
            return RedirectToAction("Index");
        }

        public ActionResult Invalid()
        {
            return View();
        }

    }
}