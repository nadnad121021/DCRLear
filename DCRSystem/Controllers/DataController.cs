﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using DCRSystem.Models;
using DCRSystem.DataModel;
using DCRSystem.CustomViewModel;
using Rotativa;

namespace DCRSystem.Controllers
{
    public class DataController : Controller
    {
        // GET: Data
        lear_DailiesCertificationRequirementEntities entities = new lear_DailiesCertificationRequirementEntities();
        public FileResult ExportEmployeeDetailsToExcel( String id )
        {
            var employeeDetails = entities.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id).FirstOrDefault();
            var employeeProgess = GetEmployeeDetails(id);
            if (employeeProgess.FullName != null && employeeDetails != null)
            {
                DataTable dt = new DataTable("Employee Details");

                dt.Columns.AddRange(new DataColumn[7] { new DataColumn("Employee ID"),
                                            new DataColumn("Employee Name"),
                                            new DataColumn("Position"),
                                            new DataColumn("HRC Cell"),
                                            new DataColumn("Current Cell"),
                                            new DataColumn("HRC Supervisor"),
                                            new DataColumn("Current Supervisor"),
                                           });


                //var certificates = from Certification in entities.Certifications
                //                   select Certification;

                dt.Rows.Add(employeeDetails.Employee_ID,employeeDetails.Last_Name +", "+employeeDetails.First_Name,employeeDetails.Position,employeeDetails.HRCCell
                            ,employeeDetails.CurrentCell,employeeDetails.HRCSupervisor,employeeDetails.CurrentSupervisor);

                //DataTable progress = new DataTable("Grid1");
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Certificate Code"),
                                            new DataColumn("Description"),
                                            new DataColumn("Type"),
                                            new DataColumn("Date Certified"),
                                            new DataColumn("Date Recertified"),
                                           new DataColumn("Certification Plan"),
                                            new DataColumn("Status"),
                                            new DataColumn("Identifier")});
                //dt.Rows.Add("", "", "", "", "", "", "", "","","");
                //dt.Rows.Add("", "Employee's Certificates :", "", "", "", "", "", "", "", "");

                foreach (var certificate in employeeProgess.EmployeeCertificates)
                {
                    dt.Rows.Add("", "", "", "", "", "", "", certificate.CertificateCode, certificate.Description, certificate.Type, certificate.DateCertified,certificate.DateRecertified,
                        certificate.CertificationPlan,certificate.Status,certificate.Identifier);
                }

