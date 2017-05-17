using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using PixelCMS.Core.Models;

namespace PixelCMS.Areas.Admin.Controllers
{
    public class InventoryController : Controller
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        List<SelectListItem> Listagent = new List<SelectListItem>();

        /// <summary>
        /// Indexes the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/11/2014 5:57 PM </created>
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

            // var xlPackage = new ExcelPackage();

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var model = db.Inventories.ToList();//Where(x => x.Quantity < x.Product.Inv_min_Quantity).ToList();
            return View(model.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Stocks the import.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/11/2014 5:57 PM </created>
        public ActionResult StockImport(int? page)
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

            var agent = db.Agents.Where(z => z.Active);
            foreach (var item in agent)
                Listagent.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });

            ViewBag.agent = Listagent;

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var model = db.StockImports.OrderByDescending(z => z.ImportDate).ToList();
            return View(model.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Adds the stock import.
        /// </summary>
        /// <param name="agent">The agent.</param>
        /// <param name="sku">The sku.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/12/2014 9:17 AM </created>
        public ActionResult AddStockImport(int agent, string sku, int quantity)
        {
            if (agent < 0 || string.IsNullOrEmpty(sku) || quantity == 0)
            {
                return Json(new { result = "Vui lòng điền đầy đủ thông tin (sku,số lượng)" });
            }
            //check trước xem sku có phải là của biến thể ko.
            var variantCB = db.VariantAttributeCombinations.FirstOrDefault(x => x.SKU == sku);
            //check sku product
            var inven = db.Inventories.FirstOrDefault(z => z.Product.SKU == sku && z.Agent_Id == agent);

            StockImport stock = new StockImport();
            stock.Quantity = quantity;
            stock.ImportDate = DateTime.Now;
            // nếu có biến thể add số lượng vào
            string attribute = "";
            string productname = "";
            if (variantCB != null)
            {
                //cắt lấy tên thuộc tính
                if (variantCB.Attribute != null)
                {
                    var att = variantCB.Attribute.Split(',');
                    foreach (var s in att)
                    {
                        int id1 = int.Parse(s);
                        var varopt = db.OptionVariants.FirstOrDefault(x => x.Id == id1);
                        attribute = attribute + varopt.GroupVariant.Name + ":" + varopt.Name + "<br/>";
                    }
                }
                //lấy tên sp
                productname = variantCB.Post.Title;

                stock.SKU = variantCB.SKU;
                stock.CurrentQuantity = variantCB.Quantity;//lưu lại số lượng củ
                stock.ProductId = variantCB.Post.Product.Id;
                //lưu số lượng mới vào kho biến thể
                variantCB.Quantity = variantCB.Quantity + quantity;
            }
            else//nl check kho của sp
                if (inven != null)
                {
                    stock.SKU = inven.Product.SKU;
                    stock.CurrentQuantity = inven.Quantity;//lưu lại số lượng củ
                    stock.ProductId = inven.Product_Id;
                    //lưu số lượng mới
                    inven.Quantity = inven.Quantity + quantity;
                    //gán tên sp
                    productname = inven.Product.Posts.FirstOrDefault().Title;
                }
                else
                {
                    //ko tìm thấy sku
                    return Json(new { succsess = false });
                }
            db.StockImports.Add(stock);
            db.SaveChanges();

            return Json(new
            {

                ProductName = productname,
                Sku = stock.SKU,
                Attribute = attribute,
                Quantity = stock.Quantity,
                CurrentQuantity = stock.CurrentQuantity,
                QuantityTotal = stock.Quantity + stock.CurrentQuantity,
                Date = stock.ImportDate.ToString()
            });
        }

        /// <summary>
        /// Stocks the import.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/11/2014 5:57 PM </created>
        public ActionResult Inventories(int? page)
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

            var listinven = new List<InventoryModel>();
            foreach (var item in db.Inventories)
            {
                var post = item.Product.Posts.FirstOrDefault();
                if (post!=null)
                {
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
                                    if (varopt != null)
                                    {
                                        attribute = attribute + varopt.GroupVariant.Name + ":" + varopt.Name + ",";
                                    }
                                }
                            }
                            inven.ProductName = post.Title;
                            inven.Attribute = attribute;
                            inven.Quantity = vari.Quantity;
                            inven.SKU = vari.SKU;
                            listinven.Add(inven);
                        }


                    }//nl nếu ko có biến thể 
                    else
                    {
                        var inven = new InventoryModel();
                        inven.ProductName = post.Title;
                        inven.Quantity = item.Quantity;
                        inven.SKU = item.Product.SKU;
                        listinven.Add(inven);
                    }  
                }
                

            }


            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(listinven.OrderByDescending(x => x.Quantity).ToPagedList(pageNumber, pageSize));
        }


        #region xuất exel

        /// <summary>
        /// Exports the inventory.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 11/21/2014 2:42 PM </created>
        [HttpPost]
        public ActionResult ExportInventory(DateTime? tungay, DateTime? denngay)
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Bao cao hang nhap");
            string filename = "baocaohangnhap.xlsx";
            SetTemplate(ws);
            //List<StockImport> stockImports = null;

            var stockImports = db.StockImports.AsEnumerable().Where(x =>
                (tungay == null ? true : (x.ImportDate.Date >= tungay.Value.Date))
             && (denngay == null ? true : (x.ImportDate.Date <= denngay.Value.Date))).OrderByDescending(x => x.ImportDate).ToList();
            if (stockImports.Count == 0)
            {
                TempData["mess"] = " không có sản phẩm nào...!";
                return RedirectToAction("StockImport");
            }
            else
            {
                ws.Cells["A1:G1"].Merge = true;
                ws.Cells["A1"].Value = "BÁO CÁO HÀNG NHẬP NGÀY " + string.Format("{0:dd/MM/yyy}", tungay) + " ĐẾN NGÀY " + string.Format("{0:dd/MM/yyy}", denngay);
                ws.Cells["A1"].Style.Font.Bold = true;
                ws.Cells["A1"].Style.Font.Size = 20;
                //title
                ws.Cells["A2"].Value = "Mã SKU";
                ws.Cells["B2"].Value = "Tên sản phẩm";
                ws.Cells["C2"].Value = "Thuộc tính";
                ws.Cells["D2"].Value = "Số lượng tồn đầu";
                ws.Cells["E2"].Value = "Số lượng nhập";
                ws.Cells["F2"].Value = "Số lượng tồn sau";
                ws.Cells["G2"].Value = "Ngày nhập";

                int i = 0;
                foreach (var item in stockImports)
                {
                    i++;
                    int j = i + 2;
                    //set border cho tất cả các ô từ dòng thứ 3
                    for (int k = 1; k < 8; k++)
                    {
                        ws.Cells[j, k].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    ws.Cells["A" + j].Value = item.SKU;
                    ws.Cells["B" + j].Value = item.Product.Posts.FirstOrDefault().Title;
                    //biến thể
                    var post = item.Product.Posts.FirstOrDefault();
                    string strAtt = "";
                    if (post.VariantAttributeCombinations.FirstOrDefault(x => x.SKU == item.SKU) != null)
                    {
                        var Attribute = post.VariantAttributeCombinations.FirstOrDefault(x => x.SKU == item.SKU).Attribute;
                        if (Attribute != null)
                        {
                            var att = Attribute.Split(',');
                            foreach (var s in att)
                            {
                                int id = int.Parse(s);
                                var varopt = db.OptionVariants.FirstOrDefault(x => x.Id == id);
                                strAtt = strAtt + varopt.GroupVariant.Name + ":" + varopt.Name + ", \n";
                            }
                        }
                    }
                    ws.Cells["C" + j].Value = strAtt;

                    ws.Cells["D" + j].Value = item.CurrentQuantity;
                    ws.Cells["E" + j].Value = item.Quantity;
                    ws.Cells["F" + j].Value = item.Quantity + item.CurrentQuantity;
                    ws.Cells["G" + j].Value = string.Format("{0:dd/MM/yyy}", item.ImportDate);
                }
            }

            byte[] data = pck.GetAsByteArray();
            return File(data, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        void SetTemplate(ExcelWorksheet ws)
        {
            for (int i = 1; i < 8; i++)
            {
                ws.Column(i).Width = 20;
                ws.Cells[2, i].Style.Font.Bold = true;
                ws.Cells[2, i].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            }
        }

        #endregion

        #region Nhập exel
        [HttpPost]
        public ActionResult ImportStockFromXlsx()
        {

            try
            {
                var file = Request.Files["importexcelfile"];
                if (file != null && file.ContentLength > 0)
                {
                    Stream stream = file.InputStream;
                    // ok, we can run the real code of the sample now
                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        // get the first worksheet in the workbook
                        var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                            throw new Exception("No worksheet found");

                        //the columns
                        var properties = new string[]
                {
                    "SKU",
                    "ProductName",
                    "Attribute",
                    "current",
                    "Quantity",
                    "",
                    "ImportDate",

                };
                        int iRow = 3;
                        while (true)
                        {
                            bool allColumnsAreEmpty = true;
                            for (var i = 1; i <= properties.Length; i++)
                                //nếu chỉ cần có 1 column có giá trị
                                if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                                {
                                    allColumnsAreEmpty = false;
                                    break;
                                }
                            //nếu tất cả các cột đều trống stop.
                            if (allColumnsAreEmpty)
                                break;

                            string SKU = Convert.ToString(worksheet.Cells[iRow, GetColumnIndex(properties, "SKU")].Value);
                            int quantity = Convert.ToInt32(worksheet.Cells[iRow, GetColumnIndex(properties, "Quantity")].Value);

                            DateTime? importDate = DateTime.Now; //null;
                            var importDateExcel = worksheet.Cells[iRow, GetColumnIndex(properties, "ImportDate")].Value;
                            if (importDateExcel != null)
                            {
                                IFormatProvider culture = new System.Globalization.CultureInfo("fr-FR", true);
                                importDate = DateTime.Parse(importDateExcel.ToString(), culture, System.Globalization.DateTimeStyles.AssumeLocal);
                            }

                            //check trước xem sku có phải là của biến thể ko.
                            var variantCB = db.VariantAttributeCombinations.FirstOrDefault(x => x.SKU == SKU);
                            //check sku product
                            var inven = db.Inventories.FirstOrDefault(z => z.Product.SKU == SKU && z.Agent_Id == 1);

                            StockImport stock = new StockImport();
                            stock.Quantity = quantity;
                            stock.ImportDate = (DateTime)importDate;
                            // nếu có biến thể add số lượng vào
                            if (variantCB != null)
                            {
                                stock.SKU = variantCB.SKU;
                                stock.CurrentQuantity = variantCB.Quantity;//lưu lại số lượng củ
                                stock.ProductId = variantCB.Post.Product.Id;
                                //lưu số lượng mới vào kho biến thể
                                variantCB.Quantity = variantCB.Quantity + quantity;
                                db.StockImports.Add(stock);
                            }
                            else//nl check kho của sp
                                if (inven != null)
                                {
                                    stock.SKU = inven.Product.SKU;
                                    stock.CurrentQuantity = inven.Quantity;//lưu lại số lượng củ
                                    stock.ProductId = inven.Product_Id;
                                    //lưu số lượng mới
                                    inven.Quantity = inven.Quantity + quantity;
                                    //gán tên sp
                                    db.StockImports.Add(stock);
                                }

                            db.SaveChanges();
                            iRow++;
                        }
                    }
                    return RedirectToAction("StockImport");

                }
                else
                {
                    TempData["Error"] = "lỗi file";
                    return RedirectToAction("StockImport");
                }
            }
            catch (Exception exc)
            {
                // ErrorNotification(exc);
                return RedirectToAction("StockImport");
            }

        }

        #region Utilities

        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            if (columnName == null)
                throw new ArgumentNullException("columnName");

            for (int i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i + 1; //excel indexes start from 1
            return 0;
        }

        #endregion

        #endregion

    }
}
