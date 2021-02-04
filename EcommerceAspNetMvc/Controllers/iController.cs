﻿using EcommerceAspNetMvc.DB;
using EcommerceAspNetMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EcommerceAspNetMvc.Controllers
{
    public class iController : BaseController
    {


        public ActionResult Index(int? id)
        {
            IQueryable<Products> product = Context.Products;
            Categories category = null;

            if (id.HasValue)
            {
                product = product.Where(x => x.Category_Id == id);
                category = Context.Categories?.FirstOrDefault(x => x.Id == id);
            }


            var model = new HomeViewModel
            {
                Products = product.ToList(),
                Category = category
            };

            List<Categories> categories = new List<Categories>();
            if (model.Category != null)
            {
                categories.Add(model.Category);
                var parentcat = model.Category.Categories2;
                while (parentcat != null)
                {
                    categories.Add(parentcat);
                    parentcat = parentcat.Categories2;
                }
            }

            TempData["cat"] = categories;
            return View(model);
        }

        [HttpGet]
        public ActionResult Product(int id)
        {
            var product = Context.Products.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return RedirectToAction("Index", "i");
            }
            ProductViewModel model = new ProductViewModel()
            {
                Product = product,
                Comments = product.Comments.ToList()
            };
            return View(model);
        }


        [HttpPost]
        public ActionResult Product(ProductViewModel model)
        {
            try
            {
                Comments com = new Comments
                {
                    Product_Id = model.Product.Id,
                    Text = model.Comment.Text,
                    AddedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Member_Id = CurrentUserId()
                };
                Context.Comments.Add(com);
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.info = "Yorum eklenirken bir hata meydana geldi" + " -> " + e.Message;

            }
            return RedirectToAction("Product", "i");
        }

        [HttpGet]
        public ActionResult AddBasket(int id, bool remove = false)
        {
            List<BasketViewModel> basket = null;
            if (Session["Basket"] == null)
            {
                basket = new List<BasketViewModel>();
            }
            else
            {
                basket = (List<BasketViewModel>)Session["Basket"];
            }

            if (basket.Any(x => x.Product.Id == id))
            {
                var pro = basket.FirstOrDefault(x => x.Product.Id == id);
                if (pro != null)
                {
                    if (remove && pro.Count > 0)
                    {
                        pro.Count -= 1;
                        
                    }
                    else
                    {
                        if (pro.Product.UnitsInStock > pro.Count)
                        {
                            pro.Count += 1;
                        }
                        else
                        {
                            TempData["info"] = "Daha fazla ürün eklenemez";
                        }
                    }
                }

            }
            else
            {
                var pro = Context.Products.FirstOrDefault(x => x.Id == id);
                if (pro != null && pro.IsContinued && pro.UnitsInStock>0)
                {
                    basket.Add(new BasketViewModel()
                    {
                        Count = 1,
                        Product = pro
                    });
                }
                else if (pro != null && !pro.IsContinued)
                {
                    TempData["info"] = "Bu ürünün satışı duruduruldu.";
                }

            }

            basket.RemoveAll(x => x.Count < 1);
            Session["Basket"] = basket;

            return RedirectToAction("Basket", "i");
        }


        [HttpGet]
        public ActionResult Basket()
        {

            List<BasketViewModel> model = (List<BasketViewModel>)Session["Basket"];

            if (model == null)
            {
                model = new List<BasketViewModel>();
            }



            ViewBag.totalPrice = model.Select(x => x.Product.Price * x.Count).Sum();

            return View(model);
        }


        [HttpGet]
        public ActionResult RemoveBasket(int id=0)
        {
            List<BasketViewModel> basket = (List<BasketViewModel>)Session["Basket"];
            
            if (basket!=null)
            {
                if (id>0)
                {
                    basket.RemoveAll(x => x.Product.Id == id);
                    
                }
                else if(id==0)
                {
                    basket.Clear();
                }
                Session["Basket"] = basket;
            }

            return RedirectToAction("Basket", "i");

        }

    }
}