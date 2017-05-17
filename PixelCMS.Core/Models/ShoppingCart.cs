using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PixelCMS.Mailer;


//using PixelCMS.Core.Models;
//using PixelCMS = PixelCMS.Core.Models.pixelcmsEntities;

namespace PixelCMS.Core.Models
{
    public class ShoppingCartItem
    {
        public int PostId { get; set; }
        public string Slug { get; set; }
        public string PostTitle { get; set; }
        public string PostImage { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string SKU { get; set; }
        public string lang { get; set; }
        // biến thể
        public string Attribute { get; set; }
        public string VariantName { get; set; }
        public decimal VariantWeight { get; set; }
    }

    public class ShoppingCart
    {
        pixelcmsEntities db = new pixelcmsEntities();
        public ShoppingCart()
        {
            ListItem = new List<ShoppingCartItem>();
        }

        public List<ShoppingCartItem> ListItem { get; set; }

        public bool AddToCart(ShoppingCartItem item)
        {
            //xét biến thể nếu có
            if (item.Attribute != null)
            {
                //kiểm tra đã tồn tại trong giỏ hàng chưa nếu có thì tăng số lượng
                var checkvariant = ListItem.Any(x => x.Attribute == item.Attribute);
                if (checkvariant)
                {
                    ShoppingCartItem existsItem = ListItem.Where(x => x.Attribute == item.Attribute).SingleOrDefault();
                    if (existsItem != null)
                    {
                        existsItem.Quantity = existsItem.Quantity + 1;
                        existsItem.Total = existsItem.Quantity * (existsItem.Price);
                    }
                }
                else
                {
                    item.Total = item.Price;
                    ListItem.Add(item);

                }
            }
            else//không có biến thể add vào giỏ/
            {
                //kiểm tra trùng sp
                bool alreadyExists = ListItem.Any(x => x.PostId == item.PostId && x.Attribute == null);
                if (alreadyExists)
                {
                    ShoppingCartItem existsItem = ListItem.Where(x => x.PostId == item.PostId && x.Attribute == null).SingleOrDefault();
                    if (existsItem != null)
                    {
                        existsItem.Quantity = existsItem.Quantity + 1;
                        existsItem.Total = existsItem.Quantity * existsItem.Price;
                    }
                }
                else
                {
                    item.Total = item.Price;
                    ListItem.Add(item);

                }
            }

            return true;
        }

        public bool RemoveFromCart(long lngProductSellID)
        {
            ShoppingCartItem existsItem = ListItem.Where(x => x.PostId == lngProductSellID).SingleOrDefault();
            if (existsItem != null)
            {
                ListItem.Remove(existsItem);
            }
            return true;
        }
        public bool RemoveFromCartVariant(string lngProductSellID)
        {
            ShoppingCartItem existsItem = ListItem.Where(x => x.Attribute == lngProductSellID).SingleOrDefault();
            if (existsItem != null)
            {
                ListItem.Remove(existsItem);
            }
            return true;
        }


        public bool UpdateQuantity(long lngProductSellID, int intQuantity, string Attribute)
        {
            ShoppingCartItem existsItem = new ShoppingCartItem();
            if (string.IsNullOrEmpty(Attribute))//nếu ko có biến thể
            {
                existsItem = ListItem.Where(x => x.PostId == lngProductSellID).SingleOrDefault();
            }
            else//nếu có biến thể
            {
                existsItem = ListItem.Where(x => x.PostId == lngProductSellID && x.Attribute == Attribute).SingleOrDefault();
            }

            if (existsItem != null)
            {
                existsItem.Quantity = intQuantity;
                existsItem.Total = existsItem.Quantity * existsItem.Price;
            }

            return true;
        }

        public decimal GetTotal()
        {
            return ListItem.Sum(x => x.Total);
        }

        public int GetCount()
        {
            int count = 0;
            foreach (var item in ListItem)
            {
                count = count + item.Quantity;
            }
            return count;
        }

        public bool EmptyCart()
        {
            ListItem.Clear();
            return true;
        }

