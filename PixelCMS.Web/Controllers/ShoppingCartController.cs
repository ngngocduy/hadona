using System.Data;
using System.Text;
using System.Web.Security;
using MvcWithOnepay.Models;
using PixelCMS.Core.Models;
using PixelCMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PixelCMS.Web.Models.Order;
using PixelCMS.Web.NganLuongService;
using WebApplication2;

// -----------------------------------------
// PIXEL CMS
// AccountController.cs v3.5
// May.2014
// Author: Ba Luan
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Controllers
{
    public class ShoppingCartController : BaseController
    {
        // Get /ShoppingCart
        pixelcmsEntities db = new pixelcmsEntities();

        List<SelectListItem> CityList = new List<SelectListItem>();
        List<SelectListItem> DistrictList = new List<SelectListItem>();
        List<SelectListItem> Shipping_CityList = new List<SelectListItem>();
        List<SelectListItem> Shipping_DistrictList = new List<SelectListItem>();

        public ActionResult Index()
        {
            string username = User.Identity.Name;
            var Citys = db.Cities.ToList();

            var address = db.Address.OrderByDescending(z => z.Date).FirstOrDefault(x => x.Username == username && x.Type == 1);
            var model = new OrderModel();
            if (address != null)
            {
                model.Name = address.Name;
                model.Phone = address.Phone;
                model.Address = address.Address1;
                model.Email = address.Email;

                model.Shipping_Name = address.Name;
                model.Shipping_Phone = address.Phone;
                model.Shipping_Address = address.Address1;
            }
            model.dcgh = true;

            //drop City
            // model.CityList.Add(new SelectListItem() { Text = "Chọn Tỉnh/TP", Value = "0" });
            if (address != null)
            {
                var district = db.Districts.FirstOrDefault(x => x.Id == address.District_Id);//get district
                if (district != null)
                {
                    var selectedCity = db.Cities.FirstOrDefault(x => x.Id == district.City_Id);//get district
                    foreach (var c in Citys)
                        model.CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (selectedCity != null && c.Id == selectedCity.Id) });//not operation???
                    ViewBag.CityList = new SelectList(model.CityList, "Value", "Text", selectedCity.Id);
                    //gán city cho shipping city
                    ViewBag.Shipping_CityList = new SelectList(model.CityList, "Value", "Text", selectedCity.Id);
                }
                else
                {
                    foreach (var c in Citys)
                        model.CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
                    ViewBag.CityList = new SelectList(model.CityList, "Value", "Text");
                    //gán city cho shipping city
                    ViewBag.Shipping_CityList = new SelectList(model.CityList, "Value", "Text");
                }
            }
            else
            {
                foreach (var c in Citys)
                    model.CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
                ViewBag.CityList = new SelectList(model.CityList, "Value", "Text");
                //gán city cho shipping city
                ViewBag.Shipping_CityList = new SelectList(model.CityList, "Value", "Text");
            }

            //drop District
            // model.DistrictList.Add(new SelectListItem() { Text = "Chọn Quận/Huyện", Value = "0" });
            if (address != null)
            {
                var selectedDistricts = db.Districts.FirstOrDefault(x => x.Id == address.District_Id);//get district
                if (selectedDistricts != null)
                {
                    var Districts = db.Districts.Where(z => z.City_Id == selectedDistricts.City_Id).ToList();
                    foreach (var d in Districts)
                        model.DistrictList.Add(new SelectListItem() { Text = d.Name, Value = d.Id.ToString(), Selected = (d.Id == selectedDistricts.Id) });//not operation???
                    ViewBag.DistrictList = new SelectList(model.DistrictList, "Value", "Text", selectedDistricts.Id);
                    //gán district cho shipping district
                    ViewBag.Shipping_DistrictList = new SelectList(model.DistrictList, "Value", "Text", selectedDistricts.Id);
                }
            }
            else
            {
                ViewBag.DistrictList = new SelectList(model.DistrictList, "Value", "Text");
                //gán district cho shipping district
                ViewBag.Shipping_DistrictList = new SelectList(model.DistrictList, "Value", "Text");
            }

            //address Ship
            var addressShip = db.Address.OrderByDescending(z => z.Date).FirstOrDefault(x => x.Username == username && x.Type == 2);
            //drop Cityship
            //model.Shipping_CityList.Add(new SelectListItem() { Text = "Chọn Tỉnh/TP", Value = "0" });
           /* if (addressShip != null)
            {
                var districtship = db.Districts.FirstOrDefault(x => x.Id == addressShip.District_Id);//get district
                if (districtship != null)
                {
                    var selectedCity = db.Cities.FirstOrDefault(x => x.Id == districtship.City_Id);//get district
                    foreach (var c in Citys)
                        model.Shipping_CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (selectedCity != null && c.Id == selectedCity.Id) });//not operation???
                    ViewBag.Shipping_CityList = new SelectList(model.Shipping_CityList, "Value", "Text", selectedCity.Id);
                }
                else
                {
                    foreach (var c in Citys)
                        model.Shipping_CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
                    ViewBag.Shipping_CityList = new SelectList(model.Shipping_CityList, "Value", "Text");
                }
            }
            else
            {
                foreach (var c in Citys)
                    model.Shipping_CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
                ViewBag.Shipping_CityList = new SelectList(model.Shipping_CityList, "Value", "Text");
            }

            //drop Districtship
            // model.Shipping_DistrictList.Add(new SelectListItem() { Text = "Chọn Quận/Huyện", Value = "0" });
            if (addressShip != null)
            {
                var selectedDistrictsShip = db.Districts.FirstOrDefault(x => x.Id == addressShip.District_Id);//get district
                if (selectedDistrictsShip != null)
                {
                    var Districts = db.Districts.Where(z => z.City_Id == selectedDistrictsShip.City_Id).ToList();
                    foreach (var d in Districts)
                        model.Shipping_DistrictList.Add(new SelectListItem() { Text = d.Name, Value = d.Id.ToString(), Selected = (d.Id == selectedDistrictsShip.Id) });//not operation???
                    ViewBag.Shipping_DistrictList = new SelectList(model.Shipping_DistrictList, "Value", "Text", selectedDistrictsShip.Id);
                }
            }
            else
            {
                ViewBag.Shipping_DistrictList = new SelectList(model.DistrictList, "Value", "Text");
            }*/

            return View(model);
        }

        /// <summary>
        /// Indexes the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 7/28/2014 3:50 PM </created>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(OrderModel order, string ptthanhtoan)
        {
            ShoppingCart objCart = (ShoppingCart)Session["Cart"];
            var Citys = db.Cities.ToList();
            //string username = User.Identity.Name;
            //var address = db.Address.OrderByDescending(z => z.Date).FirstOrDefault(x => x.Username == username && x.Type == 1);

            #region drop cty district
            //drop City

            if (order.DistrictId > 0)
            {
                var district = db.Districts.FirstOrDefault(x => x.Id == order.DistrictId);//get district
                if (district != null)
                {
                    var selectedCity = db.Cities.FirstOrDefault(x => x.Id == district.City_Id);//get district
                   
                    foreach (var c in Citys)
                        order.CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (selectedCity != null && c.Id == selectedCity.Id) });//not operation???
                    ViewBag.CityList = new SelectList(order.CityList, "Value", "Text", selectedCity.Id);
                }
            }
            else
            {
                order.CityList.Clear();
                foreach (var c in Citys)
                    order.CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
                ViewBag.CityList = new SelectList(order.CityList, "Value", "Text");
            }

            //drop District
            if (order.DistrictId > 0)
            {
                var selectedDistricts = db.Districts.FirstOrDefault(x => x.Id == order.DistrictId);//get district
                if (selectedDistricts != null)
                {
                    var Districts = db.Districts.Where(z => z.City_Id == selectedDistricts.City_Id).ToList();
                    foreach (var d in Districts)
                        order.DistrictList.Add(new SelectListItem() { Text = d.Name, Value = d.Id.ToString(), Selected = (d.Id == selectedDistricts.Id) });//not operation???
                    ViewBag.DistrictList = new SelectList(order.DistrictList, "Value", "Text", selectedDistricts.Id);
                }
            }
            else
            {
                order.DistrictList.Clear();
                ViewBag.DistrictList = new SelectList(order.DistrictList, "Value", "Text");
            }


            //address Ship
            // var addressShip = db.Address.OrderByDescending(z => z.Date).FirstOrDefault(x => x.Username == username && x.Type == 2);
            //drop Cityship
            if (order.Shipping_DistrictId > 0)
            {
                var districtship = db.Districts.FirstOrDefault(x => x.Id == order.Shipping_DistrictId);//get district
                if (districtship != null)
                {
                    var selectedCity = db.Cities.FirstOrDefault(x => x.Id == districtship.City_Id);//get district
                    foreach (var c in Citys)
                        order.Shipping_CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (selectedCity != null && c.Id == selectedCity.Id) });//not operation???
                    ViewBag.Shipping_CityList = new SelectList(order.Shipping_CityList, "Value", "Text", selectedCity.Id);
                }
            }
            else
            {
                order.Shipping_CityList.Clear();
                foreach (var c in Citys)
                    order.Shipping_CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
                ViewBag.Shipping_CityList = new SelectList(order.Shipping_CityList, "Value", "Text");
            }

            //drop Districtship
            if (order.Shipping_DistrictId > 0)
            {
                var selectedDistrictsShip = db.Districts.FirstOrDefault(x => x.Id == order.Shipping_DistrictId);//get district
                if (selectedDistrictsShip != null)
                {
                    var Districts = db.Districts.Where(z => z.City_Id == selectedDistrictsShip.City_Id).ToList();
                    foreach (var d in Districts)
                        order.Shipping_DistrictList.Add(new SelectListItem() { Text = d.Name, Value = d.Id.ToString(), Selected = (d.Id == selectedDistrictsShip.Id) });//not operation???
                    ViewBag.Shipping_DistrictList = new SelectList(order.Shipping_DistrictList, "Value", "Text", selectedDistrictsShip.Id);
                }
            }
            else
            {
                order.Shipping_DistrictList.Clear();
                ViewBag.Shipping_DistrictList = new SelectList(order.DistrictList, "Value", "Text");
            }
            #endregion

            //tỉnh/tp
            var city = db.Cities.FirstOrDefault(z => z.Id == order.CityId);
            var cityship = db.Cities.FirstOrDefault(z => z.Id == order.Shipping_CityId);
            //quận/huyện
            var district1 = db.Districts.FirstOrDefault(z => z.Id == order.DistrictId);
            var districtship1 = db.Districts.FirstOrDefault(z => z.Id == order.Shipping_DistrictId);

            ModelState.Remove("Shipping_CityList");
            ModelState.Remove("Shipping_DistrictList");
            ModelState.Remove("DistrictList");
            ModelState.Remove("CityList");
            if (ModelState.IsValid && objCart != null && objCart.GetCount() > 0)
            {
                var ordermodel = new Order();
                ordermodel.Name = order.Name;
                ordermodel.Phone = order.Phone;
                ordermodel.Email = order.Email;
                ordermodel.Address = "Tỉnh/TP:" + (city != null ? city.Name : "") + ", Quận/Huyện:" + (district1 != null ? district1.Name : "") + ", Địa chỉ:" + order.Address;
                ordermodel.Note = order.Note;
                ordermodel.Username = User.Identity.Name;
                //lưu tình trạng thanh toán qua onepay=1
                
                //nếu check địa chỉ thanh toán=địa chỉ giao hàng
                if (order.dcgh)
                {
                    ordermodel.Shipping_Name = ordermodel.Name;
                    ordermodel.Shipping_Phone = ordermodel.Phone;
                    ordermodel.Shipping_Address = ordermodel.Address;
                    ordermodel.Shipping_Email = ordermodel.Email;
                }
                else
                {
                    ordermodel.Shipping_Name = order.Shipping_Name;
                    ordermodel.Shipping_Phone = order.Shipping_Phone;
                    ordermodel.Shipping_Address = "Tỉnh/TP:" + (cityship != null ? cityship.Name : "") + ", Quận/Huyện:" + (districtship1 != null ? districtship1.Name : "") + ", Địa chỉ:" + order.Shipping_Address;
                    ordermodel.Shipping_Email = order.Shipping_Email;
                }
                //tạo đơn hàng.nếu có check thì lấy địa chỉ giao hàng ngược lại lấy địa chỉ thanh toán
                int orderid = objCart.CreateOrder(ordermodel, order.dcgh ? order.DistrictId : order.Shipping_DistrictId);

                //---------thanh toán onepay cổng quốc tế--------------------------------------------------
                if (ptthanhtoan == "onepay_quocte")
                {
                    decimal totalPrices = 0;

                    if (objCart != null && objCart.GetCount() > 0)
                    {
                        totalPrices = objCart.GetTotal();
                    }
                    string amount = (totalPrices * 100).ToString();
                    string url = IRedirectOnepay(RandomString(), amount, "", orderid);
                    // Empty the shopping cart
                    objCart.EmptyCart();
                    return Redirect(url);
                }
                //--------thanh toán onepay cổng nội địa---------------------------------------------------
                if (ptthanhtoan == "onepay_noidia")
                {
                    decimal totalPrices = 0;

                    if (objCart != null && objCart.GetCount() > 0)
                    {
                        totalPrices = objCart.GetTotal();
                    }
                    string amount = (totalPrices * 100).ToString();
                    string url = RedirectOnepay(RandomString(), amount, "", orderid);
                    // Empty the shopping cart
                    objCart.EmptyCart();
                    return Redirect(url);
                }

                #region Ngân Lượng
                if (ptthanhtoan == "nganluong")
                {
                    var config = new CommonController();
                    var nl = new NL_Checkout();

                    decimal totalPrices = 0;

                    if (objCart != null && objCart.GetCount() > 0)
                    {
                        totalPrices = objCart.GetTotal();
                    }
                    string amount = totalPrices.ToString();

                    String receiver = config.LoadOption("email-receiver");
                    String comments = "Thanh toán ngân lượng"; //Thông tin giao dịch
                    String return_url = Url.Action("nganluongResponse","ShoppingCart"); // Địa chỉ trả về 
                    String price = amount;
                    String url_NL;
                    url_NL = nl.buildCheckoutUrl(return_url, receiver, comments, orderid.ToString(), price);

                    objCart.EmptyCart();
                    return Redirect(url_NL);
                }

                #endregion

                // Empty the shopping cart
                objCart.EmptyCart();
                TempData["result"] = "Thanh toán thành công";
                TempData["OrderId"] = orderid;
               // return RedirectToAction("Result","ShoppingCart");
                return RedirectToAction("Result");
            }
            return View(order);
        }

        public ActionResult login()
        {
            return PartialView("_Login");
        }

        #region Onpay,ngan luong

        public ActionResult nganluongResponse()
        {
            var nl = new NL_Checkout();
            var config = new CommonController();
            //lấy kết quả trả về
           string transaction_info= Request.QueryString["transaction_info"];
           string order_code = Request.QueryString["order_code"];
           string price = Request.QueryString["price"];
           string payment_id = Request.QueryString["payment_id"];
           string payment_type = Request.QueryString["payment_type"];
           string error_text = Request.QueryString["error_text"];
           string secure_code = Request.QueryString["secure_code"];
           string TRANSACTION_TYPE = Request.QueryString["TRANSACTION_TYPE"];

           bool bl = nl.verifyPaymentUrl(transaction_info, order_code, price, payment_id, payment_type, error_text, secure_code);
            if (!bl)
            {
                TempData["result"] = error_text;
            }
            else if(string.IsNullOrEmpty(error_text))
            {
                //gọi services check tình trạng
              /*  NGANLUONG_APIPortTypeClient sv = new NGANLUONG_APIPortTypeClient();
                string merchant_id = config.LoadOption("NL_merchant_id");
                string param ="<ORDERS><TOTAL>1</TOTAL><ORDER><ORDER_CODE>"+order_code+"</ORDER_CODE><PAYMENT_ID>"+payment_id+"</PAYMENT_ID></ORDER></ORDERS>";
                string checksum = nl.GetMD5Hash(merchant_id + param + config.LoadOption("NL_password"));
               var s=  sv.checkOrder(merchant_id, param,checksum);*/

                int orderId = int.Parse(order_code);
                var order = db.Orders.Find(orderId);
                order.PaymentStatus = 4;//đã thanh toán qua ngân lượng
                db.SaveChanges();
                TempData["result"] = "Đã thanh toán thành công";
                TempData["OrderId"] = orderId;
            }
            return RedirectToAction("Result");
        }

        #region Local
        /// <summary>
        /// Chuyen huong url sang trang thanh toan cua onepay
        /// </summary>

        public ActionResult OnepayResponse()
        {
            var config = new CommonController();
            string hashvalidateResult = "";

            // Khoi tao lop thu vien
            VPCRequest conn = new VPCRequest(config.LoadOption("URL_ONEPAY_TRUTH1"));
            conn.SetSecureSecret(config.LoadOption("SecureHash1"));

            // Xu ly tham so tra ve va du lieu ma hoa
            hashvalidateResult = conn.Process3PartyResponse(Request.QueryString);

            // Lay tham so tra ve tu cong thanh toan
            string vpc_TxnResponseCode = conn.GetResultField("vpc_TxnResponseCode");
            string amount = conn.GetResultField("vpc_Amount");
            string localed = conn.GetResultField("vpc_Locale");
            string command = conn.GetResultField("vpc_Command");
            string version = conn.GetResultField("vpc_Version");
            string cardType = conn.GetResultField("vpc_Card");
            string orderInfo = conn.GetResultField("vpc_OrderInfo");
            string merchantID = conn.GetResultField("vpc_Merchant");
            string authorizeID = conn.GetResultField("vpc_AuthorizeId");
            string merchTxnRef = conn.GetResultField("vpc_MerchTxnRef");
            string transactionNo = conn.GetResultField("vpc_TransactionNo");
            string acqResponseCode = conn.GetResultField("vpc_AcqResponseCode");
            string txnResponseCode = vpc_TxnResponseCode;
            string message = conn.GetResultField("vpc_Message");

            int orderId = int.Parse(orderInfo);
            var order = db.Orders.Find(orderId);

            // Kiem tra 2 tham so tra ve quan trong nhat
            if (hashvalidateResult == "CORRECTED" && txnResponseCode.Trim() == "0")
            {
                order.PaymentStatus = 2;//đã thanh toán qua onepay
                db.SaveChanges();
                TempData["result"] = "Đã thanh toán thành công";
                TempData["OrderId"] = orderId;
                return RedirectToAction("Result");
            }
            else if (hashvalidateResult == "INVALIDATED" && txnResponseCode.Trim() == "0")
            {
                order.PaymentStatus = 3;//thanh toán qua onepay trong tình trạng đang chờ
                db.SaveChanges();
                TempData["result"] = "Thanh toán OnePay trong tình trạng đang xử lý";
                TempData["OrderId"] = orderId;
                return RedirectToAction("Result");
            }
            else
            {
                TempData["result"] = "Thanh toán OnePay Không thành công(Đơn hàng đã được lưu)";
                return View("Result");
            }
        }

        /// <summary>
        /// Redirect den onepay
        /// </summary>
        public string RedirectOnepay(string codeInvoice, string amount, string ip, int orderid)
        {

            //lấy từ option
            var config = new CommonController();
            // Khoi tao lop thu vien
            //url truth https://onepay.vn/onecomm-pay/vpc.op
            VPCRequest conn = new VPCRequest(config.LoadOption("URL_ONEPAY_TRUTH1"));
            conn.SetSecureSecret(config.LoadOption("SecureHash1"));

            // Gan cac thong so de truyen sang cong thanh toan onepay
            // conn.AddDigitalOrderField("AgainLink", config.LoadOption("AgainLink"));
            conn.AddDigitalOrderField("Title", "Thanh toán qua onepay cổng nội địa");
            conn.AddDigitalOrderField("vpc_Locale", config.LoadOption("Language1"));
            conn.AddDigitalOrderField("vpc_Version", config.LoadOption("Version1"));
            conn.AddDigitalOrderField("vpc_Command", config.LoadOption("Command1"));
            conn.AddDigitalOrderField("vpc_Merchant", config.LoadOption("MerchantId1"));
            conn.AddDigitalOrderField("vpc_AccessCode", config.LoadOption("AccessCode1"));
            conn.AddDigitalOrderField("vpc_MerchTxnRef", RandomString());
            conn.AddDigitalOrderField("vpc_OrderInfo", orderid.ToString());
            conn.AddDigitalOrderField("vpc_Amount", amount);

            // Thong tin them ve khach hang. De trong neu khong co thong tin
            /* conn.AddDigitalOrderField("vpc_SHIP_Street01", order.Shipping_Address);
             conn.AddDigitalOrderField("vpc_SHIP_Provice", order.DistrictId.ToString());//quan/huyen
             conn.AddDigitalOrderField("vpc_SHIP_City", order.Shipping_CityId.ToString());
             conn.AddDigitalOrderField("vpc_SHIP_Country", "");
             conn.AddDigitalOrderField("vpc_Customer_Phone", order.Shipping_Phone);
             conn.AddDigitalOrderField("vpc_Customer_Email", order.Shipping_Email);
             conn.AddDigitalOrderField("vpc_Customer_Id", "");
             conn.AddDigitalOrderField("vpc_TicketNo", ip);*/

            conn.AddDigitalOrderField("vpc_Currency", "VND");
            conn.AddDigitalOrderField("vpc_ReturnURL", Url.Action("OnepayResponse", "ShoppingCart", null, Request.Url.Scheme, null));

            String url = conn.Create3PartyQueryString();
            return url;
        }

        #endregion

        #region Internal

        /// <summary>
        /// Chuyen huong url sang trang thanh toan cua onepay
        /// </summary>

        public ActionResult IOnepayResponse()
        {
            var config = new CommonController();
            string hashvalidateResult = "";

            // Khoi tao lop thu vien
            VPCRequest conn = new VPCRequest(config.LoadOption("URL_ONEPAY_TRUTH"));
            conn.SetSecureSecret(config.LoadOption("SecureHash"));

            // Xu ly tham so tra ve va du lieu ma hoa
            hashvalidateResult = conn.Process3PartyResponse(Request.QueryString);

            // Lay tham so tra ve tu cong thanh toan
            string vpc_TxnResponseCode = conn.GetResultField("vpc_TxnResponseCode");
            string amount = conn.GetResultField("vpc_Amount");
            string localed = conn.GetResultField("vpc_Locale");
            string command = conn.GetResultField("vpc_Command");
            string version = conn.GetResultField("vpc_Version");
            string cardType = conn.GetResultField("vpc_Card");
            string orderInfo = conn.GetResultField("vpc_OrderInfo");
            string merchantID = conn.GetResultField("vpc_Merchant");
            string authorizeID = conn.GetResultField("vpc_AuthorizeId");
            string merchTxnRef = conn.GetResultField("vpc_MerchTxnRef");
            string transactionNo = conn.GetResultField("vpc_TransactionNo");
            string acqResponseCode = conn.GetResultField("vpc_AcqResponseCode");
            string txnResponseCode = vpc_TxnResponseCode;
            string message = conn.GetResultField("vpc_Message");

            int orderId = int.Parse(orderInfo);
            var order = db.Orders.Find(orderId);

            // Kiem tra 2 tham so tra ve quan trong nhat
            if (hashvalidateResult == "CORRECTED" && txnResponseCode.Trim() == "0")
            {
                order.PaymentStatus = 2;//đã thanh toán qua onepay
                db.SaveChanges();
                TempData["result"] = "Đã thanh toán thành công";
                TempData["OrderId"] = orderId;
                return RedirectToAction("Result");
            }
            else if (hashvalidateResult == "INVALIDATED" && txnResponseCode.Trim() == "0")
            {
                order.PaymentStatus = 3;//thanh toán qua onepay trong tình trạng đang chờ
                db.SaveChanges();
                TempData["result"] = "Thanh toán OnePay trong tình trạng đang xử lý";
                TempData["OrderId"] = orderId;
                return RedirectToAction("Result");
            }
            else
            {
                TempData["result"] = "Thanh toán OnePay Không thành công(Đơn hàng đã được lưu)";
                return View("Result");
            }
        }

        /// <summary>
        /// Sinh ky tu ngau nhien
        /// </summary>
        private string RandomString()
        {
            var str = new StringBuilder();
            var random = new Random();
            for (int i = 0; i <= 5; i++)
            {
                var c = Convert.ToChar(Convert.ToInt32(random.Next(65, 68)));
                str.Append(c);
            }
            return str.ToString().ToLower();
        }

        /// <summary>
        /// Redirect den onepay
        /// </summary>
        public string IRedirectOnepay(string codeInvoice, string amount, string ip, int orderid)
        {
            //lấy từ option
            var config = new CommonController();
            //Khoi tao lop thu vien
            //url truth http://onepay.vn/vpcpay/vpcpay.op
            VPCRequest conn = new VPCRequest(config.LoadOption("URL_ONEPAY_TRUTH"));
            conn.SetSecureSecret(config.LoadOption("SecureHash"));

            // Gan cac thong so de truyen sang cong thanh toan onepay
            conn.AddDigitalOrderField("AgainLink", config.LoadOption("AgainLink"));
            conn.AddDigitalOrderField("Title", "Tich hop onepay vao web asp.net mvc3,4");
            conn.AddDigitalOrderField("vpc_Locale", config.LoadOption("Language"));
            conn.AddDigitalOrderField("vpc_Version", config.LoadOption("Version"));
            conn.AddDigitalOrderField("vpc_Command", config.LoadOption("Command"));
            conn.AddDigitalOrderField("vpc_Merchant", config.LoadOption("MerchantId"));
            conn.AddDigitalOrderField("vpc_AccessCode", config.LoadOption("AccessCode"));
            conn.AddDigitalOrderField("vpc_MerchTxnRef", RandomString());
            conn.AddDigitalOrderField("vpc_OrderInfo", orderid.ToString());
            conn.AddDigitalOrderField("vpc_Amount", amount);

            // Thong tin them ve khach hang. De trong neu khong co thong tin
            /* conn.AddDigitalOrderField("vpc_SHIP_Street01", order.Shipping_Address);
             conn.AddDigitalOrderField("vpc_SHIP_Provice", order.DistrictId.ToString());//quan/huyen
             conn.AddDigitalOrderField("vpc_SHIP_City", order.Shipping_CityId.ToString());
             conn.AddDigitalOrderField("vpc_SHIP_Country", "");
             conn.AddDigitalOrderField("vpc_Customer_Phone", order.Shipping_Phone);
             conn.AddDigitalOrderField("vpc_Customer_Email", order.Shipping_Email);
             conn.AddDigitalOrderField("vpc_Customer_Id", "");
             conn.AddDigitalOrderField("vpc_TicketNo", ip);*/

            conn.AddDigitalOrderField("vpc_ReturnURL", Url.Action("IOnepayResponse", "ShoppingCart", null, Request.Url.Scheme, null));

            string url = conn.Create3PartyQueryString();
            return url;
        }

        #endregion

        #endregion

        public ActionResult Result()
        {
            var id = TempData["OrderId"];
            if (id!=null)
            {
                Order order = db.Orders.Find(id);

                ViewBag.name = order.Name;
                ViewBag.sdt = order.Shipping_Phone;
                ViewBag.dc = order.Shipping_Address;

                var orderdetails = db.Order_Details.Where(z => z.Order_Id == order.Id).ToList();
                return View(orderdetails); 
            }
            return View();
        }
    }
}
