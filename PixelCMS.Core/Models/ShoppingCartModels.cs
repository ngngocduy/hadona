using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelCMS.Core.Models
{
    public class ShoppingCartModels
    {
        // gio hang duoc luu tru trong session
        public ShoppingCart Cart { get; set; }
        public decimal FixedFee { get; set; }
        public decimal Total { get; set; }
    }
}