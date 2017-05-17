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
    
    public partial class Cat
    {
        public Cat()
        {
            this.Cat1 = new HashSet<Cat>();
            this.MenuItems = new HashSet<MenuItem>();
            this.Slugs = new HashSet<Slug>();
            this.Posts = new HashSet<Post>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<int> Owner_Id { get; set; }
        public Nullable<int> Level { get; set; }
        public Nullable<int> Order { get; set; }
        public string Meta_Title { get; set; }
        public string Meta_Key { get; set; }
        public string Meta_Description { get; set; }
        public string Html_Head { get; set; }
        public string Image { get; set; }
        public bool Active { get; set; }
        public string Lang { get; set; }
        public Nullable<int> MainLang_Id { get; set; }
        public string Description { get; set; }
        public bool Feature { get; set; }
        public string Banner_Image { get; set; }
        public Nullable<int> Position { get; set; }
        public string Parent_Path { get; set; }
    
        public virtual ICollection<Cat> Cat1 { get; set; }
        public virtual Cat Cat2 { get; set; }
        public virtual PostType PostType { get; set; }
        public virtual ICollection<MenuItem> MenuItems { get; set; }
        public virtual ICollection<Slug> Slugs { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}