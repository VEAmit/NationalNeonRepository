using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NationalNeon.Business.Interfaces;
using NationalNeon.Domain.Job;
using NationalNeon.Web.ViewModels;
using ExpressMapper;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NationalNeon.Utility.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NationalNeon.Web.Controllers
{
    public class JobsController : Controller
    {
        private readonly IJobBusiness ijobBusiness;
        private readonly ITaskBusiness itaskBusiness;
        private readonly IEmployeeBusiness iEmployeeBusiness;
        private readonly IHostingEnvironment ihostingEnv;
        public JobsController(IJobBusiness ijobBusiness, IHostingEnvironment ihostingEnv, ITaskBusiness itaskBusiness, IEmployeeBusiness iEmployeeBusiness)
        {
            this.ijobBusiness = ijobBusiness;
            this.ihostingEnv = ihostingEnv;
            this.itaskBusiness = itaskBusiness;
            this.iEmployeeBusiness = iEmployeeBusiness;
        }
        [Route("Job")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddJobs()
        {
            //EmployeeType employeeType = EmployeeType.Sales;
            //var empTypeList = EnumHelper<EmployeeType>.GetValues(employeeType).Select(item => new { Id = (int)item, Name = item.ToString() }).ToList();

            var usersList = iEmployeeBusiness.GetAllEmployeeBySales();
            ViewBag.EmployeeType = new SelectList(usersList, "EmployeeId", "FirstName");


            //ViewBag.EmployeeType = new SelectList(empTypeList, "Id", "Name");
            return PartialView("_AddJobs");

        }
        public ActionResult JobsList()
        {
            //var model = ijobBusiness.GetAllJobsOnView();
            //ViewBag.Model = model;
            //return PartialView("_JobsList");
            var model = ijobBusiness.GetAllJobsOnView();

            foreach (var modalItem in model)
            {
                if (modalItem.Tasks.Count > 0)
                {
                    var completedTaskTotalHours = modalItem.Tasks.Where(row => row.Completed == 1).Sum(row => Convert.ToDecimal(row.BudgetedHours));
                    var totalHours = modalItem.Tasks.Sum(row => Convert.ToDecimal(row.BudgetedHours));
                    modalItem.ProgressPercent = Convert.ToString(Convert.ToInt32(Math.Round(completedTaskTotalHours / totalHours, 2) * 100)) + "%";
                }
            }
            ViewBag.Model = model;
            return PartialView("_JobsList");
        }

        public ActionResult GetCalenderJobsList()
        {
            var model = ijobBusiness.GetCalenderJobsList().Select(x =>
            new
            {
                title = x.job_name,
                start = x.target_completion_date,
                className = x.status == "Archived" ? new[] { "event", "bg-color-greenLight" } : new[] { "event", "bg-color-red" },
                icon = "fa fa-briefcase",
                jobid = x.jobId
            }).ToList();

            return Json(new
            {
                success = true,
                data = model
            });
        }

        [HttpPost]
        public ActionResult AddJobs(JobViewModel jobModel)
        {
            try
            {
                JobModel data = new JobModel();
                Mapper.Map(jobModel, data);
                if (jobModel.jobId == 0)
                {
                    ijobBusiness.AddJobs(data);
                    return Json(new
                    {
                        success = true,
                        title = "<strong>Success:</strong>",
                        type = "success",
                        message = "Jobs added Succesfully",
                        action = "added"
                    });
                }
                else
                {
                    // ijobBusiness.UpdateJobModel(model);
                    var datamodal = new JobModel();
                    datamodal.jobId = jobModel.jobId;
                    datamodal.job_name = jobModel.job_name;
                    datamodal.job_number = jobModel.job_number;
                    datamodal.revenue = jobModel.revenue;
                    datamodal.description = jobModel.description;
                    datamodal.special_Notes = jobModel.special_Notes;
                    datamodal.sales_person = jobModel.sales_person;
                    datamodal.scheduled_date = jobModel.scheduled_date;
                    datamodal.status = jobModel.status;
                    datamodal.target_completion_date = jobModel.target_completion_date;
                    ijobBusiness.UpdateJobModel(datamodal);
                }
                return Json(new
                {
                    success = true,
                    title = "<strong>Success:</strong>",
                    type = "info",
                    message = "Jobs updated Succesfully",
                    action = "edit"
                });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Something went wrong.";
            }
            return RedirectToAction("JobsList");
        }

        public ActionResult Delete(int id)
        {
            ijobBusiness.DeleteJobs(id);
            return Json(new
            {
                success = true
            });
            // return RedirectToAction("JobsList");
        }

        public ActionResult GetJobs(int id)
        {
            var jobDetails = ijobBusiness.GetJob(id);
            return Json(new
            {
                success = true,
                data = jobDetails
            });
        }

        [HttpPost]
        public ActionResult UpdateJobs(JobViewModel jobModel)
        {
            try
            {
                JobModel data = new JobModel();
                Mapper.Map(jobModel, data);

                ijobBusiness.UpdateJobModel(data);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Something went wrong.";
            }
            return RedirectToAction("JobsList");
        }


        [HttpPost]
        public async Task<ActionResult> saveUploadFile(int jobId)
        {
            try
            {
                string filePath = string.Empty, fileDatabasePath = string.Empty, fileUniquePath = string.Empty;
                var files = Request.Form.Files;
                foreach (var file in files)
                {
                    string filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    filename = this.EnsureCorrectFilename(filename);
                    fileUniquePath = "\\uploads\\" + jobId + "_" + Path.GetFileNameWithoutExtension(filename.Replace("_","")) + "_" + DateTime.UtcNow.ToFileTime().ToString() + Path.GetExtension(filename);
                    filePath = Path.Combine(this.ihostingEnv.WebRootPath + fileUniquePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                }
                if (UpdateJobFilePath(jobId, fileUniquePath))
                {
                    return Json(new
                    {
                        success = true
                    });
                }
                else
                {
                    return Json(new
                    {
                        error = true
                    });
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Something went wrong.";
                return Json(new
                {
                    error = true
                });
            }

        }
        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);
            return filename;
        }
        private bool UpdateJobFilePath(int jobId, string filePath)
        {
            try
            {
                
                JobFileUploadModel jobFileUploadModel = new JobFileUploadModel();
                jobFileUploadModel.JobId = jobId;
                jobFileUploadModel.FilePath = filePath;
                ijobBusiness.UpdateJobFileUploadPath(jobFileUploadModel);
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }
        public ActionResult Detail(int jobId)
        {
            
            var model = ijobBusiness.GetJob(jobId);
            var usersList = iEmployeeBusiness.GetAllEmployeeBySales();
            ViewBag.EmployeeType = new SelectList(usersList, "FirstName", "FirstName");

            if (model.Tasks.Count>0)
            {
                var completedTaskTotalHours = model.Tasks.Where(row => row.Completed == 1).Sum(row => Convert.ToDecimal(row.BudgetedHours));

                var totalHours = model.Tasks.Sum(row => Convert.ToDecimal(row.BudgetedHours));
                ViewBag.progress = Convert.ToString(Convert.ToInt32(Math.Round(completedTaskTotalHours / totalHours, 2) * 100)) + "%";

                model.Tasks = itaskBusiness.GetAll().Where(row => row.jobId == jobId).ToList();
            }

           // ViewBag.jobFiles = JsonConvert.SerializeObject(model.JobFileUpload);
            return View(model);
        }
        [HttpPost]
        public ActionResult DeleteUploadedFile(int jobFileUploadId)
        {
            try
            {
                ijobBusiness.DeleteUploadedFile(jobFileUploadId);
                return Json(new
                {
                    success = true
                });
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    success = false
                });
            }
        }

        [HttpGet]
        public ActionResult GetJobFiles(int jobId)
        {
            try
            {
                var model = ijobBusiness.GetJob(jobId);
                if (model.JobFileUpload.Count > 0)
                {
                    return Json(new { data = model.JobFileUpload, dataCount = model.JobFileUpload.Count }, new JsonSerializerSettings() { Formatting = Formatting.Indented });
                }
                else
                {
                    return Json(new { data = "", dataCount = 0 });
                }
            }
            catch(Exception ex)
            {
                return Json(new { data = "Error", dataCount = -1 });
            }

        }
        [HttpPost]
        public async Task<ActionResult> UploadFiles(string jobId)
        {
            string filePath = string.Empty, fileDatabasePath = string.Empty, fileUniquePath = string.Empty;
            var files = Request.Form.Files;
            foreach (var file2 in files)
            {
                string filename = ContentDispositionHeaderValue.Parse(file2.ContentDisposition).FileName.Trim('"');
                filename = this.EnsureCorrectFilename(filename);
                fileUniquePath = "\\uploads\\"  + Path.GetFileNameWithoutExtension(filename.Replace("_", "")) + "_" + DateTime.UtcNow.ToFileTime().ToString() + Path.GetExtension(filename);
                filePath = Path.Combine(this.ihostingEnv.WebRootPath + fileUniquePath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file2.CopyToAsync(stream);
                }
            }
     
            if (!string.IsNullOrEmpty(jobId))
            {
                if (UpdateJobFilePath(Convert.ToInt32(jobId), fileUniquePath))
                {
                    return Json(new
                    {
                        success = true
                    });
                }
                
            }
            return Json(new
            {
                error = true
            });
        }
        public IActionResult CompleteTask(int taskId)
        {
            try
            {
                itaskBusiness.updateincompleteTask(taskId);
                return Json(new
                {
                    success = true
                });
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    success = false,
                });

            }
            
        }

        public IActionResult UpdateJobTargetDate(int jobId, DateTime targetCompletionDate)
        {
            try
            {
                ijobBusiness.UpdateJobTargetDate(jobId, targetCompletionDate);
                return Json(new
                {
                    success = true,
                    data = "Success"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    data = "Error"
                });
            }

        }

    }
}
