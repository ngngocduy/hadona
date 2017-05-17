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
    
    public partial class PostType_Name
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lang { get; set; }
        public Nullable<int> Posttype_id { get; set; }
        public Nullable<int> Attribute_id { get; set; }
        public Nullable<int> Option_id { get; set; }
        public Nullable<int> OptionGroup_id { get; set; }
        public Nullable<int> AttributeGroup_id { get; set; }
    
        public virtual AttributeGroup AttributeGroup { get; set; }
        public virtual OptionGroup OptionGroup { get; set; }
        public virtual PostAttribute PostAttribute { get; set; }
        public virtual Option Option { get; set; }
        public virtual PostType PostType { get; set; }
    }
}
