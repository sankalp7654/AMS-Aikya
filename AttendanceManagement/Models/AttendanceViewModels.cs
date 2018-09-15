using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AttendanceManagement.Models;

namespace AttendanceManagement.ViewModel
{
    public class AttendanceViewModel
    {
        public List<Student> Students { get; set; }
        public List<Attendance> Attds{ get; set; }
        public string TeacherId { get; set; }
        public string Date { get; set; }
        public string Slot { get; set; }
        public string SubjectCode { get; set; }
        public List<Boolean> IsPresent { get; set; }

        public virtual Student Student { get; set; }
    }
}