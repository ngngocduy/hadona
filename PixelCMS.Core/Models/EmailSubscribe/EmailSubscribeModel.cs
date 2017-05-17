using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
namespace PixelCMS.Core.Models
{
   public class EmailSubscribeModel
    {
        public string Email { get; set; }

        public DateTime Date { get; set; }

        //public bool Active { get; set; }
    }
}
