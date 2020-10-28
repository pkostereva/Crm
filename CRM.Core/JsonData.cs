using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CRM.Core
{
    public class JsonData
    {
        public string intent { get; set; }

        public Payer payer { get; set; }

        public Transaction[] transactions { get; set; }

        //[JsonProperty("note_to_payer")]
        //public string NoteToPayer { get; set; }

        public RedirectUrls redirect_urls { get; set; }
    }

    public partial class Payer
    {
        public string payment_method { get; set; }
    }

    public partial class RedirectUrls
    {
        public string return_url { get; set; }

        public string cancel_url { get; set; }
    }

    public partial class Transaction
    {
        public Amount amount { get; set; }

        //[JsonProperty("description")]
        //public string Description { get; set; }

        //[JsonProperty("custom")]
        //public string Custom { get; set; }

        //[JsonProperty("invoice_number")]
        //public string InvoiceNumber { get; set; }

        //[JsonProperty("payment_options")]
        //public PaymentOptions PaymentOptions { get; set; }

        //[JsonProperty("soft_descriptor")]
        //public string SoftDescriptor { get; set; }

        //[JsonProperty("item_list")]
        //public ItemList ItemList { get; set; }
    }

    public partial class Amount
    {
        public string total { get; set; }

        public string currency { get; set; }

        //[JsonProperty("details")]
        //public Details Details { get; set; }
    }

    public partial class Details
    {
        [JsonProperty("subtotal")]
        public string Subtotal { get; set; }

        [JsonProperty("tax")]
        public string Tax { get; set; }

        [JsonProperty("shipping")]
        public string Shipping { get; set; }

        [JsonProperty("handling_fee")]
        public string HandlingFee { get; set; }

        [JsonProperty("shipping_discount")]
        public string ShippingDiscount { get; set; }

        [JsonProperty("insurance")]
        public string Insurance { get; set; }
    }

    public partial class ItemList
    {
        [JsonProperty("items")]
        public Item[] Items { get; set; }

        [JsonProperty("shipping_address")]
        public ShippingAddress ShippingAddress { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("quantity")]
        [JsonConverter(typeof(StringEnumConverter))]
        public long Quantity { get; set; }

        [JsonProperty("price")]
        [JsonConverter(typeof(StringEnumConverter))]
        public long Price { get; set; }

        [JsonProperty("tax")]
        public string Tax { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public partial class ShippingAddress
    {
        [JsonProperty("recipient_name")]
        public string RecipientName { get; set; }

        [JsonProperty("line1")]
        public string Line1 { get; set; }

        [JsonProperty("line2")]
        public string Line2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("postal_code")]
        [JsonConverter(typeof(StringEnumConverter))]
        public long PostalCode { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    public partial class PaymentOptions
    {
        [JsonProperty("allowed_payment_method")]
        public string AllowedPaymentMethod { get; set; }
    }
}
