﻿using DCRSystem.CustomViewModel;
using DCRSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCRSystem.Controllers
{
    [Authorize(Roles = "Default")]
    public class OrdinaryController : Controller
    {
        public commonEmployeesEntities cEE = new commonEmployeesEntities();
        public lear_DailiesCertificationRequirementEntities ldcr = new lear_DailiesCertificationRequirementEntities();
        [HttpGet]
        public ActionResult MyCertificate(String id)
        {
            
            EmployeeModel empModel = new EmployeeModel();
            if (id != null)
            {
                var employee = ldcr.EmployeeDCR_Vw.Where(emp => emp.Employee_ID == id && emp.Job_Status.ToUpper().Contains("CURRENT")).FirstOrDefault();
               // System.Diagnostics.Debug.WriteLine(employee.Job_Status + "hehehehe");
                if (employee != null)
                {
                    empModel.Employee = new EmployeeDCR_Vw {
                        Employee_ID = employee.Employee_ID,First_Name = employee.First_Name,Last_Name = employee.Last_Name,HRCCell = employee.HRCCell
                        ,Job_Status = employee.Job_Status,HRCSupervisor = employee.HRCSupervisor,PlanRecertificationDate = employee.PlanRecertificationDate,Position = employee.Position
                    };
                    empModel.Certifications = ldcr.Certifications.OrderBy(l => l.Code).ToList();
                    empModel.TotalCertifications = ldcr.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID).OrderBy(cr => cr.CertificationCode).ToList();
                    empModel.CurrentCertification = ldcr.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID).OrderByDescending(cr => cr.DateCertified).FirstOrDefault();
                    double SKP = (Convert.ToDouble(empModel.TotalCertifications.Count()) / Convert.ToDouble(empModel.Certifications.Count() + 2)) * (100);
                    if (!Double.IsNaN(SKP))
                    {
                       // ViewBag.SkillsCertifiedPercentage = Convert.ToInt32(SKP);
                        empModel.PercentAgeCertified = Convert.ToInt32(SKP);
                    }
                    else
                    {
                       // ViewBag.SkillsCertifiedPercentage = 0;
                        empModel.PercentAgeCertified = 0;
                    }
                    var ReCertified = ldcr.CertificationTrackers.Where(cr => cr.EmpBadgeNo == employee.Employee_ID && cr.DateRecertified != null).OrderBy(cr => cr.Id).ToList().Count();
                    // ViewBag.TotalReCertified = ReCertified;
                    empModel.NumberReCertified = ReCertified;
                    double SRKP = (Convert.ToDouble(ReCertified) / Convert.ToDouble(empModel.Certifications.Count() + 2)) * (100);
                    if (!Double.IsNaN(SRKP))
                    {
                       // ViewBag.SkillsReCertifiedPercentage = Convert.ToInt32(SRKP);
                        empModel.PercentAgeReCertified = Convert.ToInt32(SRKP);
                    }
                    else
                    {
                       // ViewBag.SkillsReCertifiedPercentage = 0;
                        empModel.PercentAgeReCertified = 0;
                    }

                    System.Diagnostics.Debug.WriteLine(Convert.ToInt32(SKP));
                    System.Diagnostics.Debug.WriteLine(Convert.ToInt32(SRKP));
                    var totalPointsCertiFied = 0;
                    var TotalPointsReCertified = 0;
                    foreach (var certification in empModel.Certifications)
                    {
                        if (empModel.TotalCertifications.FirstOrDefault(el => el.CertificationCode == certification.Code) == null)
                        {
                            CertificationTracker CT = new CertificationTracker();
                            CT.Id = 0;
                            CT.EmpBadgeNo = id;
                            CT.CertificationCode = certification.Code;
                            CT.DateCertified = null;
                            CT.DateCertified = null;
                            empModel.MyCertifications.Add(CT);
                        }
                        else
                        {
                            empModel.MyCertifications.Add(empModel.TotalCertifications.FirstOrDefault(el => el.CertificationCode == certification.Code));
                            totalPointsCertiFied += Convert.ToInt32(certification.Points);
                        }
                        if (empModel.TotalCertifications.FirstOrDefault(el => el.CertificationCode == certification.Code && el.DateRecertified != null) != null)
                        {
                            TotalPointsReCertified += Convert.ToInt32(certification.Points);
                        }
                    }
                    System.Diagnostics.Debug.WriteLine(empModel.MyCertifications.Count());
                    empModel.TotalPointsCertified = totalPointsCertiFied;
                    empModel.TotalPointsReCertified = TotalPointsReCertified;
                }
            }
            
            return View(empModel);
        }
    }
}
