﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommerceAspNetMvc.DB;

namespace EcommerceAspNetMvc.Controllers
{
    public class BaseController : Controller
    {
        protected EcommerceDbEntities Context { get; private set; }
        public BaseController()
        {
            Context = new EcommerceDbEntities();
            ViewBag.MenuCategories = Context.Categories.Where(x => x.Parent_Id == null).ToList();
        }

        protected Members CurrentUser()
        {
            return (Members) Session["logonuser"];
        }

        protected int CurrentUserId()
        {
            return ((Members)Session["logonuser"]).Id;
        }

        protected bool IsLogon()
        {
            if (Session["logonuser"] == null) 
                return false;
            else 
                return true;
        }
    }
}