        public int CreateOrder(Order order, int districtId)
        {
            db.Orders.Add(order);
            order.Date_Add = DateTime.Now.Date;
            //lưu tình trạng thanh toán onepay = 1(chưa thanh toán)
            order.PaymentStatus = 1;
            if (db.Order_Status.FirstOrDefault() != null)
            {
                order.Status_Id = db.Order_Status.OrderBy(x => x.Order).First().Id;
            }
            db.SaveChanges();
            decimal orderTotal = 0;
            var cartItems = ListItem.ToList();
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                //- số lượng trong kho
                var variantCB = db.VariantAttributeCombinations.FirstOrDefault(x => x.Attribute == item.Attribute);
                var product = db.Posts.Find(item.PostId);
                // xét biến thể trước
                if (variantCB != null)
                {
                    variantCB.Quantity = variantCB.Quantity - item.Quantity;
                }//- trong kho ko biến thể
                else if (product!=null&&product.Product!=null)
                {
                    var inven = product.Product.Inventories.FirstOrDefault();
                    if (inven!=null)
                    {
                        inven.Quantity = inven.Quantity - item.Quantity;
                    }
                }

                if (item.Price == null)
                {
                    item.Price = 0;
                }
                var orderDetail = new Order_Details()
                {
                    Product_Id = item.PostId,
                   // Attribute = item.Attribute,
                    Attribute=item.VariantName,
                    Product_Name = item.PostTitle,
                    Order_Id = order.Id,
                    Unit_Price = item.Price,
                    Quantity = item.Quantity,
                    SKU = item.SKU,
                    Image = item.PostImage
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Quantity * item.Price);
                db.Order_Details.Add(orderDetail);
            }
            var o = db.Orders.FirstOrDefault(x => x.Id == order.Id);
            var ship = db.Shippings.OrderByDescending(z => z.CreateDate).FirstOrDefault(x => x.District_Id == districtId);
            // Set the order's total to the orderTotal count
            if (o != null)
            {
                //lưu total + với giá vận chuyển
                if (ship != null)
                {
                    if (ship.District_Id != null)
                    {
                        o.Total = orderTotal + ship.FixedFee;
                        o.ShipFee = ship.FixedFee;
                    }
                    else
                    {
                        o.Total = orderTotal;
                    }
                }
                else
                {
                    o.Total = orderTotal;
                }
            }

            //send mail cho admin
            UserMailer _UserMailer = new UserMailer();
            var namewebsite = db.Options.FirstOrDefault(s => s.Code == "common-sitename");
            //email admin
            var emails = db.Options.FirstOrDefault(s => s.Code == "email-mail");
            var emailsetting = "luanpb@thekymoi.com";
            if (emails != null)
            {
                emailsetting = emails.Value;
            }
            string str = "<table class='orderdetail' style='width: 100%;'><th>Tên sản phẩm</th><th>Thuộc tính</th><th>Số lượng</th><th>Đơn giá</th><th>Tổng cộng</th>";
            foreach (var item in ListItem)
            {
                str = str + "<tr><td style='padding-top: 10px;'>" + item.PostTitle + "</td><td style='padding-top: 10px;'>" + item.VariantName + "</td><td style='padding-top: 10px;'>" + item.Quantity + "</td><td style='padding-top: 10px;'>" + item.Price + "</td><td style='padding-top: 10px;'>" + item.Quantity * item.Price + "</td></tr>";
            }
            str = str + "<tr><td></td><td></td><td></td><td></td><td style='font-weight: bold;'>Tổng tiền:" + string.Format("{0:0,000}", GetTotal()) + "</td></tr></table>";
           //Gửi quản trị
            _UserMailer.SendMailorders(emailsetting, order.Name, order.Email, "Khách đặt hàng từ website", str, order.Email, order.Phone, order.Address,order.Note);

            //gửi cho khách
            _UserMailer.SendMailorders(order.Email, order.Name, order.Email, namewebsite != null ? namewebsite.Value : "", str, emailsetting, order.Phone, order.Address, order.Note);
            // Save the order
            db.SaveChanges();

            // Return the OrderId as the confirmation number
            return order.Id;
        }
    }
}