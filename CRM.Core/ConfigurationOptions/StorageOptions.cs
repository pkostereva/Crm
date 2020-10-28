namespace CRM.Core
{
    public class StorageOptions : IStorageOptions
    {
        public string DBConnectionString { get; set; }
        public string DBMasterConnectionString { get; set; }
    }

}
