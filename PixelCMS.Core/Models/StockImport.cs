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
    
    public partial class StockImport
    {
        public int Id { get; set; }
        public string SKU { get; set; }
        public Nullable<int> ProductId { get; set; }
        public System.DateTime ImportDate { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> CurrentQuantity { get; set; }
    
        public virtual Product Product { get; set; }
    }
}