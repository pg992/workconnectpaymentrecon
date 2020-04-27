using Newtonsoft.Json;
using System;

namespace PaymentReconciliation.Contracts
{
    public partial class TransactionDetails
    {
        [JsonProperty("transaction_details")]
        public TransactionDetail[] TransactionDetailsTransactionDetails { get; set; }

        [JsonProperty("account_number")]
        public string AccountNumber { get; set; }

        [JsonProperty("start_date")]
        public string StartDate { get; set; }

        [JsonProperty("end_date")]
        public string EndDate { get; set; }

        [JsonProperty("last_refreshed_datetime")]
        public string LastRefreshedDatetime { get; set; }

        [JsonProperty("page")]
        public long Page { get; set; }

        [JsonProperty("total_items")]
        public long TotalItems { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("LinkTransactionDetailss")]
        public LinkTransactionDetails[] LinkTransactionDetailss { get; set; }
    }

    public partial class LinkTransactionDetails
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }

        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }
    }

    public partial class TransactionDetail
    {
        [JsonProperty("transaction_info")]
        public TransactionInfo TransactionInfo { get; set; }

        [JsonProperty("payer_info")]
        public PayerInfo PayerInfo { get; set; }

        [JsonProperty("shipping_info")]
        public ShippingInfo ShippingInfo { get; set; }

        [JsonProperty("cart_info")]
        public CartInfo CartInfo { get; set; }

        [JsonProperty("store_info")]
        public Info StoreInfo { get; set; }

        [JsonProperty("auction_info")]
        public Info AuctionInfo { get; set; }

        [JsonProperty("incentive_info")]
        public Info IncentiveInfo { get; set; }
    }

    public partial class Info
    {
    }

    public partial class CartInfo
    {
        [JsonProperty("item_details", NullValueHandling = NullValueHandling.Ignore)]
        public ItemDetail[] ItemDetails { get; set; }
    }

    public partial class ItemDetail
    {
        [JsonProperty("item_quantity")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ItemQuantity { get; set; }

        [JsonProperty("item_unit_price")]
        public AvailableBalance ItemUnitPrice { get; set; }

        [JsonProperty("item_amount")]
        public AvailableBalance ItemAmount { get; set; }

        [JsonProperty("total_item_amount")]
        public AvailableBalance TotalItemAmount { get; set; }
    }

    public partial class AvailableBalance
    {
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class PayerInfo
    {
        [JsonProperty("address_status")]
        public string AddressStatus { get; set; }

        [JsonProperty("payer_name")]
        public PayerName PayerName { get; set; }

        [JsonProperty("account_id", NullValueHandling = NullValueHandling.Ignore)]
        public string AccountId { get; set; }

        [JsonProperty("email_address", NullValueHandling = NullValueHandling.Ignore)]
        public string EmailAddress { get; set; }

        [JsonProperty("payer_status", NullValueHandling = NullValueHandling.Ignore)]
        public string PayerStatus { get; set; }

        [JsonProperty("country_code", NullValueHandling = NullValueHandling.Ignore)]
        public string CountryCode { get; set; }
    }

    public partial class PayerName
    {
        [JsonProperty("given_name", NullValueHandling = NullValueHandling.Ignore)]
        public string GivenName { get; set; }

        [JsonProperty("surname", NullValueHandling = NullValueHandling.Ignore)]
        public string Surname { get; set; }

        [JsonProperty("alternate_full_name", NullValueHandling = NullValueHandling.Ignore)]
        public string AlternateFullName { get; set; }
    }

    public partial class ShippingInfo
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
        public AddressTransactionDetails Address { get; set; }
    }

    public partial class AddressTransactionDetails
    {
        [JsonProperty("line1")]
        public string Line1 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("postal_code")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long PostalCode { get; set; }
    }

    public partial class TransactionInfo
    {
        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("transaction_event_code")]
        public string TransactionEventCode { get; set; }

        [JsonProperty("transaction_initiation_date")]
        public string TransactionInitiationDate { get; set; }

        [JsonProperty("transaction_updated_date")]
        public string TransactionUpdatedDate { get; set; }

        [JsonProperty("transaction_amount")]
        public AvailableBalance TransactionAmount { get; set; }

        [JsonProperty("transaction_status")]
        public string TransactionStatus { get; set; }

        [JsonProperty("transaction_subject", NullValueHandling = NullValueHandling.Ignore)]
        public string TransactionSubject { get; set; }

        [JsonProperty("ending_balance")]
        public AvailableBalance EndingBalance { get; set; }

        [JsonProperty("available_balance")]
        public AvailableBalance AvailableBalance { get; set; }

        [JsonProperty("protection_eligibility")]
        public string ProtectionEligibility { get; set; }

        [JsonProperty("paypal_account_id", NullValueHandling = NullValueHandling.Ignore)]
        public string PaypalAccountId { get; set; }

        [JsonProperty("fee_amount", NullValueHandling = NullValueHandling.Ignore)]
        public AvailableBalance FeeAmount { get; set; }
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t)
        {
            return t == typeof(long) || t == typeof(long?);
        }

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
