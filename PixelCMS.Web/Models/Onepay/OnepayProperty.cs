using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcWithOnepay.Models
{
    public class OnepayProperty
    {
        //tt tk test
        public const string HASH_CODE = "18D7EC3F36DF842B42E1AA729E4AB010";
            //"6D0870CDE5F24F34F3915FB0045120DB";

        public const string ACCESS_CODE = "614240F4";
            //"6BEB2546";

        public const string MERCHANT_ID = "TESTONEPAYUSD";
            //"TESTONEPAY";

        public const string URL_ONEPAY_TEST = "http://mtf.onepay.vn/vpcpay/vpcpay.op";

        public const string URL_ONEPAY_TRUTH = "http://onepay.vn/vpcpay/vpcpay.op";

        public const string COMMAND = "pay";

        public const string PAYGATE_LANGUAGE = "en";

        public const string VERSION = "2";

        public const string AGAIN_LINK = "onepay.vn";
    }
}