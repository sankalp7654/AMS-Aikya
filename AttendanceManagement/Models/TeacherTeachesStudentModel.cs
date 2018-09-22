using AttendanceManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceManagement.Models
{
    public class TeacherTeachesStudentModel
    {
   
        public string Department_DID { get; set; }
        public int Sem { get; set; }
        public string Section { get; set; }
        public string Sub_Code { get; set; }
        public List<Student> Students { get; set; }

    }
}