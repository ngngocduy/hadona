using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PixelCMS.Core.Models
{
    public partial class VariantAttribute
    {
        public List<SelectListItem> TypeList { get; set; }

        public string TypeText
        {
            get
            {
                switch (Type)
                {
                    case 1:
                        return "Ô màu sắc";
                    case 2:
                        return "Ô check(Cho chọn nhiều)";
                    case 3:
                        return "Ô tròn radio(Cho chọn 1)";
                    case 5:
                        return "Ô vuông radio(Cho chọn 1)";
                    case 4:
                        return "Danh sách đổ xuống";
                }
                return string.Empty;
            }
        }
    }
}
