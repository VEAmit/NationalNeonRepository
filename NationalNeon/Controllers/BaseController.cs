﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace OnlineRibbonCore.Controllers
{

    public abstract class BaseController : Controller
    {
      
        public BaseController()
        {
           
        }
   
        protected void SetMessage(string message, bool error = false)
        {
            if (error)
                TempData["ERROR"] = message;
            else
                TempData["SUCCESS"] = message;
        }

        protected void SetErrorMessage(string message)
        {
            SetMessage(message, true);
        }

       
        protected int GetLoginId()
        {
            if (User == null)
                return 0;
            var c = User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            return c == null ? 0 : Convert.ToInt32(c.Value);
        }


        protected ViewResult ErrorView(Exception ex = null)
        {
            if (ex != null)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                ViewBag.ErrorMessage = ex.ToString();
                //CREModelsDataBase.DBLogger.LogError(DbContext, ex, "", 0, 0);
            }
            else
            {
                ViewBag.ErrorMessage = "Something went wrong.";
            }
            return View("Error");
        }

        protected ViewResult ErrorView(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View("Error");
        }

        public RedirectResult RedirectToErrorPage(string message)
        {
            SetErrorMessage(message);
            return Redirect("~/Home/Error");
        }

        protected JsonResult ErrorJson(string message, object data = null)
        {
            return Json(new { success = false, status = "Error", message = message, data = data });
        }

        protected JsonResult OkJson(object data = null, string message = null)
        {
            return Json(new { success = true, status = "OK", message = message, data = data });
        }

       
    }
}