using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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
    // [Auth(View = (int)ViewEnum.ReportManagement, Role = (int)RoleEnum.Edit)]
    public class ExportReportController : Controller
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        List<SelectListItem> items = new List<SelectListItem>();
        List<SelectListItem> roleslist = new List<SelectListItem>();
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Index()
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
            //new
            var type = db.PostTypes.FirstOrDefault(x => x.Code == "product");
            int typeid = type != null ? type.Id : 12;
            var list = db.Cats.Where(c => c.Owner_Id == null && c.Type == typeid).OrderByDescending(x => x.Order).ToList();
            foreach (var item in list)
            {
                string bullet = "---";

                items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                bullet += "---";
                Drrdown(item.Id, bullet, null);
            }

            ViewBag.cat = new SelectList(items, "Value", "Text");

            var rolesl = db.webpages_Roles.OrderBy(x => x.RoleId).ToList();

            foreach (var item in rolesl.Where(x => x.RoleName != "tkmadmin" && x.RoleName != "admin"))
            {
                roleslist.Add(new SelectListItem { Text = item.RoleName, Value = item.RoleName });
            }
            ViewBag.RolesList = roleslist;

            return View();
        }
        #region Load category
        public void Drrdown(Int32 catid, string bullet, int? id)
        {

            var listchild = db.Cats.Where(x => x.Owner_Id == catid).OrderByDescending(x => x.Order).ToList();
            if (listchild.Count > 0)
            {
                foreach (var item in listchild.Where(x => id != null ? (x.Id != id) : true))
                {
                    string bullet1 = "---";

                    items.Add(new SelectListItem { Text = bullet + item.Name, Value = item.Id.ToString() });

                    Drrdown(item.Id, bullet + bullet1, id);

                }

            }
        }

        #endregion
        [HttpPost]
        [OutputCache(Duration = 10, Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Export(String getexporttype, String getexporttype1, String getproducttype, String getsalername, String getdatetype, String gettungay, String getdenngay, String getcuahang)
        {
            int order_statusId = 4;//hoàn thành
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Bao cao");
            String filename = null;
            DateTime[] dates = getexportdates(getdatetype);

            #region tổng thu nhập new xuất theo table order
            if (getexporttype == "totalrevenue")
            {
                pck.Workbook.Worksheets.Delete("Bao cao");
                filename = "baocaotongdoanhthu.xlsx";
                ws = pck.Workbook.Worksheets.Add("Bao cao tong doanh thu ");
                SetTemplate(ws);
                decimal sum = 0;
                List<Order> orders = null;
                //ngay chi dinh
                if (dates == null)
                {

                    DateTime tungay = DateTime.ParseExact(gettungay, "dd/MM/yyyy", null);
                    DateTime denngay = DateTime.ParseExact(getdenngay, "dd/MM/yyyy", null);
                    orders = db.Orders.Where(x => x.Date_Add >= tungay && x.Date_Add <= denngay && x.Status_Id == order_statusId).ToList();

                    if (orders.Count == 0)
                    {
                        TempData["mess"] = " không có thu nhập...!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ws.Cells["A1"].Value = "BÁO CÁO TỔNG DOANH THU NGÀY " + string.Format("{0:dd/MM/yyy}", tungay) + " ĐẾN NGÀY " + string.Format("{0:dd/MM/yyy}", denngay);

                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["A1"].Style.Font.Size = 20;
                        ws.Cells["A2"].Value = "Mã số đơn hàng";
                        ws.Cells["B2"].Value = "Ngày đặt hàng";
                        ws.Cells["C2"].Value = "Mã số giao dịch";
                        ws.Cells["D2"].Value = "Mã số khách hàng";
                        ws.Cells["E2"].Value = "Mã số nhân viên";
                        ws.Cells["F2"].Value = "Tổng số lượng hàng";
                        ws.Cells["G2"].Value = "Tổng tiền";
                        //ws.Cells["H2"].Value = "Ngày tạo";
                    }
                }
                else//hang ngay,thang,nam
                {
                    orders = db.Orders.AsEnumerable().Where(x => x.Date_Add >= dates[0] && x.Date_Add <= dates[1] && x.Status_Id == order_statusId).ToList();
                    if (orders.Count == 0)
                    {
                        TempData["mess"] = " không có thu nhập...!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ws.Cells["A1"].Value = "BÁO CÁO TỔNG DOANH THU NGÀY " + string.Format("{0:dd/MM/yyy}", (dates[0])) + " ĐẾN NGÀY " + string.Format("{0:dd/MM/yyy}", (dates[1]));

                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["A1"].Style.Font.Size = 20;
                        ws.Cells["A2"].Value = "Mã số đơn hàng";
                        ws.Cells["B2"].Value = "Ngày đặt hàng";
                        ws.Cells["C2"].Value = "Tài khoản";
                        ws.Cells["D2"].Value = "Tên khách hàng";
                        ws.Cells["E2"].Value = "Tình trạng";
                        ws.Cells["F2"].Value = "Tổng số lượng hàng";
                        ws.Cells["G2"].Value = "Tổng tiền";
                        // ws.Cells["H2"].Value = "Ngày tạo";
                    }
                }
                for (int i = 0; i < orders.Count; i++)
                {
                    int j = i + 3;
                    //set border cho tất cả các ô từ dòng thứ 3
                    for (int k = 1; k < 8; k++)
                    {
                        ws.Cells[j, k].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                    ws.Cells["A" + j].Value = orders.ElementAt(i).Order_Code;
                    ws.Cells["B" + j].Value = string.Format("{0:dd/MM/yyy}", orders.ElementAt(i).Date_Add);
                    ws.Cells["C" + j].Value = orders.ElementAt(i).Username;
                    ws.Cells["D" + j].Value = orders.ElementAt(i).Name;
                    ws.Cells["E" + j].Value = orders.ElementAt(i).StatusText;
                    //đếm sl hàng trong orderdetail
                    int oid = orders.ElementAt(i).Id;
                    int odt = db.Order_Details.Where(x => x.Order_Id == oid).Count();
                    ws.Cells["F" + j].Value = odt;

                    ws.Cells["G" + j].Value = orders.ElementAt(i).Total;
                    //ws.Cells["H" + j].Value = orders.ElementAt(i).Date_Add.ToShortDateString();
                    sum = sum + orders.ElementAt(i).Total ?? 0;
                    int totalrow = orders.Count + 3;
                    ws.Cells["A" + totalrow].Value = "Tổng thu nhập";
                    ws.Cells["A" + totalrow].Style.Font.Bold = true;
                    ws.Cells["B" + totalrow].Value = string.Format("{0:0,0 vnd}", sum);

                }

            }
            #endregion

            #region doanh thu saler
            if (getexporttype == "doanhthusaler")
            {
                pck.Workbook.Worksheets.Delete("Bao cao");
                filename = "doanhthusaler-" + getsalername + ".xlsx";
                ws = pck.Workbook.Worksheets.Add("Bao cao tong doanh thu nhan vien ");
                SetTemplate(ws);
                decimal sum = 0;
                List<Order> orders = null;
                if (dates == null)
                {

                    DateTime tungay = DateTime.ParseExact(gettungay, "dd/MM/yyyy", null);
                    DateTime denngay = DateTime.ParseExact(getdenngay, "dd/MM/yyyy", null);
                    orders = db.Orders.Where(x => x.Date_Add >= tungay && x.Date_Add <= denngay && x.Status_Id == order_statusId && x.Username == getsalername).ToList();
                    if (orders.Count == 0)
                    {
                        TempData["mess"] = "NVKD " + getsalername + " không có danh thu...!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ws.Cells["A1"].Value = "BÁO CÁO DOANH THU NHÂN VIÊN " + getsalername + " NGÀY " + string.Format("{0:dd/MM/yyy}", tungay) + " ĐẾN NGÀY " + string.Format("{0:dd/MM/yyy}", denngay);

                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["A1"].Style.Font.Size = 20;
                        ws.Cells["A2"].Value = "Mã số đơn hàng";
                        ws.Cells["B2"].Value = "Ngày đặt hàng";
                        ws.Cells["C2"].Value = "Tên tài khoản";
                        ws.Cells["D2"].Value = "Tên khách hàng";
                        ws.Cells["E2"].Value = "Tổng số lượng hàng";
                        ws.Cells["F2"].Value = "Tình trạng";
                        ws.Cells["G2"].Value = "Tổng tiền";

                    }
                }
                else
                {
                    //kiểm tra saler có tồn tại hay không
                    var u = new UsersContext();
                    var username = u.UserProfiles.FirstOrDefault(x => x.UserName == getsalername);
                    if (username != null)
                    {
                        orders = db.Orders.AsEnumerable().Where(x => x.Date_Add >= dates[0] && x.Date_Add <= dates[1] && x.Status_Id == order_statusId && x.Username == getsalername).ToList();
                        if (orders.Count == 0)
                        {
                            TempData["mess"] = "NVKD " + getsalername + " không có danh thu...!";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ws.Cells["A1"].Value = "BÁO CÁO DOANH THU NHÂN VIÊN " + getsalername + " NGÀY " + string.Format("{0:dd/MM/yyy}", dates[0]) + " ĐẾN NGÀY " + string.Format("{0:dd/MM/yyy}", dates[1]);

                            ws.Cells["A1"].Style.Font.Bold = true;
                            ws.Cells["A1"].Style.Font.Size = 20;
                            ws.Cells["A2"].Value = "Mã số đơn hàng";
                            ws.Cells["B2"].Value = "Ngày đặt hàng";
                            ws.Cells["C2"].Value = "Tên tài khoản";
                            ws.Cells["D2"].Value = "Tên khách hàng";
                            ws.Cells["E2"].Value = "Tổng số lượng hàng";
                            ws.Cells["F2"].Value = "Tình trạng";
                            ws.Cells["G2"].Value = "Tổng tiền";
                        }
                    }
                    else
                    {
                        TempData["mess"] = "NVKD " + getsalername + " không tồn tại !";
                        return RedirectToAction("Index");
                    }

                }
                for (int i = 0; i < orders.Count; i++)
                {
                    int j = i + 3;
                    //set border cho tất cả các ô từ dòng thứ 3
                    for (int k = 1; k < 8; k++)
                    {
                        ws.Cells[j, k].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                    ws.Cells["A" + j].Value = orders.ElementAt(i).Id;
                    ws.Cells["B" + j].Value = string.Format("{0:dd/MM/yyy}", orders.ElementAt(i).Date_Add);
                    ws.Cells["C" + j].Value = orders.ElementAt(i).Username;
                    ws.Cells["D" + j].Value = orders.ElementAt(i).Name;
                    ws.Cells["E" + j].Value = orders[i].Order_Details.Sum(x => x.Quantity);
                    ws.Cells["F" + j].Value = orders.ElementAt(i).StatusText;
                    ws.Cells["G" + j].Value = orders.ElementAt(i).Total;
                    //  ws.Cells["H" + j].Value = orders.ElementAt(i).Date_Add.ToShortDateString();
                    sum = sum + orders.ElementAt(i).Total ?? 0;
                    int totalrow = orders.Count + 3;
                    ws.Cells["A" + totalrow].Value = "Tổng thu nhập";
                    ws.Cells["A" + totalrow].Style.Font.Bold = true;
                    ws.Cells["B" + totalrow].Value = string.Format("{0:0,0 vnd}", sum);

                }

            }
            #endregion

            #region Mặt hàng
            if (getexporttype == "mathang")
            {
                List<Order_Details> orderdetail = null;
                decimal sum = 0;
                var CatId = int.Parse(getproducttype);
                var cat = db.Cats.FirstOrDefault(x => x.Id == CatId);
                filename = "doanhthumathang-" + cat.Name + ".xlsx";
                ws = pck.Workbook.Worksheets.Add("doanh thu mat hang " + cat.Name);
                SetTemplate(ws);
                if (dates == null)
                {
                    DateTime tungay = DateTime.ParseExact(gettungay, "dd/MM/yyyy", null);
                    DateTime denngay = DateTime.ParseExact(getdenngay, "dd/MM/yyyy", null);
                    orderdetail =
                       db.Order_Details.Where(
                           x =>
                           x.Order.Date_Add >= tungay && x.Order.Date_Add <= denngay &&
                           x.Order.Status_Id == order_statusId && x.Post.Cats.FirstOrDefault(z => z.Id == CatId) != null).ToList();
                    if (orderdetail.Count == 0)
                    {
                        TempData["mess"] = " không có danh thu mặt hàng " + cat.Name;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ws.Cells["A1"].Value = "BÁO CÁO DOANH THU MẶT HÀNG " + cat.Name.ToUpper() + " NGÀY " + string.Format("{0:dd/MM/yyy}", tungay) + " ĐẾN NGÀY " + string.Format("{0:dd/MM/yyy}", denngay);

                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["A1"].Style.Font.Size = 20;
                        ws.Cells["A2"].Value = "Mã số đơn hàng";
                        ws.Cells["B2"].Value = "Tên sản phẩm";
                        ws.Cells["C2"].Value = "Thuộc tính";
                        ws.Cells["D2"].Value = "Mã hàng(SKU)";
                        ws.Cells["E2"].Value = "Giá";
                        ws.Cells["F2"].Value = "Số lượng";
                        ws.Cells["G2"].Value = "Thành tiền";
                        ws.Cells["H2"].Value = "Ngày tạo";
                    }
                }
                else
                {
                    orderdetail =
                     db.Order_Details.AsEnumerable().Where(
                         x =>
                         x.Order.Date_Add >= dates[0] && x.Order.Date_Add <= dates[1] &&
                         x.Order.Status_Id == order_statusId && x.Post.Cats.FirstOrDefault(z => z.Id == CatId) != null).ToList();
                    if (orderdetail.Count == 0)
                    {
                        TempData["mess"] = " không có danh thu mặt hàng " + cat.Name;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ws.Cells["A1"].Value = "BÁO CÁO DOANH THU MẶT HÀNG " + cat.Name.ToUpper() + " NGÀY " + string.Format("{0:dd/MM/yyy}", dates[0]) + " ĐẾN NGÀY " + string.Format("{0:dd/MM/yyy}", dates[1]);

                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["A1"].Style.Font.Size = 20;
                        ws.Cells["A2"].Value = "Mã số đơn hàng";
                        ws.Cells["B2"].Value = "Tên sản phẩm";
                        ws.Cells["C2"].Value = "Thuộc tính";
                        ws.Cells["D2"].Value = "Mã hàng(SKU)";
                        ws.Cells["E2"].Value = "Giá";
                        ws.Cells["F2"].Value = "Số lượng";
                        ws.Cells["G2"].Value = "Thành tiền";
                        ws.Cells["H2"].Value = "Ngày tạo";

                    }
                }
                for (int i = 0; i < orderdetail.Count; i++)
                {
                    int j = i + 3;
                    //set border cho tất cả các ô từ dòng thứ 3
                    for (int k = 1; k < 9; k++)
                    {
                        ws.Cells[j, k].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    ws.Cells["A" + j].Value = orderdetail[i].Order_Id;
                    ws.Cells["B" + j].Value = orderdetail.ElementAt(i).Post.Title;
                    ws.Cells["C" + j].Value = orderdetail.ElementAt(i).Attribute != null ? orderdetail.ElementAt(i).Attribute.Replace("<br/>", ",") : null;
                    ws.Cells["D" + j].Value = orderdetail[i].SKU;
                    ws.Cells["E" + j].Value = orderdetail.ElementAt(i).Unit_Price;
                    ws.Cells["F" + j].Value = orderdetail.ElementAt(i).Quantity;
                    ws.Cells["G" + j].Value = orderdetail.ElementAt(i).Unit_Price * orderdetail.ElementAt(i).Quantity;
                    ws.Cells["H" + j].Value = string.Format("{0:dd/MM/yyy}", orderdetail.ElementAt(i).Order.Date_Add);
                    sum = sum + (orderdetail.ElementAt(i).Unit_Price * orderdetail.ElementAt(i).Quantity) ?? 0;
                }
                int totalrow = orderdetail.Count + 3;
                ws.Cells["A" + totalrow].Value = "Tổng thu nhập";
                ws.Cells["A" + totalrow].Style.Font.Bold = true;
                ws.Cells["B" + totalrow].Value = string.Format("{0:0,0 vnd}", sum);
            }
            #endregion

            #region Hàng tồn
            if (getexporttype == "baocaohangton")
            {
                filename = "baocaohangton.xlsx";
                pck.Workbook.Worksheets.Delete("Bao cao");
                ws = pck.Workbook.Worksheets.Add("Bao cao hang ton");
                SetTemplate(ws);
                //lấy danh sách sp tồn kho
                var listinven = new List<InventoryModel>();
                foreach (var item in db.Inventories)
                {
                    var post = item.Product.Posts.FirstOrDefault();
                    if (post.VariantAttributeCombinations.Count > 0)//xet biến thể
                    {
                        var variant = post.VariantAttributeCombinations.Where(x => x.Quantity > item.Product.Inv_min_Quantity).OrderByDescending(x => x.Quantity);
                        foreach (var vari in variant)
                        {
                            var inven = new InventoryModel();
                            string attribute = "";
                            if (vari.Attribute != null)
                            {
                                var att = vari.Attribute.Split(',');
                                foreach (var s in att)
                                {
                                    int id = int.Parse(s);
                                    var varopt = db.OptionVariants.FirstOrDefault(x => x.Id == id);

                                    attribute = attribute + varopt.GroupVariant.Name + ":" + varopt.Name + ",";
                                }
                            }
                            inven.ProductName = post.Title;
                            inven.Attribute = attribute;
                            inven.Quantity = vari.Quantity;
                            inven.SKU = vari.SKU;
                            inven.Id = post.Id;
                            inven.Price = vari.Price > 0 ? vari.Price : (post.Price ?? 0);
                            listinven.Add(inven);
                        }
                    }//nl nếu ko có biến thể 
                    else
                    {
                        var inven = new InventoryModel();
                        inven.ProductName = post.Title;
                        inven.Quantity = item.Quantity;
                        inven.SKU = item.Product.SKU;
                        inven.Id = post.Id;
                        inven.Price = post.Price ?? 0;
                        listinven.Add(inven);
                    }

                }

                var products = listinven.OrderByDescending(x => x.Quantity).ToList();
                if (products.Count == 0)
                {
                    TempData["mess"] = " không có hàng tồn...!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ws.Cells["A1"].Value = "BÁO CÁO HÀNG TỒN KHO";

                    ws.Cells["A1"].Style.Font.Bold = true;
                    ws.Cells["A1"].Style.Font.Size = 20;
                    ws.Cells["A2"].Value = "Mã hàng(SKU)";
                    ws.Cells["B2"].Value = "Nhà cung cấp";
                    ws.Cells["C2"].Value = "Tên";
                    ws.Cells["D2"].Value = "Thuộc tính";
                    ws.Cells["E2"].Value = "Số lượng";
                    ws.Cells["F2"].Value = "Giá";
                    ws.Cells["G2"].Value = "Ngày tạo";

                    for (int i = 0; i < products.Count; i++)
                    {
                        int j = i + 3;
                        //set border cho tất cả các ô từ dòng thứ 3
                        for (int k = 1; k < 8; k++)
                        {
                            ws.Cells[j, k].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }

                        // var cat = db.Posts.Find(products[i].Id).Cats.FirstOrDefault();
                        var sup = db.Posts.Find(products[i].Id).Product.Manufacturer;
                        var Date_Create = db.Posts.Find(products[i].Id).Date_Create;

                        ws.Cells["A" + j].Value = products.ElementAt(i).SKU;
                        ws.Cells["B" + j].Value = sup != null ? sup.Name : null;
                        ws.Cells["C" + j].Value = products.ElementAt(i).ProductName;
                        ws.Cells["D" + j].Value = products[i].Attribute;
                        ws.Cells["E" + j].Value = products.ElementAt(i).Quantity;
                        ws.Cells["F" + j].Value = string.Format("{0:0,0 vnd}", products.ElementAt(i).Price);
                        ws.Cells["G" + j].Value = string.Format("{0:dd/MM/yyy}", Date_Create);

                    }
                }

            }
            #endregion

            #region doanh thu cửa hàng
            if (getexporttype == "cuahang")
            {
                pck.Workbook.Worksheets.Delete("Bao cao");
                filename = "doanhthucuahang-" + getcuahang + ".xlsx";
                ws = pck.Workbook.Worksheets.Add("Báo cáo doanh thu của cửa hàng ");
                SetTemplate(ws);
                decimal sum = 0;
                List<Order> orders = new List<Order>();
                if (dates == null)
                {



                    DateTime tungay = DateTime.ParseExact(gettungay, "dd/MM/yyyy", null);
                    DateTime denngay = DateTime.ParseExact(getdenngay, "dd/MM/yyyy", null);


                    var uInR = Roles.GetUsersInRole(getcuahang).ToList();
                    foreach (var item in uInR)
                    {

                        var y = db.Orders.Where(x => x.Date_Add >= tungay && x.Date_Add <= denngay && x.Status_Id == order_statusId && x.Username == item).ToList();
                        orders.AddRange(y);
                    }

                    if (orders.Count == 0)
                    {
                        TempData["mess"] = getcuahang + " không có danh thu...!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ws.Cells["A1"].Value = "BÁO CÁO DOANH THU  " + getcuahang + " NGÀY " + string.Format("{0:dd/MM/yyy}", tungay) + " ĐẾN NGÀY " + string.Format("{0:dd/MM/yyy}", denngay);

                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["A1"].Style.Font.Size = 20;
                        ws.Cells["A2"].Value = "Mã số đơn hàng";
                        ws.Cells["B2"].Value = "Ngày đặt hàng";
                        ws.Cells["C2"].Value = "Tên tài khoản";
                        ws.Cells["D2"].Value = "Tên khách hàng";
                        ws.Cells["E2"].Value = "Tổng số lượng hàng";
                        ws.Cells["F2"].Value = "Tình trạng";
                        ws.Cells["G2"].Value = "Tổng tiền";

                    }
                }
                else
                {
                    var uInR = Roles.GetUsersInRole(getcuahang).ToList();
                    foreach (var item in uInR)
                    {
                        var y = db.Orders.AsEnumerable().Where(x => x.Date_Add >= dates[0] && x.Date_Add <= dates[1] && x.Status_Id == order_statusId && x.Username == item).ToList();
                        orders.AddRange(y);
                    }
                    if (orders.Count == 0)
                    {
                        TempData["mess"] = getcuahang + " không có danh thu...!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ws.Cells["A1"].Value = "BÁO CÁO DOANH THU" + getcuahang + " NGÀY " + string.Format("{0:dd/MM/yyy}", dates[0]) + " ĐẾN NGÀY " + string.Format("{0:dd/MM/yyy}", dates[1]);

                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["A1"].Style.Font.Size = 20;
                        ws.Cells["A2"].Value = "Mã số đơn hàng";
                        ws.Cells["B2"].Value = "Ngày đặt hàng";
                        ws.Cells["C2"].Value = "Tên tài khoản";
                        ws.Cells["D2"].Value = "Tên khách hàng";
                        ws.Cells["E2"].Value = "Tổng số lượng hàng";
                        ws.Cells["F2"].Value = "Tình trạng";
                        ws.Cells["G2"].Value = "Tổng tiền";
                    }


                }
                for (int i = 0; i < orders.Count; i++)
                {
                    int j = i + 3;
                    //set border cho tất cả các ô từ dòng thứ 3
                    for (int k = 1; k < 8; k++)
                    {
                        ws.Cells[j, k].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                    ws.Cells["A" + j].Value = orders.ElementAt(i).Order_Code;
                    ws.Cells["B" + j].Value = string.Format("{0:dd/MM/yyy}", orders.ElementAt(i).Date_Add);
                    ws.Cells["C" + j].Value = orders.ElementAt(i).Username;
                    ws.Cells["D" + j].Value = orders.ElementAt(i).Name;
                    ws.Cells["E" + j].Value = null;
                    ws.Cells["F" + j].Value = orders.ElementAt(i).StatusText;
                    ws.Cells["G" + j].Value = orders.ElementAt(i).Total;
                    //  ws.Cells["H" + j].Value = orders.ElementAt(i).Date_Add.ToShortDateString();
                    sum = sum + orders.ElementAt(i).Total ?? 0;
                    int totalrow = orders.Count + 3;
                    ws.Cells["A" + totalrow].Value = "Tổng thu nhập";
                    ws.Cells["A" + totalrow].Style.Font.Bold = true;
                    ws.Cells["B" + totalrow].Value = string.Format("{0:0,0 vnd}", sum);

                }

            }

            #endregion

            #region Hàng nhập
            /*if (getexporttype == "baocaohangnhap")
            {
                filename = "baocaohangnhap.xlsx";
                pck.Workbook.Worksheets.Delete("Bao cao");
                ws = pck.Workbook.Worksheets.Add("Bao cao hang nhap");
                SetTemplate(ws);
                decimal sum = 0;
                List<Product> products = null;
                if (dates == null)
                {
                    DateTime tungay = DateTime.ParseExact(gettungay, "dd/MM/yyyy", null);
                    DateTime denngay = DateTime.ParseExact(getdenngay, "dd/MM/yyyy", null);
                    products = productRepository.GetProductbyImportDay1(tungay, denngay);
                    if (products.Count == 0)
                    {
                        TempData["mess"] = " không có hàng nhập...!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ws.Cells["A1"].Value = "BÁO CÁO HÀNG Nhập " + string.Format("{0:dd/MM/yyy}", tungay) + " ĐẾN NGÀY " + string.Format("{0:dd/MM/yyy}", denngay);

                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["A1"].Style.Font.Size = 20;
                        ws.Cells["A1"].Style.Font.Color.SetColor(Color.Red);
                        ws.Cells["A2"].Value = "Mã số hàng";
                        ws.Cells["B2"].Value = "Loại hàng";
                        ws.Cells["C2"].Value = "Nhà cung cấp";
                        ws.Cells["D2"].Value = "Số lượng";
                        ws.Cells["E2"].Value = "Tên";
                        ws.Cells["F2"].Value = "Giá nhập";
                        ws.Cells["G2"].Value = "Ngày nhập";
                        ws.Cells["H2"].Value = "Ngày cập nhật";

                        for (int i = 0; i < products.Count; i++)
                        {
                            var idloaihang = products.ElementAt(i).CategoryId;
                            var idnhacungcap = products.ElementAt(i).SupplierId;
                            var catname = db.Categories.FirstOrDefault(x => x.Id == idloaihang);
                            var sup = db.Suppliers.FirstOrDefault(x => x.Id == idnhacungcap);
                            int j = i + 3;
                            ws.Cells["A" + j].Value = products.ElementAt(i).Code;
                            ws.Cells["A" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["B" + j].Value = products.ElementAt(i).ProductType == 2 ? "Sim" : catname.Name;
                            ws.Cells["B" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["C" + j].Value = sup.Name;
                            // ws.Cells["C" + j].Value = products.ElementAt(i).BrandId.HasValue && brands.ContainsKey(products.ElementAt(i).BrandId.Value) ? brands[products.ElementAt(i).BrandId.Value] : "";
                            ws.Cells["C" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["D" + j].Value = products.ElementAt(i).Quantity;
                            ws.Cells["D" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["E" + j].Value = products.ElementAt(i).Name;
                            ws.Cells["E" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["F" + j].Value = string.Format("{0:0,0 vnd}", products.ElementAt(i).ImportPrice);
                            ws.Cells["F" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["G" + j].Value = products.ElementAt(i).CreateTime.ToShortDateString();
                            ws.Cells["G" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["H" + j].Value = products.ElementAt(i).UpdateTime.ToString();
                            ws.Cells["H" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            sum = sum + products.ElementAt(i).ImportPrice;
                            int totalrow = products.Count + 3;
                            ws.Cells["A" + totalrow].Value = "Tổng tiền hàng nhập";
                            ws.Cells["A" + totalrow].Style.Font.Bold = true;
                            ws.Cells["A" + totalrow].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            //ws.Cells["A" + totalrow].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["A" + totalrow].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            ws.Cells["B" + totalrow].Value = string.Format("{0:0,0 vnd}", sum);

                        }
                    }
                }
                else
                {
                    products = productRepository.GetProductbyImportDay1(dates[0], dates[1]);
                    if (products.Count == 0)
                    {
                        TempData["mess"] = " không có hàng nhập...!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ws.Cells["A1"].Value = "BÁO CÁO HÀNG NHẬP " + string.Format("{0:dd/MM/yyy}", dates[0]) + " ĐẾN NGÀY " + string.Format("{0:dd/MM/yyy}", dates[1]);

                        ws.Cells["A1"].Style.Font.Bold = true;
                        ws.Cells["A1"].Style.Font.Size = 20;
                        ws.Cells["A1"].Style.Font.Color.SetColor(Color.Red);
                        ws.Cells["A2"].Value = "Mã số hàng";
                        ws.Cells["B2"].Value = "Loại hàng";
                        ws.Cells["C2"].Value = "Nhà cung cấp";
                        ws.Cells["D2"].Value = "Số lượng";
                        ws.Cells["E2"].Value = "Tên";
                        ws.Cells["F2"].Value = "Giá nhập";
                        ws.Cells["G2"].Value = "Ngày nhập";
                        ws.Cells["H2"].Value = "Ngày cập nhật";

                        for (int i = 0; i < products.Count; i++)
                        {
                            var idloaihang = products.ElementAt(i).CategoryId;
                            var idnhacungcap = products.ElementAt(i).SupplierId;
                            var catname = db.Categories.FirstOrDefault(x => x.Id == idloaihang);
                            var sup = db.Suppliers.FirstOrDefault(x => x.Id == idnhacungcap);
                            int j = i + 3;
                            ws.Cells["A" + j].Value = products.ElementAt(i).Code;
                            ws.Cells["A" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["B" + j].Value = products.ElementAt(i).ProductType == 2 ? "Sim" : catname.Name;
                            ws.Cells["B" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["C" + j].Value = sup.Name;
                            // ws.Cells["C" + j].Value = products.ElementAt(i).BrandId.HasValue && brands.ContainsKey(products.ElementAt(i).BrandId.Value) ? brands[products.ElementAt(i).BrandId.Value] : "";
                            ws.Cells["C" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["D" + j].Value = products.ElementAt(i).Quantity;
                            ws.Cells["D" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["E" + j].Value = products.ElementAt(i).Name;
                            ws.Cells["E" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["F" + j].Value = string.Format("{0:0,0 vnd}", products.ElementAt(i).ImportPrice);
                            ws.Cells["F" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["G" + j].Value = products.ElementAt(i).CreateTime.ToShortDateString();
                            ws.Cells["G" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["H" + j].Value = products.ElementAt(i).UpdateTime.ToString();
                            ws.Cells["H" + j].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            sum = sum + products.ElementAt(i).ImportPrice;
                            int totalrow = products.Count + 3;
                            ws.Cells["A" + totalrow].Value = "Tổng tiền hàng nhập";
                            ws.Cells["A" + totalrow].Style.Font.Bold = true;
                            ws.Cells["A" + totalrow].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            //ws.Cells["A" + totalrow].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            ws.Cells["A" + totalrow].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            ws.Cells["B" + totalrow].Value = string.Format("{0:0,0 vnd}", sum);

                        }
                    }
                }

            }*/
            #endregion

            byte[] data = pck.GetAsByteArray();
            return File(data, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        public DateTime[] getexportdates(String getdatetype)
        {
            DateTime[] dates = null;
            if (getdatetype == "hangngay")
            {
                dates = new DateTime[2];
                DateTime today = DateTime.Today;
                DateTime firsthourofday = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
                dates[0] = firsthourofday;
                dates[1] = DateTime.Now;
            }
            if (getdatetype == "hangtuan")
            {
                dates = new DateTime[2];
                DateTime now = DateTime.Now;
                int delta = DayOfWeek.Monday - now.DayOfWeek;
                DateTime monday = now.AddDays(delta);
                dates[0] = monday;
                dates[1] = now;
            }

            if (getdatetype == "hangthang")
            {
                dates = new DateTime[2];
                DateTime today = DateTime.Today;
                DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
                dates[0] = startOfMonth;
                dates[1] = DateTime.Now;
            }
            if (getdatetype == "hangnam")
            {
                dates = new DateTime[2];
                DateTime now = DateTime.Now;
                int year = DateTime.Now.Year;
                DateTime firstDayofYear = new DateTime(year, 1, 1);
                dates[0] = firstDayofYear;
                dates[1] = now;

            }
            return dates;
        }
        void SetTemplate(ExcelWorksheet ws)
        {
            for (int i = 1; i < 8; i++)
            {
                ws.Column(i).Width = 17;
                ws.Cells[2, i].Style.Font.Bold = true;
                ws.Cells[2, i].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            }
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 2/12/2014 10:08 PM </created>
       public JsonResult getUserName(string query = "")
        {
            var u = new UsersContext();
            var uprofile = u.UserProfiles.ToList();
            var rolesl = db.webpages_Roles.OrderBy(x => x.RoleId).ToList();

            List<UserProfile> up = new List<UserProfile>();

            foreach (var item in rolesl.Where(x => x.RoleName != "tkmadmin" && x.RoleName != "member"))//nhóm thành viên user đăng ký
            {
                foreach (var child in item.webpages_UsersInRoles)
                {
                    foreach (var i in uprofile.Where(x => x.UserId == child.UserId).ToList())
                    {
                        up.Add(i);
                    }
                }
            }
            var list = String.IsNullOrEmpty(query) ? up.ToList() :
                 up.Where(p => p.UserName.Contains(query)).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

    }
}
