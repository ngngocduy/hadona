using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections;
using PixelCMS.Controllers;

public class NL_Checkout
{
    private String nganluong_url = "https://www.nganluong.vn/checkout.php";

    public string merchant_site_code
    {
        get
        {
            var config = new CommonController();
            return config.LoadOption("NL_merchant");
        }
    }
    public string secure_pass
    {
        get
        {
            var config = new CommonController();
            return config.LoadOption("NL_secure_pass");
        }
    }

    //private String merchant_site_code = "123456";	//thay mã merchant site mà b?n dã dang ký vào dây
    //  private String secure_pass = "7b80e03da8";	//thay m?t kh?u giao ti?p gi?a website c?a b?n v?i NgânLu?ng.vn mà b?n dã dang ký vào dây
    public String GetMD5Hash(String input)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();

        byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);

        bs = x.ComputeHash(bs);

        System.Text.StringBuilder s = new System.Text.StringBuilder();

        foreach (byte b in bs)
        {

            s.Append(b.ToString("x2").ToLower());

        }

        String md5String = s.ToString();

        return md5String;
    }

    public String buildCheckoutUrl(String return_url, String receiver, String transaction_info, String order_code, String price)
    {
        // T?o bi?n secure code
        String secure_code = "";

        secure_code += this.merchant_site_code;

        secure_code += " " + HttpUtility.UrlEncode(return_url).ToLower();

        secure_code += " " + receiver;

        secure_code += " " + transaction_info;

        secure_code += " " + order_code;

        secure_code += " " + price;

        secure_code += " " + this.secure_pass;

        // T?o m?ng bam
        Hashtable ht = new Hashtable();

        ht.Add("merchant_site_code", this.merchant_site_code);

        ht.Add("return_url", HttpUtility.UrlEncode(return_url).ToLower());

        ht.Add("receiver", receiver);

        ht.Add("transaction_info", transaction_info);

        ht.Add("order_code", order_code);

        ht.Add("price", price);

        ht.Add("secure_code", this.GetMD5Hash(secure_code));

        // T?o url redirect
        String redirect_url = this.nganluong_url;

        if (redirect_url.IndexOf("?") == -1)
        {
            redirect_url += "?";
        }
        else if (redirect_url.Substring(redirect_url.Length - 1, 1) != "?" && redirect_url.IndexOf("&") == -1)
        {
            redirect_url += "&";
        }

        String url = "";

        // Duy?t các ph?n t? trong m?ng bam ht1 d? t?o redirect url
        IDictionaryEnumerator en = ht.GetEnumerator();

        while (en.MoveNext())
        {
            if (url == "")
                url += en.Key.ToString() + "=" + en.Value.ToString();
            else
                url += "&" + en.Key.ToString() + "=" + en.Value.ToString();
        }

        String rdu = redirect_url + url;

        return rdu;
    }

    public Boolean verifyPaymentUrl(String transaction_info, String order_code, String price, String payment_id, String payment_type, String error_text, String secure_code)
    {
        // T?o mã xác th?c t? ch? web
        String str = "";

        str += " " + transaction_info;

        str += " " + order_code;

        str += " " + price;

        str += " " + payment_id;

        str += " " + payment_type;

        str += " " + error_text;

        str += " " + this.merchant_site_code;

        str += " " + this.secure_pass;

        // Mã hóa các tham s?
        String verify_secure_code = "";

        verify_secure_code = this.GetMD5Hash(str);

        // Xác th?c mã c?a ch? web v?i mã tr? v? t? nganluong.vn
        if (verify_secure_code == secure_code) return true;

        return false;
    }

}