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
    
    public partial class Roles_PostType
    {
        public int Role_Id { get; set; }
        public int PostType_Id { get; set; }
        public bool View { get; set; }
        public bool Create { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool CatView { get; set; }
        public bool CatCreate { get; set; }
        public bool CatEdit { get; set; }
        public bool CatDelete { get; set; }
    
        public virtual webpages_Roles webpages_Roles { get; set; }
        public virtual PostType PostType { get; set; }
    }
}
