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
    
    public partial class District
    {
        public District()
        {
            this.Addresses = new HashSet<Address>();
            this.Shippings = new HashSet<Shipping>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> City_Id { get; set; }
    
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<Shipping> Shippings { get; set; }
    }
}
