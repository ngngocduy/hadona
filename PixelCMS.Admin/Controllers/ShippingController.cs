using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PagedList;
using PixelCMS.Core.Models;

namespace PixelCMS.Areas.Admin.Controllers
{
    public class ShippingController : Controller
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        //
        // GET: /Shipping/
        public ActionResult Index(int? page)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    if (db.Roles_Access.Find(roleid).Admin == false)
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
            var shippinglist = db.Shippings.ToList();

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(shippinglist.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    if (db.Roles_Access.Find(roleid).Admin == false)
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
            var model = new ShippingModel();

            //drop City
            model.CityList.Add(new SelectListItem() { Text = "Chọn Tỉnh/TP", Value = "0" });
            var Citys = db.Cities.ToList();
            foreach (var c in Citys)
                model.CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            ViewBag.CityList = new SelectList(model.CityList, "Value", "Text");

            //drop District
            model.DistrictList.Add(new SelectListItem() { Text = "Chọn Quận/Huyện", Value = "0" });
            ViewBag.DistrictList = new SelectList(model.DistrictList, "Value", "Text");

            return View(model);
        }
        [HttpPost]
        public ActionResult Create(ShippingModel model)
        {
            // if (ModelState.IsValid)
            //{
            var shipping = new Shipping();
            shipping.FixedFee = model.FixedFee;
            shipping.MinWeight = model.MinWeight;
            shipping.MaxWeight = model.MaxWeight;
            shipping.FeeOfUnit = model.FeeOfUnit;
            shipping.CreateDate = DateTime.Now;
            if (model.selectedItem!=null)
            {
                var listDistrict = model.selectedItem.Split(',');
                for (int i = 0; i < listDistrict.Length; i++)
                {
                    shipping.District_Id = int.Parse(listDistrict[i]);
                    db.Shippings.Add(shipping);
                    db.SaveChanges();
                }  
            }
            // }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    if (db.Roles_Access.Find(roleid).Admin == false)
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

            var ship = db.Shippings.Find(id);
            var model = new ShippingModel();
            model.FixedFee = ship.FixedFee??0;
            model.MinWeight = ship.MinWeight??0;
            model.MaxWeight = ship.MaxWeight??0;
            model.FeeOfUnit = ship.FeeOfUnit??0;

            //drop City
            model.CityList.Add(new SelectListItem() { Text = "Chọn Tỉnh/TP", Value = "0" });
            var Citys = db.Cities.ToList();
            foreach (var c in Citys)
                model.CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });

            //drop District
            model.DistrictList.Add(new SelectListItem() { Text = "Chọn Quận/Huyện", Value = "0" });

            if (ship.District_Id != null)
            {
                var district = db.Districts.FirstOrDefault(z => z.Id == ship.District_Id);
                //drop City
                ViewBag.CityList = new SelectList(model.CityList, "Value", "Text", district != null ? district.City.Id : 0);

                //drop District
                var Districts = db.Districts.Where(z => z.City_Id == district.City.Id).ToList();
                foreach (var d in Districts)
                    model.DistrictList.Add(new SelectListItem() { Text = d.Name, Value = d.Id.ToString() });
                ViewBag.DistrictList = new SelectList(model.DistrictList, "Value", "Text", ship.District_Id);
            }
            else
            {
                //drop City
                ViewBag.CityList = new SelectList(model.CityList, "Value", "Text");

                //drop District
                ViewBag.DistrictList = new SelectList(model.DistrictList, "Value", "Text");
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(ShippingModel model)
        {
            var shipping = db.Shippings.Find(model.Id);
            shipping.District_Id = model.DistrictId != 0 ? model.DistrictId : null;
            shipping.FixedFee = model.FixedFee;
            shipping.MinWeight = model.MinWeight;
            shipping.MaxWeight = model.MaxWeight;
            shipping.FeeOfUnit = model.FeeOfUnit;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
       /* [HttpPost]
        public ActionResult Delete(int id)
        {
            var ship = db.Shippings.Find(id);
            db.Shippings.Remove(ship);
            db.SaveChanges();
            return Content(Boolean.TrueString);
        }
*/
        // DELETE Email
        [HttpPost]
        public ActionResult Delete(int id)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    if (db.Roles_Access.Find(roleid).Admin == false)
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

            try
            {
                var ship = db.Shippings.Find(id);
                db.Shippings.Remove(ship);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

    }
}
