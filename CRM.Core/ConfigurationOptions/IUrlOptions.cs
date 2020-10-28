namespace CRM.Core
{
    public interface IUrlOptions
    {
        string CrmApiUrl { get; set; }
        string TransactionStoreApiUrl { get; set; }
    }
}