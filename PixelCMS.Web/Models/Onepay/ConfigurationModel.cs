
namespace Nop.Plugin.Payments.OnePay.Models
{
    public class ConfigurationModel
    {
        public string SecureHash { get; set; }

        public string AccessCode { get; set; }

        // public string virtualPaymentClientURL { get; set; }
        public string URL_ONEPAY_TEST { get; set; }

        public string URL_ONEPAY_TRUTH { get; set; }

        public string Command { get; set; }

        public string MerchantId { get; set; }

        public string Language { get; set; }

        public int Version { get; set; }

        public string AgainLink { get; set; }
        //
        public string MerchTxnRef { get; set; }

        public string OrderInfo { get; set; }

        public string Amount { get; set; }

        public string Locale { get; set; }

        public string Currency { get; set; }

        public string TicketNo { get; set; }

        public string ReturnURL { get; set; }

        public string Title { get; set; }

        public decimal AdditionalFee { get; set; }

        public bool AdditionalFeePercentage { get; set; }
    }
}