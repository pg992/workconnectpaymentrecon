using System;
using Newtonsoft.Json;

namespace PaymentReconciliation.Contracts
{
    public partial class Payments
    {
        [JsonProperty("create_time")]
        public DateTimeOffset CreateTime { get; set; }

        [JsonProperty("update_time")]
        public DateTimeOffset UpdateTime { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("intent")]
        public string Intent { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("payer")]
        public Payer Payer { get; set; }

        [JsonProperty("purchase_units")]
        public PurchaseUnit[] PurchaseUnits { get; set; }

        [JsonProperty("links")]
        public Link[] Links { get; set; }
    }

    public partial class Link
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }

        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public partial class Payer
    {
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }

        [JsonProperty("payer_id")]
        public string PayerId { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }
    }

    public partial class Address
    {
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
    }

    public partial class Name
    {
        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }
    }

    public partial class PurchaseUnit
    {
        [JsonProperty("reference_id")]
        public string ReferenceId { get; set; }

        [JsonProperty("invoice_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long InvoiceId { get; set; }

        [JsonProperty("amount")]
        public Amount Amount { get; set; }

        [JsonProperty("payee")]
        public Payee Payee { get; set; }

        [JsonProperty("payments")]
        public PaymentsClass Payments { get; set; }
    }

    public partial class Amount
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }
    }

    public partial class Payee
    {
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }

        [JsonProperty("merchant_id")]
        public string MerchantId { get; set; }
    }

    public partial class PaymentsClass
    {
        [JsonProperty("captures")]
        public Capture[] Captures { get; set; }
    }

    public partial class Capture
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("invoice_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long InvoiceId { get; set; }

        [JsonProperty("final_capture")]
        public bool FinalCapture { get; set; }

        [JsonProperty("create_time")]
        public DateTimeOffset CreateTime { get; set; }

        [JsonProperty("update_time")]
        public DateTimeOffset UpdateTime { get; set; }

        [JsonProperty("amount")]
        public Amount Amount { get; set; }

        [JsonProperty("seller_protection")]
        public SellerProtection SellerProtection { get; set; }

        [JsonProperty("links")]
        public Link[] Links { get; set; }
    }

    public partial class SellerProtection
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
