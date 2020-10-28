using CRM.DB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.DB.Storages
{
    public interface ILeadStorage
    {
        ValueTask<Account> AccountAdd(Account account);
        ValueTask AccountDeleteById(long Id);
        ValueTask<Account> AccountGetById(long id);
        ValueTask<List<Account>> GetAllAccounts();
        ValueTask<Lead> LeadAddOrUpdate(Lead lead);
        ValueTask LeadDeleteById(long id);
        ValueTask<Lead> LeadGetById(long? id);
        ValueTask<List<Lead>> LeadsGetAll();
        ValueTask<List<Lead>> SearchLead(LeadSearchModel datamodel);
        void TransactionCommit();
        void TransactionStart();
        void TransactioRollBack();
    }
}