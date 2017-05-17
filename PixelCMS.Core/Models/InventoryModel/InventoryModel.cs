using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelCMS.Core.Models
{
    public class InventoryModel
    {
        public string ProductName { get; set; }
        public string Attribute { get; set; }
        public int Quantity { get; set; }
        public string SKU  { get; set; }

        //new
        public int Id { get; set; }
        public decimal Price { get; set; }
    }
}
