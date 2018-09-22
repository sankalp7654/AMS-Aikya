using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;
using LinqToExcel;
using System.Data.SqlClient;

namespace AttendanceManagement.Controllers
{
    public class BulkUploadController : Controller
    {
        private AttendanceManagementDBEntities1 db = new AttendanceManagementDBEntities1();

        // GET: BulkUpload
        public ActionResult Index()
        {
            return View();
        }

        // <summary>  
        // This function is used to download excel format.  
        // and exec it
        // </summary>  
        // <param name="Path"></param>  
        // <returns>file</returns>  
        //public FileResult DownloadExcel()
        //{
        //    string path = "/Doc/Details.xlsx";
        //    return File(path, "application/vnd.ms-excel", "Details.xlsx");
        //}

        [HttpPost]
        public JsonResult UploadExcel(Student students, HttpPostedFileBase FileUpload)
        {

            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {


                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/Doc/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Student$]", connectionString);
                    var adapter1 = new OleDbDataAdapter("SELECT * FROM [Subject$]", connectionString);
                    var adapter2 = new OleDbDataAdapter("SELECT * FROM [Department$]", connectionString);
                    var adapter3 = new OleDbDataAdapter("SELECT * FROM [Teacher$]", connectionString);

                    var ds = new DataSet();
                    var ds1 = new DataSet();
                    var ds2 = new DataSet();
                    var ds3 = new DataSet();

                    adapter.Fill(ds, "ExcelTable");
                    adapter1.Fill(ds1, "ExcelTable");
                    adapter2.Fill(ds2, "ExcelTable");
                    adapter3.Fill(ds2, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];
                    DataTable dtable1 = ds1.Tables["ExcelTable"];
                    DataTable dtable2 = ds2.Tables["ExcelTable"];
                    DataTable dtable3 = ds3.Tables["ExcelTable"];


                    string sheetName = "Sheet1";
                    string sheetName1 = "Sheet2";
                    string sheetName2 = "Sheet3";
                    string sheetName3 = "Sheet4";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var stud = from a in excelFile.Worksheet<Student>(sheetName) select a;

                    foreach (var a in stud)
                    {
                        try
                        {
                            if (a.USN != "" && a.Name != "" && a.Section != null && a.Sem != 0 && a.Department_DID != null)
                            {
                                Student TU = new Student();
                                TU.Name = a.Name;
                                TU.USN = a.USN;
                                TU.Sem = a.Sem;
                                TU.Department_DID = a.Department_DID;
                               // TU.Subject_SubCode = a.Subject_SubCode;
                                db.Students.Add(TU);
                                db.SaveChanges();
                            }
                            else
                            {
                                data.Add("<ul>");
                                if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                if (a.USN == "" || a.USN == null) data.Add("<li> USN is required</li>");
                                if (a.Sem == 0) data.Add("<li>Sem is required</li>");
                                if (a.Section == "" || a.Section == "" ) data.Add("<li>Section is required</li>");
                                if (a.Department_DID == "" || a.Department_DID == "") data.Add("<li>Department_DID is required</li>");
                                //
                              //  if (a.Subject_SubCode == "" || a.Subject_SubCode == "") data.Add("<li>Subject SubCode is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    var subjects = from b in excelFile.Worksheet<Subject>(sheetName1) select b;

                    foreach (var b in subjects)
                    {
                        try
                        {
                            if (b.SubCode != null && b.Name != "" && b.Department_DID != null)
                            {
                                Subject TU = new Subject();
                                TU.SubCode = b.SubCode;
                                TU.Name = b.Name;
                                TU.Department_DID = b.Department_DID;
              
                                db.Subjects.Add(TU);
                                db.SaveChanges();
                            }
                            else
                            {
                                data.Add("<ul>");
                                if (b.SubCode == null) data.Add("<li> SubCode is required</li>");
                                if (b.Department_DID == null) data.Add("<li> deptId is required</li>");
                                if (b.Name == null) data.Add("<li>Name is required </li>");
                                if (b.SubCode == "" || b.SubCode == null) data.Add("<li>Block is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }

                    var teachers = from c in excelFile.Worksheet<Teacher>(sheetName2) select c;

                    foreach (var c in teachers)
                    {
                        try
                        {
                            if (c.TID != null && c.Name != "" && c.Department_DID != "")
                            {
                                Teacher TU = new Teacher();
                                TU.TID = c.TID;
                                TU.Name = c.Name;
                                TU.Department_DID = c.Department_DID;
                                db.Teachers.Add(TU);
                                db.SaveChanges();
                            }
                            else
                            {
                                data.Add("<ul>");
                                if (c.TID == null) data.Add("<li> TID is required</li>");
                                if (c.Name == null || c.Name == "") data.Add("<li> name is required</li>");
                                if (c.Department_DID == null) data.Add("<li>department id  is required</li>");


                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }

                    var department = from a in excelFile.Worksheet<Department>(sheetName3) select a;

                    foreach (var a in department)
                    {
                        try
                        {
                            if (a.DID != "" && a.Name != "")
                            {
                                Department TU = new Department();
                                TU.Name = a.Name;
                                TU.DID = a.DID;
                                db.Departments.Add(TU);
                                db.SaveChanges();
                            }
                            else
                            {
                                data.Add("<ul>");
                                if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                if (a.DID == "" || a.DID == null) data.Add("<li> DID is required</li>");
                                
                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {

                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);

                                }

                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
   