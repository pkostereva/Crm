using CRM.DB.Models;
using CRM.DB.Storages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransactionStore.Repository;

namespace CrmRepository.Repositories
{
    public class LeadRepository : ILeadRepository
    {
        private ILeadStorage _leadStorage;

        public LeadRepository( ILeadStorage leadStorage)
        {
            _leadStorage = leadStorage;
        }

        public async ValueTask<RequestResult<Lead>> AddOrUpdateLead(Lead dataModel)
        {
            var result = new RequestResult<Lead>();
            try
            {
                _leadStorage.TransactionStart();
                result.RequestData = await _leadStorage.LeadAddOrUpdate(dataModel);
                _leadStorage.TransactionCommit();
                result.IsOkay = true;
            }
            catch (Exception ex)
            {
                _leadStorage.TransactioRollBack();
                result.ExMessage = ex.Message;               
            }
            return result;
        }

        public async ValueTask<RequestResult<List<Lead>>> GetAllLeads()
        {
            var result = new RequestResult<List<Lead>>();
            try
            {
                result.RequestData = await _leadStorage.LeadsGetAll();
                result.IsOkay = true;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
            }
            return result;
        }

        public async ValueTask<RequestResult<Lead>> GetLeadById(long id)
        {
            var result = new RequestResult<Lead>();
            try
            {
                result.RequestData = await _leadStorage.LeadGetById(id);
                result.IsOkay = true;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
            }
            return result;
        }

        public async ValueTask<RequestResult<List<Lead>>> SearchLead(LeadSearchModel dataModel)
        {
            var result = new RequestResult<List<Lead>>();
            try
            {
                result.RequestData = await _leadStorage.SearchLead(dataModel);
                result.IsOkay = true;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
            }
            return result;
        }

        public async ValueTask<RequestResult<Lead>> DeleteLeadById(long id)
        {
            var result = new RequestResult<Lead>();
            try
            {
                _leadStorage.TransactionStart();
                await _leadStorage.LeadDeleteById(id);
                _leadStorage.TransactionCommit();
                result.IsOkay = true;
            }
            catch (Exception ex)
            {
                _leadStorage.TransactioRollBack();
                result.ExMessage = ex.Message;
            }
            return result;
        }
    }
}
