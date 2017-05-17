using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelCMS.Core.Models
{
    public class ShippingModel
    {
        public ShippingModel()
        {
            CityList = new List<SelectListItem>();
            DistrictList = new List<SelectListItem>();
        }
        public int Id { get; set; }

        [Display(Name = "Phí cố định")]
        public decimal FixedFee { get; set; }

        public decimal MinWeight { get; set; }

        public decimal MaxWeight { get; set; }

        [Display(Name = "Phí cho từng đơn vị")]
        public decimal FeeOfUnit { get; set; }

        //[Required]
        [Display(Name = "Tỉnh/TP")]
        public int CityId { get; set; }

        [Required]
        [Display(Name = "Quận/Huyện")]
        public int? DistrictId { get; set; }

        [Display(Name = "CityList")]
        public List<SelectListItem> CityList { get; set; }

        [Display(Name = "DistrictList")]
        public List<SelectListItem> DistrictList { get; set; }

        public string selectedItem { get; set; }
    }
}