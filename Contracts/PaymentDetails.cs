using System;
using Newtonsoft.Json;

namespace PaymentReconciliation.Contracts
{
    public partial class PaymentDetails
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("amount")]
        public Amount Amount { get; set; }

        [JsonProperty("final_capture")]
        public bool FinalCapture { get; set; }

        [JsonProperty("seller_protection")]
        public SellerProtectionPaymentDetails SellerProtection { get; set; }

        [JsonProperty("seller_receivable_breakdown")]
        public SellerReceivableBreakdown SellerReceivableBreakdown { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("create_time")]
        public DateTimeOffset CreateTime { get; set; }

        [JsonProperty("update_time")]
        public DateTimeOffset UpdateTime { get; set; }

        [JsonProperty("links")]
        public LinkPaymentDetails[] Links { get; set; }
    }

    public partial class AmountPaymentDetails
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class LinkPaymentDetails
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }

        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }
    }

    public partial class SellerProtectionPaymentDetails
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("dispute_categories")]
        public string[] DisputeCategories { get; set; }
    }

    public partial class SellerReceivableBreakdown
    {
        [JsonProperty("gross_amount")]
        public AmountPaymentDetails GrossAmount { get; set; }

        [JsonProperty("paypal_fee")]
        public AmountPaymentDetails PaypalFee { get; set; }

        [JsonProperty("net_amount")]
        public AmountPaymentDetails NetAmount { get; set; }
    }
}
