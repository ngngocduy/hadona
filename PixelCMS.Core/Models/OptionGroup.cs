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
    
    public partial class OptionGroup
    {
        public OptionGroup()
        {
            this.PostType_Name = new HashSet<PostType_Name>();
            this.Options = new HashSet<Option>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> Order { get; set; }
    
        public virtual ICollection<PostType_Name> PostType_Name { get; set; }
        public virtual ICollection<Option> Options { get; set; }
    }
}
