namespace CRM.Core.ConfigurationOptions
{
    public interface IPayPalOptions
    {
        string ClientId { get; set; }
        string Secret { get; set; }
    }
}