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
    
    public partial class ProductAttributeGroup
    {
        public ProductAttributeGroup()
        {
            this.ProductAttributes = new HashSet<ProductAttribute>();
        }
    
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> Order { get; set; }
    
        public virtual ICollection<ProductAttribute> ProductAttributes { get; set; }
    }
}
