using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using PixelCMS.Core.Models;
using PixelCMS.Mailer;
using PixelCMS.Helpers;
using PagedList;

// -----------------------------------------
// PIXEL CMS
// CommonController.cs v1.2
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Controllers
{
    public class CommonController : BaseController
    {
        private string currentlang = CultureHelper.GetCurrentNeutralCulture();
        private pixelcmsEntities db = new pixelcmsEntities();
        #region header
        public ActionResult Header()
        {
            return PartialView("_Header");
        }
        #endregion

        #region Menu
        public ActionResult Menu(string culture, string positioncode, string vitri)
        {
            var menus = db.MenuItems.Where(x => x.Menu1.MenuPosition1.Active == true && x.Menu1.MenuPosition1.Code == positioncode && x.Active == true && x.Menu1.Lang == culture).OrderBy(o => o.Order).ToList();
            // Moi position moi thi tao them 1 "if" va tra ve mot view moi
            if (positioncode == "menu-top")
                return PartialView("_MenuTop", menus);

            if (positioncode == "menu-footer" || positioncode == "menu-footer1")
                return PartialView("_Menu_Footer", menus);

            if (positioncode == "menu-left" | positioncode == "menu-award" | positioncode == "menu-invertor")
                return PartialView("_MenuLeft", menus);
            if (vitri == "footer")
                return PartialView("_Menu_Footer", menus);

            return PartialView("_Menu", menus);
        }
        #endregion

        #region Footer
        public ActionResult Footer()
        {
            return PartialView("_Footer");
        }
        #endregion

        #region Left
        /*   public ActionResult MenuLeft(string culture, string positioncode)
        {
            var menus = db.MenuItems.Where(x => x.Menu1.MenuPosition1.Active == true && x.Menu1.MenuPosition1.Code == positioncode && x.Active == true && x.Menu1.Lang == culture).OrderBy(o => o.Order).ToList();

            return PartialView("_MenuLeft", menus);
        }*/

        public ActionResult Left()
        {
            return PartialView("_Left");
        }
        #endregion

        #region Option
        //
        // GET: /Common/
        public string LoadOption(string code)
        {
            try
            {
                Option conf = db.Options.FirstOrDefault(c => c.Code == code);
                conf.Value = System.Web.HttpUtility.HtmlDecode(conf.Value);
                return conf.Value;
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion

        #region URL
        public string LoadURL(string culture, string type, int id = 0)
        {
            try
            {
                var url = "#";
                if (type == "posttype")
                {
                    var posttype = db.Slugs.FirstOrDefault(x => x.PostType == id && x.Lang == culture).Slug_Full;
                    url = "/" + culture + "/" + posttype;
                }
                else if (type == "post")
                {
                    var post = db.Posts.Find(id);
                    var postslug = db.Slugs.FirstOrDefault(x => x.Post == post.Id).Slug_Full;
                    // Luu y slug culture cua Post va Cat la khac nhau
                    // NEU WEBSITE 1 NGON NGU THI BO CAC PHAN CULTURE VA LANG DI (LUU Y LA VAN GIU LAI PHAN RESOURCE

                    #region POST LINK
                    if (post != null)
                    {
                        if (post.PostType.AsPost == true && post.PostType.Active == true)
                        {
                            //edit by luan
                            url = "/" + post.Lang + "/" + postslug;

                            //old
                            if (post.PostType.ShowType == true)
                            {
                                var posttype = db.Slugs.FirstOrDefault(x => x.PostType == post.PostType.Id && x.Lang == culture).Slug_Full;
                                url = "/" + post.Lang + "/" + posttype + "/" + postslug;
                            }

                        }
                    }
                    #endregion
                }
                else if (type == "cat")
                {
                    var cat = db.Cats.Find(id);
                    //  var posttype = db.Slugs.FirstOrDefault(x => x.PostType == cat.PostType.Id && x.Lang == culture).Slug_Full;
                    var catslug = db.Slugs.FirstOrDefault(x => x.Cat == cat.Id).Slug_Full;
                    if (cat.PostType.Has_Parallel_Language && cat.MainLang_Id != null)
                    {
                        var tmp = db.Cats.Find(cat.MainLang_Id).Owner_Id;
                        if (tmp != null)
                        {
                            cat.Owner_Id = db.Cats.FirstOrDefault(x => x.MainLang_Id == tmp).Id;
                        }
                    }
                    #region CAT LINK
                    if (cat.PostType.Active == true && cat.PostType.AsPost == true && cat.PostType.Has_Cat == true)
                    {
                        //edit by luan 5/12/2014
                        url = "/" + cat.Lang + "/" + catslug;

                        //old
                        /* if (cat.Level == 3)
                         {
                             var cat2 = db.Cats.Find(cat.Owner_Id);
                             var catslug2 = db.Slugs.FirstOrDefault(x => x.Cat == cat2.Id).Slug_Full;
                             var cat1 = db.Cats.Find(cat2.Owner_Id);
                             var catslug1 = db.Slugs.FirstOrDefault(x => x.Cat == cat1.Id).Slug_Full;

                             url = "/" + cat.Lang + "/" + posttype + "/" + catslug1 + "/" + catslug2 + "/" + catslug;
                         }
                         else if (cat.Level == 2)
                         {
                             var cat1 = db.Cats.Find(cat.Owner_Id);
                             var catslug1 = db.Slugs.FirstOrDefault(x => x.Cat == cat1.Id).Slug_Full;
                             url = "/" + cat.Lang + "/" + posttype + "/" + catslug1 + "/" + catslug;
                         }
                         else if (cat.Level == 1)
                         {
                             url = "/" + cat.Lang + "/" + posttype + "/" + catslug;
                         }*/
                    }
                    #endregion
                }

                #region SPECIAL LINK (contact, search, home)
                else if (type == "contact")
                {
                    url = "/" + culture + "/lien-he";
                }
                else if (type == "search")
                {
                    url = "/" + culture + "/search";
                }
                else if (type == "home")
                {
                    url = "/" + culture;
                }
                #endregion

                return url;
            }
            catch
            {
                return "";
            }
        }

        public ActionResult LoadURL2(string culture, string type, int id = 0)
        {
            try
            {
                var url = "#";
                if (type == "posttype")
                {
                    var posttype = db.Slugs.FirstOrDefault(x => x.PostType == id && x.Lang == culture).Slug_Full;
                    url = "/" + culture + "/" + posttype;
                }
                else if (type == "post")
                {
                    var post = db.Posts.Find(id);
                    var postslug = db.Slugs.FirstOrDefault(x => x.Post == post.Id).Slug_Full;
                    // Luu y slug culture cua Post va Cat la khac nhau
                    // NEU WEBSITE 1 NGON NGU THI BO CAC PHAN CULTURE VA LANG DI (LUU Y LA VAN GIU LAI PHAN RESOURCE

                    #region POST LINK
                    if (post != null)
                    {
                        if (post.PostType.AsPost == true && post.PostType.Active == true)
                        {
                            //edit by luan
                            url = "/" + post.Lang + "/" + postslug;

                            //old
                            if (post.PostType.ShowType == true)
                            {
                                var posttype = db.Slugs.FirstOrDefault(x => x.PostType == post.PostType.Id && x.Lang == culture).Slug_Full;
                                url = "/" + post.Lang + "/" + posttype + "/" + postslug;
                            }

                        }
                    }
                    #endregion
                }

                return Redirect(url);
            }
            catch
            {
                return RedirectToAction("index", "home");
            }
        }
        #endregion

        #region Attribute
        //
        // GET: /Common/
        public string LoadAttribute(string code, int id)
        {
            try
            {
                Post_PostAttribute attribute = db.Post_PostAttributes.FirstOrDefault(x => x.Id_Post == id && x.PostAttribute.Code == code);
                attribute.Value = System.Web.HttpUtility.HtmlDecode(attribute.Value);
                return attribute.Value;
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion

        #region Breadcrumb
        public ActionResult Breadcrumb(string breadcrumb)
        {
            ViewBag.BreadCrumb = breadcrumb;
            return PartialView("_Breadcrumb");
        }
        #endregion

        #region Search box
        // Action Search nam o ben phan Content-Page
        public ActionResult SearchBox()
        {
            return PartialView("_SearchBox");
        }
        [HttpGet]
        public JsonResult getsp(string culture, string query = "")
        {
            var slug = db.Slugs.FirstOrDefault(x => x.Slug_Full == "product");
            if (slug != null)
            {
                var list = db.Posts.Where(p => (p.Title.Contains(query) || p.Product.SKU.Contains(query)) && p.Active && p.Type == slug.PostType && p.Lang == culture)
                    .Select(x => new
                    {
                        x.Title,
                        x.Id,
                        x.Description,
                        x.Thumb,
                        x.Price,
                        SKU = x.Product.SKU,
                        slug = x.Slugs.FirstOrDefault(z => z.Post == x.Id).Slug_Full
                    }).Take(5);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Contact Form

        public ActionResult Contact(string productname)
        {
            ViewBag.productname = productname;
            if (!string.IsNullOrEmpty(productname))
            {
                return PartialView("_ContactProduct");
            }
            return PartialView("_Contact");
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ContactForm(Email email)
        {
            string retValue = "There was an error submitting the form, please try again later.";

            if (ModelState.IsValid)
            {
                email.DateSend = DateTime.Now;
                db.Emails.Add(email);

                //lấy email lưu vào EmailSubscribe
                var EmailSubscribe = new EmailSubscribe();
                EmailSubscribe.Email = email.Email1;
                EmailSubscribe.Active = true;
                EmailSubscribe.Date = DateTime.Now;
                //add role là member cho EmailSubscribe
                var role = db.webpages_Roles.FirstOrDefault(x => x.RoleName == "member");
                if (role != null)
                    EmailSubscribe.webpages_Roles.Add(role);

                //check EmailSubscribes đã tồn tại hay chưa nếu đã tồn tại ko lưu nữa.//cái này lưu để sử dụng cho mail marketing
                var IsExist = db.EmailSubscribes.Any(x => x.Email == email.Email1);
                if (!IsExist)
                    db.EmailSubscribes.Add(EmailSubscribe);
                //end add mail

                db.SaveChanges();
                ModelState.Clear();
                UserMailer _UserMailer = new UserMailer();
                var emails = db.Options.FirstOrDefault(s => s.Code == "email-mail");
                var emailsetting = "luanpb@thekymoi.com";
                if (emails != null)
                {
                    emailsetting = emails.Value;
                }
                _UserMailer.SendMailContact(emailsetting, email.Username, email.Email1, email.Subject, email.Text, email.Email1, email.Phone, email.Address);
                retValue = "Your Request for Contact was submitted successfully. We will contact you shortly.";
                return Json(new { succsess = true });
            }
            return Content(retValue);
        }
        #endregion

        #region EmailSubscribe
        public ActionResult EmailSubscribe()
        {
            return PartialView("_EmailSubscribe");
        }
        [HttpPost]
        public ActionResult EmailSubscribePost(string Email)
        {
            //lấy email lưu vào EmailSubscribe
            var EmailSubscribe = new EmailSubscribe();
            EmailSubscribe.Email = Email;
            EmailSubscribe.Active = true;
            EmailSubscribe.Date = DateTime.Now;
            //add role là member cho EmailSubscribe
            var role = db.webpages_Roles.FirstOrDefault(x => x.RoleName == "member");
            if (role != null)
                EmailSubscribe.webpages_Roles.Add(role);

            //check EmailSubscribes đã tồn tại hay chưa nếu đã tồn tại ko lưu nữa.//cái này lưu để sử dụng cho mail marketing
            var IsExist = db.EmailSubscribes.Any(x => x.Email == Email);
            if (!IsExist)
                db.EmailSubscribes.Add(EmailSubscribe);
            else
            {
                return Json(new { result = false });
            }
            //end add mail

            db.SaveChanges();

            return Json(new { result = true });
        }
        #endregion

        #region AdminPanel
        public ActionResult AdminPanel(int? postid, string posttype)
        {
            ViewBag.PostId = postid;
            ViewBag.PostType = posttype;
            return PartialView("_AdminPanel");
        }
        #endregion

        #region cart

        /// <summary>
        /// Them san pham vao gio hang
        /// </summary>
        /// <param name="id">ID san pham can them</param>
        /// <returns>Tra ve json theo dinh dang {Code: ReturnCode, Msg: "Return message"}</returns>
        [HttpPost]
        public JsonResult AddToCart(int id, string Attribute)
        {
            var response = new { Code = 1, Msg = "Fail" };
            var objPost = db.Posts.FirstOrDefault(x => x.Id == id);

            var culture = Session["_cultures"];
            var SessionId = Session.SessionID;
            if (objPost != null)
            {
                //set lang cho title không được lang nó gửi ve en trc vi (có thể cho chạy trên view xem sao)
                /*if (objPost.Lang != currentlang)
                {
                    var itemlang = db.Posts.FirstOrDefault(x => x.Lang == currentlang && x.MainLang_Id == objPost.Id);
                    if (itemlang != null)
                    {
                        objPost.Title = itemlang.Title;
                    }
                }*/
                //end set lang

                ShoppingCart objCart = (ShoppingCart)Session["Cart"];
                if (objCart == null)
                {
                    objCart = new ShoppingCart();
                }

                ShoppingCartItem item = new ShoppingCartItem();
                item.PostId = objPost.Id;
                item.PostTitle = objPost.Title;
                item.PostImage = objPost.Thumb;
                item.Attribute = Attribute;
                item.lang = objPost.Lang;

                if (objPost.Product != null)
                    item.SKU = objPost.Product.SKU;

                //nếu có biến thể
                var objVariantCB = db.VariantAttributeCombinations.FirstOrDefault(x => x.Attribute == Attribute);
                if (objVariantCB != null)
                {
                    item.SKU = objVariantCB.SKU;
                    //lấy giá k.hop biến thể
                    if (objVariantCB.Price > 0)
                    {
                        item.Price = objVariantCB.Price;
                    }
                }

                //kiểm tra hình biến thể nếu có thì lấy ra gán vào
                /*if (objVariantCB.ImageId != null)
                {
                    var img = db.MediaFiles.FirstOrDefault(x => x.Id == objVariantCB.ImageId);
                    if (img != null)
                    {
                        item.PostImage = img.URL;
                    }
                }*/
                //end variant

                /*   <!--chiến dịch giảm giá-->*/
                string username = User.Identity.Name;
                if (!string.IsNullOrEmpty(username) && objPost.Product != null)
                {
                    var discounts = objPost.Product.Discounts;
                    var role = Roles.GetRolesForUser(username).FirstOrDefault();

                    var discount = discounts.OrderByDescending(x => x.CreateDate).FirstOrDefault(x => x.webpages_Roles.RoleName == role);
                    //chiến dịch
                    if (discount != null && discount.StartDate <= DateTime.Now.Date && DateTime.Now.Date <= discount.EndDate)
                    {
                        objPost.Price = discount.DiscountAmount;
                    }
                    //Giảm giá
                    else if (objPost.Product.Sale_Start <= DateTime.Now.Date && DateTime.Now.Date <= objPost.Product.Sale_End)
                    {
                        objPost.Price = objPost.Product.Sale_Price;
                    }
                }
                //
                item.Price = objPost.Price != null ? objPost.Price.Value : 0;
                item.Quantity = 1;
                item.Total = objPost.Price_Old != null ? objPost.Price_Old.Value : 0;

                // nếu có thuộc tinh kết hợp
                if (item.Attribute != null)
                {
                    string str = "";
                    decimal variantPrice = 0;
                    var att = item.Attribute.Split(',');
                    foreach (var s in att)
                    {
                        int id1 = int.Parse(s);
                        var Variants = db.Variants.FirstOrDefault(x => x.Id == id1);

                        int optionid = Variants.VariantOption_Id;
                        var varopt = db.OptionVariants.FirstOrDefault(x => x.Id == optionid);
                        variantPrice = variantPrice + Variants.Price;
                        str = str + varopt.GroupVariant.Name + ":<span>" + varopt.Name + (Variants.Price > 0 ? "(+" + string.Format("{0:0,0}", Variants.Price) + ")" : "") + "</span><br/>";
                    }
                    //set tên các thuộc tính
                    item.VariantName = str;
                    //+giá của các thuộc tính thêm vào sp
                    item.Price = item.Price + variantPrice;
                    //
                    item.Total = item.Price;
                }


                if (objPost.Product != null)
                {
                    var inven = objPost.Product.Inventories.FirstOrDefault();
                    var variantCB = db.VariantAttributeCombinations.FirstOrDefault(x => x.Attribute == Attribute);
                    //check số lượng biến thể trong kho
                    if (variantCB != null)
                    {
                        var cart = objCart.ListItem.FirstOrDefault(z => z.Attribute == Attribute);
                        //nếu sl trong kho <= số lượng trong giỏ hàng
                        if (variantCB.Quantity <= (cart != null ? cart.Quantity : 0))
                        {
                            response = new { Code = 2, Msg = "Số lượng trong kho không đủ" };
                            return Json(response);
                        }
                    }
                    else
                        if (inven != null) //check số lượng trong kho
                    {
                        var cart = objCart.ListItem.FirstOrDefault(z => z.PostId == id);
                        if (inven.Quantity <= (cart != null ? cart.Quantity : 0))
                        {
                            response = new { Code = 2, Msg = "Số lượng trong kho không đủ" };
                            return Json(response);
                        }
                    }

                    objCart.AddToCart(item);
                    Session["Cart"] = objCart;
                    response = new { Code = 0, Msg = "Success" };
                }
                //add 6.4.2015
                else
                {
                    objCart.AddToCart(item);
                    Session["Cart"] = objCart;
                    response = new { Code = 0, Msg = "Success" };
                }
            }
            return Json(response);
        }

        /// <summary>
        /// Xoa san pham khoi gio hang
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RemoveFromCart(long id, string Attribute)
        {
            var response = new { Code = 1, Msg = "Fail" };
            ShoppingCart objCart = (ShoppingCart)Session["Cart"];
            if (objCart != null)
            {
                //xét ko có biển thể
                if (string.IsNullOrEmpty(Attribute))
                {

                    objCart.RemoveFromCart(id);
                    Session["Cart"] = objCart;
                    response = new { Code = 0, Msg = "Success" };
                }
                else//có biến thể mẹ phức tạp vl
                {
                    objCart.RemoveFromCartVariant(Attribute);
                    Session["Cart"] = objCart;
                    response = new { Code = 0, Msg = "Success" };
                }
            }


            return Json(response);
        }

        /// <summary>
        /// Cap nhat so luong san pham can mua trong gio hang
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateQuantity(long id, int quantity, string Attribute)
        {
            ShoppingCart objCart = (ShoppingCart)Session["Cart"];

            var response = new { Code = 1, Msg = "Fail" };

            var post = db.Posts.FirstOrDefault(x => x.Id == id);
            if (post != null)
            {
                //số lượng thực
                var inven = post.Product.Inventories.FirstOrDefault();
                //sô lượng của biến thể
                var variant = db.VariantAttributeCombinations.FirstOrDefault(x => x.Attribute == Attribute);

                //check số lượng biến thể
                if (variant != null)
                {
                    if (variant.Quantity < quantity)
                    {
                        response = new { Code = 2, Msg = "Số lượng trong kho không đủ" };
                        return Json(response);
                    }
                }
                else if (inven != null)//check số lượng
                {
                    if (inven.Quantity < quantity)
                    {
                        response = new { Code = 2, Msg = "Số lượng trong kho không đủ" };
                        return Json(response);
                    }
                }

                if (objCart != null)
                {
                    objCart.UpdateQuantity(id, quantity, Attribute);
                    Session["Cart"] = objCart;
                    response = new { Code = 0, Msg = "Success" };
                }
            }
            return Json(response);
        }

        /// <summary>
        /// Gia lap method lay thong tin san pham tu DB dua vao ID san pham
        /// </summary>
        /// <param name="id">ID san pham can lay thong tin</param>
        /// <returns></returns>
        public Post GetPostByID(int id)
        {
            var p = db.Posts.FirstOrDefault(x => x.Id == id);
            return new Post()
            {
                Id = id,
                Title = p.Title,
                Thumb = p.Thumb,
                Price = p.Price,
                Price_Old = p.Price_Old,
                Slugs = p.Slugs
            };
        }

        // [HttpPost]
        /// <summary>
        /// Updates the cart.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 5/6/2014 1:57 PM </created>
        public ActionResult UpdateCart()
        {

            ShoppingCartModels model = new ShoppingCartModels();
            model.Cart = (ShoppingCart)Session["Cart"];


            return PartialView("_UpdateCart", model);
        }

        /// <summary>
        /// Updates the cart count.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 5/6/2014 1:57 PM </created>
        public ActionResult UpdateCartCount()
        {
            ShoppingCartModels model = new ShoppingCartModels();
            model.Cart = (ShoppingCart)Session["Cart"];
            return PartialView("_UpdateCartCount", model);
        }

        public ActionResult UpdateTotal()
        {
            string username = User.Identity.Name;
            ShoppingCartModels model = new ShoppingCartModels();
            model.Cart = (ShoppingCart)Session["Cart"];

            var district = Request.QueryString["DistrictId"];
            //nếu có yêu cầu địa chỉ tính lại total đơn hàng(chỉ update cart chưa lưu.lưu ở shoppingcartcontroller)
            if (!string.IsNullOrEmpty(district))
            {
                int districtId = int.Parse(district);
                var ship = db.Shippings.OrderByDescending(z => z.CreateDate).FirstOrDefault(x => x.District_Id == districtId);
                if (ship != null)
                {
                    model.FixedFee = ship.FixedFee ?? 0;
                    if (model.Cart != null)
                        model.Total = model.Cart.GetTotal() + model.FixedFee;
                }
            }
            else
            {
                //nếu có đăng nhập :lấy địa chỉ đăng ký của user tính phí vận chuyện
                if (username != null)
                {
                    var address = db.Address.OrderByDescending(z => z.Date).FirstOrDefault(x => x.Username == username && x.Type == 1);
                    if (address != null)
                    {
                        if (address.District_Id != null)
                        {
                            var ship = db.Shippings.OrderByDescending(z => z.CreateDate).FirstOrDefault(x => x.District_Id == address.District_Id);
                            if (ship != null)
                            {
                                model.FixedFee = ship.FixedFee ?? 0;
                                if (model.Cart != null)
                                    model.Total = model.Cart.GetTotal() + model.FixedFee;
                            }
                        }
                    }
                }

            }
            return PartialView("_UpdateTotal", model);
        }

        #endregion

        #region Wishlist

        /// <summary>
        /// Wishlists the specified productid.
        /// </summary>
        /// <param name="productid">The productid.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/22/2014 10:30 AM </created>
        public ActionResult Addwishlist(int id)
        {
            var response = new { Code = 1, Msg = "Fail" };
            //gọi cookie
            HttpCookie cookierq = Request.Cookies["wishlist"];
            if (cookierq != null)
            {
                cookierq["productid"] = cookierq["productid"] + (cookierq["productid"] != "" ? "," : string.Empty) + id.ToString();
                Response.Cookies.Add(cookierq);
                cookierq.Expires = DateTime.Now.AddDays(20);
                response = new { Code = 0, Msg = "Success" };
            }
            else
            {
                HttpCookie cookie = new HttpCookie("wishlist");
                cookie["productid"] = id.ToString();
                Response.Cookies.Add(cookie);
                cookie.Expires = DateTime.Now.AddDays(20);
                response = new { Code = 0, Msg = "Success" };
            }

            return Json(response);

        }

        /// <summary>
        /// Removes from wish list.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/22/2014 12:03 PM </created>
        [HttpPost]
        public JsonResult RemoveFromWishList(long id)
        {
            var response = new { Code = 1, Msg = "Fail" };
            //gọi cookie
            HttpCookie cookierq = Request.Cookies["wishlist"];
            if (cookierq != null)
            {
                var wl = cookierq["productid"].Split(',');
                cookierq["productid"] = "";

                foreach (var item in wl)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        int productid = int.Parse(item);
                        if (productid != id)
                            cookierq["productid"] = cookierq["productid"] + (cookierq["productid"] != "" ? "," : string.Empty) + productid.ToString();
                    }
                }
                Response.Cookies.Add(cookierq);
                cookierq.Expires = DateTime.Now.AddDays(20);
                response = new { Code = 0, Msg = "Success" };
            }
            return Json(response);
        }

        /// <summary>
        /// Updates the wish list count.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/22/2014 12:03 PM </created>
        public ActionResult UpdateWishListCount()
        {
            return PartialView("_UpdateWishListCount");
        }

        /// <summary>
        /// Updates the wish list.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/22/2014 12:03 PM </created>
        public ActionResult UpdateWishList()
        {
            // ShoppingCartModels model = new ShoppingCartModels();
            //  model.Cart = (ShoppingCart)Session["Cart"];
            var model = new List<Post>();
            HttpCookie cookierq = Request.Cookies["wishlist"];
            if (cookierq != null)
            {
                var wl = cookierq["productid"].Split(',');
                foreach (var item in wl)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        int productid = int.Parse(item);
                        var post = db.Posts.Where(x => x.Id == productid);
                        model.AddRange(post);
                    }
                }
            }
            return PartialView("_UpdateWishList", model);
        }

        #endregion

        #region Compare list

        /// <summary>
        /// Adds the compare list.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/26/2014 2:31 PM </created>
        public ActionResult AddCompareList(int id)
        {
            //gọi cookie
            HttpCookie cookierq = Request.Cookies["comparelist"];
            if (cookierq != null)
            {
                var wl = cookierq["compareproductids"].Split(',');

                //xét chỉ lấy 4 sp cuối
                if (wl.Count() >= 4)
                {
                    cookierq["compareproductids"] = "";
                    foreach (var item in wl.Skip(wl.Count() - 3))
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            int proid = int.Parse(item);
                            if (proid != id)
                                cookierq["compareproductids"] = cookierq["compareproductids"] + (cookierq["compareproductids"] != "" ? "," : string.Empty) + item;
                        }
                    }
                    cookierq["compareproductids"] = cookierq["compareproductids"] + (cookierq["compareproductids"] != "" ? "," : string.Empty) + id.ToString();
                }
                else if (cookierq["compareproductids"] != "")
                {
                    cookierq["compareproductids"] = "";
                    {
                        foreach (var item in wl)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                int proid = int.Parse(item);
                                if (proid != id)
                                    cookierq["compareproductids"] = cookierq["compareproductids"] + (cookierq["compareproductids"] != "" ? "," : string.Empty) + item;
                            }
                        }
                        cookierq["compareproductids"] = cookierq["compareproductids"] + (cookierq["compareproductids"] != "" ? "," : string.Empty) + id.ToString();
                    }
                }
                else
                {
                    //add
                    cookierq["compareproductids"] = cookierq["compareproductids"] + id.ToString();
                }

                Response.Cookies.Add(cookierq);
                cookierq.Expires = DateTime.Now.AddDays(20);
            }
            else
            {
                HttpCookie cookie = new HttpCookie("comparelist");
                cookie["compareproductids"] = id.ToString();
                Response.Cookies.Add(cookie);
                cookie.Expires = DateTime.Now.AddDays(20);
            }
            return RedirectToAction("CompareList", "Content");
        }

        /// <summary>
        /// Clears the compare list.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/26/2014 3:59 PM </created>
        public ActionResult ClearCompareList()
        {
            HttpCookie cookie = Request.Cookies["comparelist"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("CompareList", "Content");
        }

        /// <summary>
        /// Removes the compare list.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/26/2014 3:59 PM </created>
        public ActionResult RemoveCompareList(int id)
        {
            //gọi cookie
            HttpCookie cookierq = Request.Cookies["comparelist"];
            if (cookierq != null)
            {
                var wl = cookierq["compareproductids"].Split(',');
                cookierq["compareproductids"] = "";
                if (wl.Count() == 1)
                {
                    cookierq.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(cookierq);
                    return RedirectToAction("CompareList", "Content");
                }
                foreach (var item in wl)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        int productid = int.Parse(item);
                        if (productid != id)
                            cookierq["compareproductids"] = cookierq["compareproductids"] + (cookierq["compareproductids"] != "" ? "," : string.Empty) + productid.ToString();
                    }
                }
                Response.Cookies.Add(cookierq);
                cookierq.Expires = DateTime.Now.AddDays(20);
            }
            return RedirectToAction("CompareList", "Content");
        }

        #endregion

        public ActionResult SetCulture(string culture, string pathname)
        {

            var sp = pathname.Split('/');
            var fullslug = "";

            var slug = sp.Count() >= 3 ? sp[2] : "";
            if (!string.IsNullOrEmpty(slug))
            {
                var check = db.Slugs.FirstOrDefault(x => x.Slug_Full == slug);
                //nếu có slug
                if (check != null)
                {
                    // loại type
                    if (check.PostType != null)
                    {
                        var potstype = check.PostType;
                        var samelang = db.Slugs.FirstOrDefault(x => x.PostType == potstype & x.Lang == culture);
                        if (samelang != null)
                        {
                            fullslug = samelang.Slug_Full;
                        }
                    }
                    //loại cat
                    else if (check.Cat != null)
                    {
                        //trường hợp 1 nếu kik từ v=>a
                        if (check.Cat1.MainLang_Id == null)
                        {
                            var catid = check.Cat1.Id;
                            var catlang = db.Cats.FirstOrDefault(x => x.MainLang_Id == catid & x.Lang == culture);
                            if (catlang != null)
                            {
                                fullslug = catlang.Slugs.FirstOrDefault().Slug_Full;
                            }
                        }//từ a=>v
                        else
                        {
                            //tim cat gốc
                            var catId = check.Cat1.MainLang_Id;
                            var catroot = db.Cats.FirstOrDefault(x => x.Id == catId);
                            if (culture == "vi")
                            {
                                fullslug = catroot.Slugs.FirstOrDefault().Slug_Full;
                            }
                            else//nếu là 1 lang khác vd :tiếng trung
                            {
                                var catlang = db.Cats.FirstOrDefault(x => x.MainLang_Id == catId & x.Lang == culture);
                                if (catlang != null)
                                {
                                    fullslug = catlang.Slugs.FirstOrDefault().Slug_Full;
                                }
                            }
                        }

                    }//loại post
                    else if (check.Post != null)
                    {
                        //trường hợp 1 nếu kik từ v=>a
                        if (check.Post1.MainLang_Id == null)
                        {
                            var postid = check.Post1.Id;
                            var postlang = db.Posts.FirstOrDefault(x => x.MainLang_Id == postid & x.Lang == culture);
                            if (postlang != null)
                            {
                                fullslug = postlang.Slugs.FirstOrDefault().Slug_Full;
                            }
                        }//từ a=>v
                        else
                        {
                            //tim post gốc
                            var PostId = check.Post1.MainLang_Id;
                            var postroot = db.Posts.FirstOrDefault(x => x.Id == PostId);
                            if (culture == "vi")
                            {
                                fullslug = postroot.Slugs.FirstOrDefault().Slug_Full;
                            }
                            else//nếu là 1 lang khác vd :tiếng trung
                            {
                                var postlang = db.Posts.FirstOrDefault(x => x.MainLang_Id == PostId & x.Lang == culture);
                                if (postlang != null)
                                {
                                    fullslug = postlang.Slugs.FirstOrDefault().Slug_Full;
                                }
                            }
                        }

                    }
                    string url = "/" + culture + "/" + fullslug;
                    return Json(new { url = url });
                }
                else
                {
                    return Json(new { url = "/" + culture + "/" + slug });
                }
            }

            return Json(new { url = "/" + culture + "/" + slug });
        }

        #region CommentRating

        public ActionResult CommentRating(int postid)
        {
            ViewBag.postid = postid;
            return PartialView("_CommentRating");
        }
        public ActionResult CommentForm(Comment comment)
        {
            if (ModelState.IsValid)
            {
                if (comment.Product_Rate == 0)
                {
                    return Json(new { succsess = "errorRate" });
                }
                // comment.CreateDate = DateTime.Now;
                db.Comments.Add(comment);
                db.SaveChanges();
                return Json(new { succsess = true });
            }
            return Json(new { succsess = false });
        }
        #endregion

        /// <summary>
        /// Products the grid.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="slug1">The slug1.</param>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 10/16/2014 1:39 PM </created>
        public ActionResult ProductGrid(string culture, int? page, string slug, decimal? minprice, decimal? maxprice, int? manuft, string attributies, int? percent)
        {
            //percent
            int minpercent = 0;
            int maxpercent = 0;
            switch (percent)
            {
                case 10:
                    minpercent = 0;
                    maxpercent = 10;
                    break;
                case 20:
                    minpercent = 10;
                    maxpercent = 20;
                    break;
                case 30:
                    minpercent = 20;
                    maxpercent = 30;
                    break;
                case 40:
                    minpercent = 30;
                    maxpercent = 50;
                    break;
                case 50:
                    minpercent = 50;
                    maxpercent = 100;
                    break;
            }
            //end percent

            var min = db.Posts.Where(x => x.Active).OrderBy(x => x.Price).FirstOrDefault();
            var max = db.Posts.Where(x => x.Active).OrderByDescending(x => x.Price).FirstOrDefault();
            if (minprice == null)
                minprice = 0;
            if (maxprice == null)
                maxprice = 0;

            if (min != null)
                minprice = minprice != 0 ? minprice : (min.Price ?? 0);
            if (max != null)
                maxprice = maxprice != 0 ? maxprice : (max.Price ?? 0);

            var count = Request.Url.Segments.Count();
            if (Request.Url.Segments[count - 1] != "ProductGrid")
            {
                slug = Request.Url.Segments[count - 1];
            }
            ViewBag.percent = percent;
            ViewBag.manuft = manuft;
            ViewBag.slug = slug;
            ViewBag.min = minprice;
            ViewBag.max = maxprice;
            ViewBag.att = attributies;
            //nếu chuyên mục tiếng anh lấy các bài viết của cm tiếng việt load lên
            //var cat = db.Cats.FirstOrDefault(x => x.Slugs.FirstOrDefault(z => z.Slug_Full == slug) != null);

            var fullslug = db.Slugs.FirstOrDefault(x => x.Slug_Full == slug);// && x.Lang == culture); slug chưa lưu 2nn
            var cat = db.Cats.Find(fullslug.Cat);
            if (cat != null && cat.PostType.Has_Parallel_Language && cat.MainLang_Id != null)
            {
                var cattemp = cat;
                cat = db.Cats.Find(cattemp.MainLang_Id);
                //đổi slug của tiếng việt
                slug = cat.Slugs.FirstOrDefault().Slug_Full;
            }

            int pageSize = 28;
            int pageNumber = (page ?? 1);

            if (fullslug != null)
            {
                //nếu thuộc tính khác null
                if (!string.IsNullOrEmpty(attributies))
                {
                    var s = attributies.Split(',');
                    //loại sản phẩm
                    if (fullslug.PostType != null)
                    {
                        var typelist = db.Posts.Where(x => x.Active
                              //  && (percent == null ? true : (minpercent < ((x.Price_Old - x.Price) / x.Price_Old * 100) && ((x.Price_Old - x.Price) / x.Price_Old * 100) <= maxpercent))
                              //  && (manuft == null ? true : (x.Product.Supplier_Id == manuft))
                              && x.Lang == culture && x.Type == fullslug.PostType
                              // && x.Price >= minprice && x.Price <= maxprice
                              )
                             .ToList();
                        foreach (var att in s)
                        {
                            int attId = int.Parse(att);
                            int groupId = db.OptionVariants.FirstOrDefault(x => x.Id == attId).GroupVariant_Id.Value;

                            typelist = typelist.Where(x => x.VariantAttributes.FirstOrDefault(z => z.VariantGroup_Id == groupId && z.Variants.FirstOrDefault(a => a.VariantOption_Id == attId) != null) != null).ToList();
                        }
                        return PartialView("_ProductGrid", typelist.OrderBy(x => x.Order).Distinct().ToPagedList(pageNumber, pageSize));
                    }

                    //loại chuyên mục khác (cat)
                    string catid = cat.Id.ToString();
                    var list = db.Posts.Where(x => x.Active
                        // && (percent == null ? true : (minpercent < ((x.Price_Old - x.Price) / x.Price_Old * 100) && ((x.Price_Old - x.Price) / x.Price_Old * 100) <= maxpercent))
                        // && (manuft == null ? true : (x.Product.Supplier_Id == manuft))
                        // && x.Price >= minprice && x.Price <= maxprice && x.Lang == culture
                        && x.Cats.FirstOrDefault(c => c.Id == cat.Id | c.Parent_Path.Contains(catid)) != null).ToList();
                    foreach (var att in s)
                    {
                        int attId = int.Parse(att);
                        int groupId = db.OptionVariants.FirstOrDefault(x => x.Id == attId).GroupVariant_Id.Value;

                        list = list.Where(x => x.VariantAttributes.FirstOrDefault(z => z.VariantGroup_Id == groupId && z.Variants.FirstOrDefault(a => a.VariantOption_Id == attId) != null) != null).ToList();
                    }
                    return PartialView("_ProductGrid", list.OrderBy(x => x.Order).Distinct().ToPagedList(pageNumber, pageSize));
                }
                else//nl ko có lọc thuộc tính.
                {
                    if (fullslug.PostType != null)
                    {
                        var ListPostType = db.Posts.Where(x => x.Active
                            // && (percent == null ? true : (minpercent < ((x.Price_Old - x.Price) / x.Price_Old * 100) && ((x.Price_Old - x.Price) / x.Price_Old * 100) <= maxpercent))
                            //  && (manuft == null ? true : (x.Product.Supplier_Id == manuft))
                            && x.Lang == culture && x.Type == fullslug.PostType
                           // && x.Price >= minprice && x.Price <= maxprice
                           ).AsEnumerable();

                        return PartialView("_ProductGrid", ListPostType.OrderBy(x => x.Order).ToList().ToPagedList(pageNumber, pageSize));
                    }
                    string catid = cat.Id.ToString();
                    var list = db.Posts.Where(x => x.Active
                        //&& (percent == null ? true : (minpercent < ((x.Price_Old - x.Price) / x.Price_Old * 100) && ((x.Price_Old - x.Price) / x.Price_Old * 100) <= maxpercent))
                        // && (manuft == null ? true : (x.Product.Supplier_Id == manuft))
                        // && x.Price >= minprice && x.Price <= maxprice && x.Lang == culture
                        && x.Cats.FirstOrDefault(c => c.Id == cat.Id | c.Parent_Path.Contains(catid)) != null);
                    return PartialView("_ProductGrid", list.OrderBy(x => x.Order).ToList().ToPagedList(pageNumber, pageSize));
                }
            }
            //var listft = db.Posts.Where(x => x.Active && x.Lang == culture && x.Cats.FirstOrDefault(z => z.Slugs.FirstOrDefault(c => c.Slug_Full == slug) != null) != null).ToList();
            //return PartialView("_ProductGrid", listft.OrderBy(x => x.Order).ToList().ToPagedList(pageNumber, pageSize));
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// lấy dữ liệu đỗ vào range và drop
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 10/17/2014 11:00 AM </created>
        public ActionResult Filter(string culture)
        {
            // drop nha cung cap
            ViewBag.manu = new SelectList(db.Manufacturers, "Id", "Name");

            var count = Request.Url.Segments.Count();
            var slug = Request.Url.Segments[count - 1];

            ViewBag.slug = slug;
            var fullslug = db.Slugs.FirstOrDefault(x => x.Slug_Full == slug && x.Lang == culture);
            if (fullslug != null)
            {
                if (fullslug.PostType != null)
                {
                    var ListPostType = db.Posts.Where(x => x.Active && x.Lang == culture && x.Type == fullslug.PostType).ToList();
                    return PartialView("_Filter", ListPostType);
                }
            }
            var list = db.Posts.Where(x => x.Active && x.Lang == culture && x.Cats.FirstOrDefault(z => z.Slugs.FirstOrDefault(c => c.Slug_Full == slug) != null) != null).ToList();

            return PartialView("_Filter", list);
            // return PartialView("_Filter");
        }

        /// <summary>
        /// Gets the likes FB.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 10/27/2014 5:23 PM </created>
        public ActionResult GetLikesFB(string url)
        {
            //lấy số like,comment
            var jsonString = new System.Net.WebClient().DownloadString("http://graph.facebook.com/?ids=" + url);
            //var values = JsonConvert.DeserializeObject(jsonString);
            return Content(jsonString);//
        }

        /// <summary>
        /// Updates the time.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 11/13/2014 2:26 PM </created>
        public JsonResult UpdateTime()//cập nhật hoạt động cuối của user
        {
            //check ol
            var username = User.Identity.Name;
            if (!string.IsNullOrEmpty(username))
            {
                var log = db.LoginHistories.OrderByDescending(x => x.TimeStart).FirstOrDefault(x => x.UserName == username);
                if (log != null)
                {
                    //hoạt động cuối
                    log.TimeEnd = DateTime.Now;
                    db.SaveChanges();
                }
            }
            return Json(new { result = "succsess", JsonRequestBehavior.AllowGet });
        }

        /// <summary>
        /// Admins the edit.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 3/3/2015 9:30 AM </created>
        public ActionResult AdminEdit(string menu, int id)
        {
            ViewBag.Id = id;
            ViewBag.menu = menu;
            return PartialView("_AdminEdit");
        }
        //public ActionResult LoadmoreProduct(int tweetcount, string lang)
        //{
        //    try
        //    {
        //        var post = db.Posts.Where(x => x.Active & x.PostType.Code == "product" & x.Lang == lang).OrderByDescending(x => x.Order).ThenByDescending(x => x.Date_Create);

        //        var count = post.Count();
        //        var countData = 6 * tweetcount; //it will exclude the previous 6 records
        //        var dataContainer = post.Skip(countData).Take(6).Select(a => new
        //        {
        //            Title = a.Title,
        //            Thumb = a.Thumb,
        //            Id = a.Id,
        //        });
        //        return Json(dataContainer, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { success = false, ex = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult LoadmoreProduct(int tweetcount, string lang, string slug)
        {
            //var count = Request.Url.Segments.Count();
            //if (Request.Url.Segments[count - 1] != "ProductGrid")
            //{
            //    slug = Request.Url.Segments[count - 1];
            //}
            ViewBag.slug = slug;

            //nếu chuyên mục tiếng anh lấy các bài viết của cm tiếng việt load lên
            //var cat = db.Cats.FirstOrDefault(x => x.Slugs.FirstOrDefault(z => z.Slug_Full == slug) != null);

            var fullslug = db.Slugs.FirstOrDefault(x => x.Slug_Full == slug);// && x.Lang == culture); slug chưa lưu 2nn
            var cat = db.Cats.Find(fullslug.Cat);
            if (cat != null && cat.PostType.Has_Parallel_Language && cat.MainLang_Id != null)
            {
                var cattemp = cat;
                cat = db.Cats.Find(cattemp.MainLang_Id);
                //đổi slug của tiếng việt
                slug = cat.Slugs.FirstOrDefault().Slug_Full;
            }

            if (fullslug != null)
            {
                if (fullslug.PostType != null)
                {
                    var ListPostType = db.Posts.Where(x => x.Active
                        && x.Lang == lang && x.Type == fullslug.PostType
                       ).AsEnumerable();

                    var countData = 6 * tweetcount; //it will exclude the previous 6 records
                    var dataContainer = ListPostType.Skip(countData).Take(6).Select(a => new
                    {
                        Title = a.Title,
                        Thumb = a.Thumb,
                        Id = a.Id,
                        Price=a.Price
                    });
                    return Json(dataContainer, JsonRequestBehavior.AllowGet);
                }
                string catid = cat.Id.ToString();
                var list = db.Posts.Where(x => x.Active
                    && x.Cats.FirstOrDefault(c => c.Id == cat.Id | c.Parent_Path.Contains(catid)) != null).AsEnumerable();

                var countData1 = 6 * tweetcount; //it will exclude the previous 6 records
                var dataContainer1 = list.Skip(countData1).Take(6).Select(a => new
                {
                    Title = a.Title,
                    Thumb = a.Thumb,
                    Id = a.Id,
                    Price = a.Price
                });
                return Json(dataContainer1, JsonRequestBehavior.AllowGet);
            }
            //var listft = db.Posts.Where(x => x.Active && x.Lang == culture && x.Cats.FirstOrDefault(z => z.Slugs.FirstOrDefault(c => c.Slug_Full == slug) != null) != null).ToList();
            //return PartialView("_ProductGrid", listft.OrderBy(x => x.Order).ToList().ToPagedList(pageNumber, pageSize));
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadmoreNews(int tweetcount, string lang)
        {
            try
            {
                var post = db.Posts.Where(x => x.Active & x.PostType.Code == "tin-tuc" & x.Lang == lang).OrderByDescending(x => x.Order).ThenByDescending(x => x.Date_Create);

                var count = post.Count();
                var countData = 6 * tweetcount; //it will exclude the previous 6 records
                var dataContainer = post.Skip(countData).Take(6).Select(a => new
                {
                    Title = a.Title,
                    Thumb = a.Thumb,
                    Id = a.Id,
                    Date_Create = a.Date_Create,
                    Description = a.Description,
                });
                return Json(dataContainer, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, ex = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Mobile

        /// <summary>
        /// Headers the mobile.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 3/27/2015 11:46 AM </created>
        public ActionResult HeaderMobile()
        {
            return PartialView("_HeaderMobile");
        }

        /// <summary>
        /// Menus the mobile.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="positioncode">The positioncode.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 3/27/2015 11:46 AM </created>
        public ActionResult MenuMobile(string culture, string positioncode)
        {
            var menus = db.MenuItems.Where(x => x.Menu1.MenuPosition1.Active == true && x.Menu1.MenuPosition1.Code == positioncode && x.Active == true && x.Menu1.Lang == culture).OrderBy(o => o.Order).ToList();
            // Moi position moi thi tao them 1 "if" va tra ve mot view moi
            if (positioncode == "menu-footer")
                return PartialView("_MenuFooterMobile", menus);

            return PartialView("_MenuMobile", menus);
        }

        /// <summary>
        /// Footers the mobile.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 3/27/2015 11:46 AM </created>
        public ActionResult FooterMobile()
        {
            return PartialView("_FooterMobile");
        }

        /// <summary>
        /// Products the grid mobile.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="page">The page.</param>
        /// <param name="slug">The slug.</param>
        /// <param name="minprice">The minprice.</param>
        /// <param name="maxprice">The maxprice.</param>
        /// <param name="manuft">The manuft.</param>
        /// <param name="attributies">The attributies.</param>
        /// <param name="percent">The percent.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 3/27/2015 11:46 AM </created>

        public ActionResult ProductGridMobile(string culture, int? page, string slug, decimal? minprice, decimal? maxprice, int? manuft, string attributies, int? percent)
        {
            //percent
            int minpercent = 0;
            int maxpercent = 0;
            switch (percent)
            {
                case 10:
                    minpercent = 0;
                    maxpercent = 10;
                    break;
                case 20:
                    minpercent = 10;
                    maxpercent = 20;
                    break;
                case 30:
                    minpercent = 20;
                    maxpercent = 30;
                    break;
                case 40:
                    minpercent = 30;
                    maxpercent = 50;
                    break;
                case 50:
                    minpercent = 50;
                    maxpercent = 100;
                    break;
            }
            //end percent

            var min = db.Posts.Where(x => x.Active).OrderBy(x => x.Price).FirstOrDefault();
            var max = db.Posts.Where(x => x.Active).OrderByDescending(x => x.Price).FirstOrDefault();
            if (minprice == null)
                minprice = 0;
            if (maxprice == null)
                maxprice = 0;

            if (min != null)
                minprice = minprice != 0 ? minprice : (min.Price ?? 0);
            if (max != null)
                maxprice = maxprice != 0 ? maxprice : (max.Price ?? 0);

            var count = Request.Url.Segments.Count();
            if (Request.Url.Segments[count - 1] != "ProductGridMobile")
            {
                slug = Request.Url.Segments[count - 1];
            }
            ViewBag.percent = percent;
            ViewBag.manuft = manuft;
            ViewBag.slug = slug;
            ViewBag.min = minprice;
            ViewBag.max = maxprice;
            ViewBag.att = attributies;
            var fullslug = db.Slugs.FirstOrDefault(x => x.Slug_Full == slug);// && x.Lang == culture); slug chưa lưu 2nn
            int pageSize = 20;
            int pageNumber = (page ?? 1);

            if (fullslug != null)
            {
                //nếu thuộc tính khác null
                if (!string.IsNullOrEmpty(attributies))
                {
                    var s = attributies.Split(',');
                    //loại sản phẩm
                    if (fullslug.PostType != null)
                    {
                        var typelist = db.Posts.Where(x => x.Active
                              && (percent == null ? true : (minpercent < ((x.Price_Old - x.Price) / x.Price_Old * 100) && ((x.Price_Old - x.Price) / x.Price_Old * 100) <= maxpercent))
                              && (manuft == null ? true : (x.Product.Supplier_Id == manuft))
                              && x.Lang == culture && x.Type == fullslug.PostType
                              && x.Price >= minprice && x.Price <= maxprice
                              )
                             .ToList();
                        foreach (var att in s)
                        {
                            int attId = int.Parse(att);
                            int groupId = db.OptionVariants.FirstOrDefault(x => x.Id == attId).GroupVariant_Id.Value;

                            typelist = typelist.Where(x => x.VariantAttributes.FirstOrDefault(z => z.VariantGroup_Id == groupId && z.Variants.FirstOrDefault(a => a.VariantOption_Id == attId) != null) != null).ToList();
                        }
                        return PartialView("_ProductGridMobile", typelist.OrderBy(x => x.Order).Distinct().ToPagedList(pageNumber, pageSize));
                    }
                    //loại chuyên mục khác (cat)
                    var cat = db.Cats.Find(fullslug.Cat);
                    string catid = cat.Id.ToString();
                    var list = db.Posts.Where(x => x.Active
                        && (percent == null ? true : (minpercent < ((x.Price_Old - x.Price) / x.Price_Old * 100) && ((x.Price_Old - x.Price) / x.Price_Old * 100) <= maxpercent))
                        && (manuft == null ? true : (x.Product.Supplier_Id == manuft))
                        && x.Price >= minprice && x.Price <= maxprice && x.Lang == culture
                         && x.Cats.FirstOrDefault(c => c.Id == cat.Id | c.Parent_Path.Contains(catid)) != null).ToList();
                    foreach (var att in s)
                    {
                        int attId = int.Parse(att);
                        int groupId = db.OptionVariants.FirstOrDefault(x => x.Id == attId).GroupVariant_Id.Value;

                        list = list.Where(x => x.VariantAttributes.FirstOrDefault(z => z.VariantGroup_Id == groupId && z.Variants.FirstOrDefault(a => a.VariantOption_Id == attId) != null) != null).ToList();
                    }
                    return PartialView("_ProductGridMobile", list.OrderBy(x => x.Order).Distinct().ToPagedList(pageNumber, pageSize));
                }
                else//nl ko có lọc thuộc tính.
                {
                    if (fullslug.PostType != null)
                    {
                        var ListPostType = db.Posts.Where(x => x.Active
                            && (percent == null ? true : (minpercent < ((x.Price_Old - x.Price) / x.Price_Old * 100) && ((x.Price_Old - x.Price) / x.Price_Old * 100) <= maxpercent))
                            && (manuft == null ? true : (x.Product.Supplier_Id == manuft))
                            && x.Lang == culture && x.Type == fullslug.PostType
                            && x.Price >= minprice && x.Price <= maxprice)
                            .AsEnumerable();
                        return PartialView("_ProductGridMobile", ListPostType.OrderBy(x => x.Order).ToList().ToPagedList(pageNumber, pageSize));
                    }
                    var cat = db.Cats.Find(fullslug.Cat);
                    string catid = cat.Id.ToString();
                    var list = db.Posts.Where(x => x.Active
                        && (percent == null ? true : (minpercent < ((x.Price_Old - x.Price) / x.Price_Old * 100) && ((x.Price_Old - x.Price) / x.Price_Old * 100) <= maxpercent))
                        && (manuft == null ? true : (x.Product.Supplier_Id == manuft))
                        && x.Price >= minprice && x.Price <= maxprice && x.Lang == culture
                         && x.Cats.FirstOrDefault(c => c.Id == cat.Id | c.Parent_Path.Contains(catid)) != null).ToList();
                    return PartialView("_ProductGridMobile", list.OrderBy(x => x.Order).ToList().ToPagedList(pageNumber, pageSize));
                }
            }
            var listft = db.Posts.Where(x => x.Active && x.Lang == culture && x.Cats.FirstOrDefault(z => z.Slugs.FirstOrDefault(c => c.Slug_Full == slug) != null) != null).ToList();
            return PartialView("_ProductGridMobile", listft.OrderBy(x => x.Order).ToList().ToPagedList(pageNumber, pageSize));
        }

        #endregion
    }
}
