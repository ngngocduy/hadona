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
    
    public partial class Inventory
    {
        public int Product_Id { get; set; }
        public int Agent_Id { get; set; }
        public int Quantity { get; set; }
    
        public virtual Agent Agent { get; set; }
        public virtual Product Product { get; set; }
    }
}
