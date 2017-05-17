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
    
    public partial class MenuItem
    {
        public int Id { get; set; }
        public Nullable<int> Menu { get; set; }
        public string Name { get; set; }
        public Nullable<int> Type { get; set; }
        public string Url { get; set; }
        public Nullable<int> Post { get; set; }
        public Nullable<int> Cat { get; set; }
        public Nullable<int> Order { get; set; }
        public Nullable<int> Level { get; set; }
        public Nullable<int> Owner_Id { get; set; }
        public bool Active { get; set; }
        public string Field_1 { get; set; }
        public string Field_2 { get; set; }
        public string Field_3 { get; set; }
        public Nullable<int> PostType { get; set; }
    
        public virtual Menu Menu1 { get; set; }
        public virtual Post Post1 { get; set; }
        public virtual PostType PostType1 { get; set; }
        public virtual Cat Cat1 { get; set; }
    }
}
