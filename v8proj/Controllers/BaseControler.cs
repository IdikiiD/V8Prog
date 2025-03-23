using System;
using System.Web.Mvc;

namespace v8proj.Controllers
{
    public class BaseControler : Controller
    {
        protected string GetLastUrl() => Request.UrlReferrer?.ToString() ?? "/Home";
        protected ActionResult RedirectToLastPage() => Redirect(GetLastUrl());
    }
}