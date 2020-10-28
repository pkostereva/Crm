namespace CRM.Core.ConfigurationOptions
{
    public class PayPalOptions : IPayPalOptions
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
    }
}
