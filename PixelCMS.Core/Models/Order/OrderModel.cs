using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixelCMS.Core.Models
{
    public partial class Order
    {
        public string PaymentText
        {
            get
            {
                switch (PaymentStatus)
                {
                    case 1:
                        return "Chưa thanh toán";
                    case 2:
                        return "Đã thanh toán OnePay";
                    case 3:
                        return "Đang chờ";
                    case 4:
                        return "Đã thanh toán Ngân Lượng";
                }
                return string.Empty;
            }
        }
        public string StatusText
        {
            get
            {
                switch (Status_Id)
                {
                    case 1:
                        return "Đang chờ";
                    case 2:
                        return "Đang xử lý";
                    case 3:
                        return "Đang giao hàng";
                    case 4:
                        return "Hoàn thành";
                    case 5:
                        return "Hủy";
                }
                return string.Empty;
            }
        }
    }
}