using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PixelCMS.Core.Models;

namespace PixelCMS.Web.Models.Order
{
    public class OrderModel
    {
        public OrderModel()
        {
            CityList = new List<SelectListItem>();
            DistrictList = new List<SelectListItem>();
            Shipping_CityList = new List<SelectListItem>();
            Shipping_DistrictList = new List<SelectListItem>();
        }

        [Required]
        public string Name { get; set; }

        public string Email { get; set; }
        [Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Shipping Name")]
        public string Shipping_Name { get; set; }

        //[Required]
        [Display(Name = "Shipping Email")]
        public string Shipping_Email { get; set; }

        [Required]
        [Display(Name = "Shipping Phone")]
        public string Shipping_Phone { get; set; }

        [Required]
        [Display(Name = "Shipping Address")]
        public string Shipping_Address { get; set; }

        public string Note { get; set; }

        //checkbox địa chỉ giao hàng
        public bool dcgh { get; set; }

        [Required]
        [Display(Name = "Tỉnh/TP")]
        public int CityId { get; set; }
        [Required]
        [Display(Name = "Quận/Huyện")]
        public int DistrictId { get; set; }

        //[Required]
        [Display(Name = "CityList")]
        public List<SelectListItem> CityList { get; set; }


        [Display(Name = "DistrictList")]
        public List<SelectListItem> DistrictList { get; set; }

        //ship
        [Required]
        public int Shipping_CityId { get; set; }
        [Required]
        public int Shipping_DistrictId { get; set; }

        [Display(Name = "Shipping CityList")]
        public List<SelectListItem> Shipping_CityList { get; set; }

        //[Required]
        [Display(Name = "Shipping DistrictList")]
        public List<SelectListItem> Shipping_DistrictList { get; set; }


    }
}