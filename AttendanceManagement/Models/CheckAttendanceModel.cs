using AttendanceManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceManagement.Models
{
    public class CheckAttendanceModel
    {

        


        public string Department_DID { get; set; }
        public int Sem { get; set; }
        public string Section { get; set; }
        public string Slot { get; set; }
        public string Date { get; set; }



        // [Required]
        //  [Display(Name = "Date")]
        // public DateTime Date { get; set; }



        //   [Required]
        //   [Display(Name = "Slot")]
        //   public int Slot { get; set; }

    }
}