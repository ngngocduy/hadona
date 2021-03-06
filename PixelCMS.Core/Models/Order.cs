//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PixelCMS.Core.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        public Order()
        {
            this.Order_Details = new HashSet<Order_Details>();
        }
    
        public int Id { get; set; }
        public string Order_Code { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Shipping_Name { get; set; }
        public string Shipping_Email { get; set; }
        public string Shipping_Phone { get; set; }
        public string Shipping_Address { get; set; }
        public string Note { get; set; }
        public Nullable<int> Coupon_Id { get; set; }
        public System.DateTime Date_Add { get; set; }
        public Nullable<System.DateTime> Date_Modified { get; set; }
        public Nullable<decimal> ShipFee { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<int> Status_Id { get; set; }
        public Nullable<int> PaymentStatus { get; set; }
        public bool View { get; set; }
        public bool IsDestroy { get; set; }
    
        public virtual Order_Status Order_Status { get; set; }
        public virtual ICollection<Order_Details> Order_Details { get; set; }
    }
}