                for (int i = 0; i < 2; i++)
                {
                    dt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "");

                }
                dt.Rows.Add("", "Skills Certified :", employeeProgess.SkillsCertified, "", "", "", "", "");
                dt.Rows.Add("", "Percentage :", employeeProgess.SkillsCertifiedPercentage, "", "", "", "", "");
                dt.Rows.Add("", "Points :", employeeProgess.SkillsCertifiedPoints, "", "", "", "", "");
                dt.Rows.Add("", "", "", "", "", "", "", "");
                dt.Rows.Add("", "", "", "", "", "", "", "");
                dt.Rows.Add("", "Skills Re-Certified :", employeeProgess.SkillsReCertified, "", "", "", "", "");
                dt.Rows.Add("", "Percentage :", employeeProgess.SkillsReCertifiedPercentage, "", "", "", "", "");
                dt.Rows.Add("", "Points :", employeeProgess.SkillsReCertifiedPoints, "", "", "", "", "");
                dt.Rows.Add("", "", "", "", "", "", "", "");
                dt.Rows.Add("", "", "", "", "", "", "", "");
                dt.Rows.Add("Details Of :", employeeProgess.EmpBadgeNo + " - " + employeeProgess.FullName, "", "", "", "", "Date Exported :", DateTime.Now.ToString());

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    //wb.Worksheets.Add(progress);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", employeeProgess.FullName+"'s-Details.xlsx");
                    }
                }
            }
            else
            {
                return null;
            }

           
        }

        public EmployeeProgressDetails GetEmployeeDetails(String id)
        {
            EmployeeProgressDetails empP = new EmployeeProgressDetails();
            var Certificates = entities.Certifications.ToList();
            var CertificationTracker = entities.certificateTracker_Vw.ToList();
            List<CertificationTracker> MyCertificates = new List<CertificationTracker>();
            var NumberOfCertified = 0;
            var NumberOfRecertified = 0;
            int TotalCertifiedPoints = 0;
            int TotalReCertifiedPoints = 0;

            CertificationPlanGenerator generator = new CertificationPlanGenerator();

            if (id != null)
            {
                var employee = entities.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id).FirstOrDefault();

                if (employee != null)
                {
                    empP.EmpBadgeNo = employee.Employee_ID;
                    empP.FullName = employee.Last_Name + " , " + employee.First_Name;
                    empP.Position = employee.Position;
                    empP.HrcCell = employee.HRCCell;
                    empP.HrcSupervisor = employee.HRCSupervisor;
                    empP.CurrentCell = employee.CurrentCell;
                    empP.CurrentSupervisor = employee.CurrentSupervisor;
                    empP.SkillsCertified = entities.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID).OrderBy(cr => cr.CertificationCode).ToList().Count() + " out of " + Certificates;

                    NumberOfCertified = entities.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID).OrderBy(cr => cr.CertificationCode).ToList().Count();
                    empP.SkillsCertified = NumberOfCertified + " out of " + Certificates.Count();

                    NumberOfRecertified = entities.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID && cr.DateRecertified != null).OrderBy(cr => cr.Id).ToList().Count();
                    empP.SkillsReCertified = NumberOfRecertified + " out of " + NumberOfCertified;

                    double SKP = (Convert.ToDouble(NumberOfCertified) / Convert.ToDouble(Certificates.Count())) * (100);
                    empP.SkillsCertifiedPercentage = !Double.IsNaN(SKP) ? Convert.ToInt32(SKP).ToString() + "%" : "0%";

                    double SRKP = (Convert.ToDouble(NumberOfRecertified) / Convert.ToDouble(Certificates.Count())) * (100);
                    empP.SkillsReCertifiedPercentage = !Double.IsNaN(SRKP) ? Convert.ToInt32(SRKP).ToString() + "%" : "0%";

                    MyCertificates = entities.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID).OrderBy(cr => cr.CertificationCode).ToList();

                    foreach (var certif in Certificates)
                    {
                        var existCert = MyCertificates.Where(cert => cert.CertificationCode == certif.Code).FirstOrDefault();
                        if (existCert != null)
                        {
                            TotalCertifiedPoints = TotalCertifiedPoints + Convert.ToInt32(certif.Points);
                            if (existCert.DateRecertified != null)
                            {
                                TotalReCertifiedPoints = TotalReCertifiedPoints + Convert.ToInt32(certif.Points);
                            }
                            empP.EmployeeCertificates.Add(
                                new EmployeeCertificates()
                                {
                                    CertificateCode = certif.Code,
                                    Description = certif.Description,
                                    Type = certif.Type,
                                    DateCertified = Convert.ToDateTime(existCert.DateCertified).ToShortDateString(),
                                    DateRecertified = existCert.DateRecertified == null ? "Not yet Recertified" : Convert.ToDateTime(existCert.DateRecertified).ToShortDateString(),
                                    CertificationPlan = generator.GetCertificationPlanA(certif.Code, existCert).ToShortDateString(),
                                    Status =  generator.DateNowAddMonth(-2) <= generator.GetCertificationPlanA(certif.Code, existCert) ?"Active" : "In-Active",
                                    Identifier = GenerateIdentifier(certif.Code,existCert)
                                }

                                );
                            
                        }
                    }
                    empP.SkillsCertifiedPoints = TotalCertifiedPoints.ToString();
                    empP.SkillsReCertifiedPoints = TotalReCertifiedPoints.ToString();


                }
                
            }
            return empP;

        }

        public String GenerateIdentifier(String code ,CertificationTracker tracker)
        {
            CertificationPlanGenerator generator = new CertificationPlanGenerator();
            var DiffMonth = generator.DifferenceMonth(generator.GetCertificationPlanA(code, tracker));
            if (DiffMonth > 0)
            {
                if(DiffMonth <= 31)
                {
                    return "One Month Overdue";
                }else if(DiffMonth > 31 && DiffMonth < 63)
                {
                    return "Almost Two Months Overdue";
                }
                else
                {
                    return "Two Months Overdue";
                }
            }
            else
            {
                return "In due time";
            }
               
        }

        public ActionResult ModalDetailsPrintable(String id ,String urlBack, String redirectUrl)
        {
            DataController controllerData = new DataController();
            
            ViewBag.id = id;
            if (id != null) { ViewBag.id = id; }
            UrlModel mode = new UrlModel();
            if (id != null) { mode.EmpId = id; }
            mode.URLBack = urlBack;
            mode.RedirectUrl = redirectUrl;
            mode.employeeProgress = controllerData.GetEmployeeDetails(id);
            var emplo = entities.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id).FirstOrDefault();
            if (emplo != null)
            {
                mode.employeeProgress.Medal = emplo.Medal;
            }
            return View(mode);

        }

        public ActionResult Calendar()
        {
            return View();
        }

        public ActionResult PrintViewToPdf(String id)
        {
            DataController controllerData = new DataController();
            EmployeeProgressDetails model = new EmployeeProgressDetails();
            if(id != null)
            {
                model = controllerData.GetEmployeeDetails(id);
                model.LogoImagePath = "learlogo.jpg";
                var emplo = entities.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id).FirstOrDefault();
                if(emplo != null)
                {
                    model.Medal = emplo.Medal;
                }
                return new Rotativa.PartialViewAsPdf("_PartialViewDetails", model) {
                    FileName = model.FullName + "'s Details.pdf"
                 
                };
            }
            else
            {
                return new HttpNotFoundResult();
            }
            //Code to get content
            
        }
        public ActionResult _PartialViewDetails()
        {
            return View();
        }

        public FileResult ExportEmployeesToExcel(String Type = "", String data = "" ,String Month ="")
        {
            List<EmployeeDCR_Vw> employees = new List<EmployeeDCR_Vw>();
            SearchController searchControl = new SearchController();

            if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(data))
            {             
                if (Type.ToString().ToUpper().Equals("BYMONTH"))
                {
                    employees = searchControl.getEmployeeByMonth(data);
                }

                if (Type.ToString().ToUpper().Equals("BYYEAR"))
                {
                    employees = searchControl.getEmployeeByYear(data);
                }

                if (Type.ToString().ToUpper().Equals("BYCERTIFICATES"))
                {
                    employees = searchControl.getEmployeeByCertificates(data);
                }

                if (Type.ToString().ToUpper().Equals("BYRCERTIFICATES"))
                {
                    employees = searchControl.getEmployeeByRCertificates(data);
                }

                if (Type.ToString().ToUpper().Equals("BYCELL"))
                {
                    employees = searchControl.getEmployeeByCell(data);
                }

                // wala pa neh

                if (Type.ToString().ToUpper().Equals("BYMEDAL"))
                {
                    employees = searchControl.getEmployeeByMedal(data);
                }

                //if (Type.ToString().ToUpper().Equals("BYLASTNAME"))
                //{
                //    employees = getEmployeeByLastName(data);
                //}

                if (Type.ToString().ToUpper().Equals("BYMONTHANDYEAR") || Type.ToString().ToUpper().Equals("BYMONTHANDYEAR"))
                {
                    employees = searchControl.getEmployeeByMonthAndYear(data, Month);
                }

                
            }
            else
            {
                employees = entities.EmployeeDCR_Vw.Where(emp => emp.Job_Status.ToUpper().Contains("CURRENT")).OrderBy(a => a.Last_Name).ToList();
                
            }

            //PutToExcel(employees);
            //return Content("Under Constraction");
            return PutToExcel(employees);
        }

        public FileResult PutToExcel(List<EmployeeDCR_Vw> list)
        {
            DataTable dt = new DataTable("Employee Details");
            List<Certification> Certificate = entities.Certifications.ToList();
            dt.Columns.AddRange(new DataColumn[7] { new DataColumn("Employee ID"),
                                            new DataColumn("Employee Name"),
                                            new DataColumn("Position"),
                                            new DataColumn("HRC Cell"),
                                            new DataColumn("Current Cell"),
                                            new DataColumn("HRC Supervisor"),
                                            new DataColumn("Current Supervisor"),
                                           });
            var initial = GetEmployeeDetails(list.ElementAt(0).Employee_ID).EmployeeCertificates;
         
                dt.Columns.AddRange(new DataColumn[31]{
                                            new DataColumn(Certificate.ElementAt(0).Code),
                                            new DataColumn(Certificate.ElementAt(1).Code),
                                            new DataColumn(Certificate.ElementAt(2).Code),
                                            new DataColumn(Certificate.ElementAt(3).Code),
                                            new DataColumn(Certificate.ElementAt(4).Code),
                                            new DataColumn(Certificate.ElementAt(5).Code),
                                            new DataColumn(Certificate.ElementAt(6).Code),
                                            new DataColumn(Certificate.ElementAt(7).Code),
                                            new DataColumn(Certificate.ElementAt(8).Code),
                                            new DataColumn(Certificate.ElementAt(9).Code),
                                            new DataColumn(Certificate.ElementAt(10).Code),
                                            new DataColumn(Certificate.ElementAt(11).Code),
                                            new DataColumn(Certificate.ElementAt(12).Code),
                                            new DataColumn(Certificate.ElementAt(13).Code),
                                            new DataColumn(Certificate.ElementAt(14).Code),
                                            new DataColumn(Certificate.ElementAt(15).Code),
                                            new DataColumn(Certificate.ElementAt(16).Code),
                                            new DataColumn(Certificate.ElementAt(17).Code),
                                            new DataColumn(Certificate.ElementAt(18).Code),
                                            new DataColumn(Certificate.ElementAt(19).Code),
                                            new DataColumn(Certificate.ElementAt(20).Code),
                                            new DataColumn(Certificate.ElementAt(21).Code),
                                            new DataColumn(Certificate.ElementAt(22).Code),
                                            new DataColumn(Certificate.ElementAt(23).Code),
                                            new DataColumn(Certificate.ElementAt(24).Code),
                                            new DataColumn(Certificate.ElementAt(25).Code),
                                            new DataColumn(Certificate.ElementAt(26).Code),
                                            new DataColumn(Certificate.ElementAt(27).Code),
                                            new DataColumn("Total Skills"),
                                            new DataColumn("Percentage"),
                                            new DataColumn("Skills Points")
                                            });

            foreach(var i in list)
            {
                EmployeeProgressDetails emp = GetEmployeeDetails(i.Employee_ID);
                
                dt.Rows.Add(emp.EmpBadgeNo,emp.FullName,emp.Position,emp.HrcCell,emp.CurrentCell,emp.HrcSupervisor,emp.CurrentSupervisor
                            );

            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                //wb.Worksheets.Add(progress);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee's-Details.xlsx");
                }
            }
           
        }

    }
}