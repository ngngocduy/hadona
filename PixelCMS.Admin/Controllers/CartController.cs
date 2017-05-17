using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PixelCMS.Core.Models;
using System.Collections;
using PagedList;
using System.Web.Security;
using WebMatrix.WebData;
// -----------------------------------------
// PIXEL CMS
// Admin Area / LanguageController.cs v1.0
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Areas.Admin.Controllers
{
    public class CartController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        //
        // GET: /Admin/Language/
        #region Order
        public ActionResult Index(string status, int? page, int? orderstatus, DateTime? tungay, DateTime? denngay, string ten)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.Order_View == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
            #endregion

            ViewBag.tungay = string.Format("{0:dd/MM/yyyy}", tungay);
            ViewBag.denngay = string.Format("{0:dd/MM/yyyy}", denngay);

            ViewBag.Status = status;
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var orders = db.Orders.Where(x =>
                 (orderstatus == null ? true : (x.Status_Id == orderstatus))
                 && (string.IsNullOrEmpty(ten) ? true : x.Name.Contains(ten))
                  && (tungay == null ? true : (x.Date_Add >= tungay))
                  && (denngay == null ? true : (x.Date_Add <= denngay))
                  ).OrderByDescending(x => x.Date_Add);
            return View(orders.ToPagedList(pageNumber, pageSize));

        }

        public enum ButtonType
        {
            Submit,
            SubmitandUpdate,
            SubmitandEdit
        }

        public class MultipleButtonsAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                Type tButtonType = typeof(ButtonType);
                foreach (var key in filterContext.HttpContext.Request.Form.AllKeys)
                {
                    if (Enum.IsDefined(tButtonType, key))
                    {
                        var pDesc = filterContext.ActionDescriptor.GetParameters()
                            .FirstOrDefault(x => x.ParameterType == tButtonType);
                        filterContext.ActionParameters[pDesc.ParameterName] = Enum.Parse(tButtonType, key);

                    }
                }
            }
        }

        // Edit Language
        public ActionResult Details(int id = 0)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.Order_View == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
            #endregion
            Order order = db.Orders.Find(id);
            order.Date_Modified = DateTime.Now;
            order.View = true;
            db.SaveChanges();
            if (order == null)
            {
                return HttpNotFound();
            }
            var currentculture = PixelCMS.Helpers.CultureHelper.GetCurrentNeutralCulture();
            List<SelectListItem> items = new List<SelectListItem>();
            var stt = db.Order_Status.Where(x => x.Active).OrderBy(x => x.Order).ToList();
            for (int i = 0; i < stt.Count(); i++)
            {
                if (stt[i].Id == order.Status_Id)
                {
                    items.Add(new SelectListItem { Selected = true, Text = stt[i].Order_Item_Language.FirstOrDefault(x => x.Lang == currentculture).Name, Value = stt[i].Id.ToString() });
                }
                else
                {
                    items.Add(new SelectListItem { Text = stt[i].Order_Item_Language.FirstOrDefault(x => x.Lang == currentculture).Name, Value = stt[i].Id.ToString() });
                }
            }
            ViewBag.Status_Id = items;
            return View(order);
        }

        [HttpPost]
        [MultipleButtons]
        public ActionResult Details(Order order, ButtonType buttonPressed)
        {
            if (ModelState.IsValid)
            {
                if (order.Status_Id == 5 && order.IsDestroy == false)//trạng thái hủy.
                {
                    //bật trạng thái hủy =true
                    order.IsDestroy = true;

                    var listDetail = db.Order_Details.Where(x => x.Order_Id == order.Id);
                    foreach (var item in listDetail)
                    {
                        //khi hủy đơn hàng + số thêm lượng trong kho trở lại
                        var variantCB = db.VariantAttributeCombinations.FirstOrDefault(x => x.Attribute == item.Attribute);
                        var product = db.Posts.Find(item.Product_Id);
                        // xét biến thể trước
                        if (variantCB != null)
                        {
                            variantCB.Quantity = variantCB.Quantity + item.Quantity;
                        }//+ trong kho 
                        else if (product != null && product.Product != null)
                        {
                            var inven = product.Product.Inventories.FirstOrDefault();
                            if (inven != null)
                            {
                                inven.Quantity = inven.Quantity + item.Quantity;
                            }
                        }
                    }
                }
                //nếu trả về các trạng thái khác - lại số lượng trong kho biến thể và sp.
                if (order.Status_Id != 5 && order.IsDestroy == true)
                {
                    //bật trạng thái hủy =false
                    order.IsDestroy = false;

                    var listDetail = db.Order_Details.Where(x => x.Order_Id == order.Id);
                    foreach (var item in listDetail)
                    {
                        //khi hủy đơn hàng - số thêm lượng trong kho trở lại
                        var variantCB = db.VariantAttributeCombinations.FirstOrDefault(x => x.Attribute == item.Attribute);
                        var product = db.Posts.Find(item.Product_Id);
                        // xét biến thể trước
                        if (variantCB != null)
                        {
                            variantCB.Quantity = variantCB.Quantity - item.Quantity;
                        }//- trong kho 
                        else if (product != null && product.Product != null)
                        {
                            var inven = product.Product.Inventories.FirstOrDefault();
                            if (inven != null)
                            {
                                inven.Quantity = inven.Quantity - item.Quantity;
                            }
                        }
                    }
                }

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();

                if (buttonPressed == ButtonType.SubmitandUpdate)
                {
                    foreach (var item in order.Order_Details)
                    {
                        var product = db.Posts.Find(item.Product_Id);
                        item.Product_Name = product.Title;
                        if (product.Price != null)
                        {
                            item.Unit_Price = product.Price;
                        }
                        else
                        {
                            item.Unit_Price = 0;
                        }
                    }
                    return RedirectToAction("Details", new { id = order.Id });
                }
                else if (buttonPressed == ButtonType.SubmitandEdit)
                {
                    return RedirectToAction("Details", new { id = order.Id });
                }
                else
                {
                    return RedirectToAction("Index", new { status = "edit-done" });
                }
            }
            return View(order);
        }

        // DELETE Language
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                Order order = db.Orders.Find(id);
                db.Orders.Remove(order);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }
        #endregion

        #region Order Status
        public ActionResult Status(string status)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            foreach (var item in roles)
                if (roles.FirstOrDefault() != null)
                {
                    {
                        var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                        var roleaccess = db.Roles_Access.Find(roleid);
                        if (roleaccess.Admin == false || roleaccess.PixelAdmin == false)
                        {
                            return RedirectToAction("Error", "Dashboard");
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            #endregion
            ViewBag.Status = status;
            return View(db.Order_Status.OrderBy(x => x.Order).ToList());
        }

        public ActionResult CreateStatus()
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.PixelAdmin == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
            #endregion
            return View();
        }
        [HttpPost]
        public ActionResult CreateStatus(Order_Status stt, FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                db.Order_Status.Add(stt);
                var langs = db.Languages.OrderBy(x => x.Order).ToList();
                foreach (var item in langs)
                {
                    if (fm["name-" + item.Code] != null)
                    {
                        Order_Item_Language sttname = new Order_Item_Language();
                        sttname.Status_Id = stt.Id;
                        sttname.Lang = item.Code;
                        sttname.Name = fm["name-" + item.Code];
                        db.Order_Item_Languages.Add(sttname);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Status", new { status = "create-done" });
            }
            return View(stt);
        }

        public ActionResult EditStatus(int id = 0)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.PixelAdmin == false || roleaccess.Order_View == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
            #endregion
            Order_Status stt = db.Order_Status.Find(id);
            if (stt == null)
            {
                return HttpNotFound();
            }
            return View(stt);
        }

        [HttpPost]
        public ActionResult EditStatus(Order_Status stt, FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stt).State = EntityState.Modified;
                var langs = db.Languages.OrderBy(x => x.Order).ToList();
                foreach (var item in langs)
                {
                    if (fm["name-" + item.Code] != null)
                    {
                        var oldname = db.Order_Item_Languages.FirstOrDefault(x => x.Status_Id == stt.Id && x.Lang == item.Code);
                        if (oldname.Name != fm["name-" + item.Code])
                        {
                            oldname.Name = fm["name-" + item.Code];
                        }
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Status", new { status = "edit-done" });
            }
            return View(stt);
        }

        // DELETE Group ATTRIBUTE
        [HttpPost]
        public ActionResult DeleteStatus(int id)
        {
            try
            {
                Order_Status stt = db.Order_Status.Find(id);
                db.Order_Status.Remove(stt);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        #endregion

        /// <summary>
        /// Views the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/29/2014 10:57 AM </created>
        public ActionResult view(int id)
        {
            var view = db.Orders.Find(id);
            if (view.View == false)
            {
                view.View = true;
            }
            else
            {
                view.View = false;
            }
            db.SaveChanges();
            return Json(new { succsess = true });
        }
    }
}
