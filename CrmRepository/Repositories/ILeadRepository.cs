using CRM.DB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransactionStore.Repository;

namespace CrmRepository.Repositories
{
    public interface ILeadRepository
    {
        ValueTask<RequestResult<Lead>> AddOrUpdateLead(Lead dataModel);
        ValueTask<RequestResult<List<Lead>>> GetAllLeads();
        ValueTask<RequestResult<Lead>> GetLeadById(long id);
        ValueTask<RequestResult<List<Lead>>> SearchLead(LeadSearchModel dataModel);
        ValueTask<RequestResult<Lead>> DeleteLeadById(long id);
    }
}