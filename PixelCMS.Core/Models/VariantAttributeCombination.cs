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
    
    public partial class VariantAttributeCombination
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Attribute { get; set; }
        public int Quantity { get; set; }
        public bool AllowOutOfStockOrders { get; set; }
        public string SKU { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public string Gtin { get; set; }
        public decimal Price { get; set; }
    
        public virtual Post Post { get; set; }
    }
}
