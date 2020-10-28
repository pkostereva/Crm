using CRM.DB.Models;
using CRM.DB.Storages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransactionStore.Repository;

namespace CrmRepository.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private ILeadStorage _leadStorage;

        public AccountRepository(ILeadStorage leadStorage)
        {
            _leadStorage = leadStorage;
        }

        public async ValueTask<RequestResult<Account>> AddAccount(Account dataModel)
        {
            var result = new RequestResult<Account>();
            try
            {
                _leadStorage.TransactionStart();
                result.RequestData = await _leadStorage.AccountAdd(dataModel);
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

        public async ValueTask<RequestResult<Account>> GetAccountById(long id)
        {
            var result = new RequestResult<Account>();
            try
            {
                result.RequestData = await _leadStorage.AccountGetById(id);
                result.IsOkay = true;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
            }
            return result;
        }

        public async ValueTask<RequestResult<Account>> DeleteAccountLeadById(long Id)
        {
            var result = new RequestResult<Account>();
            try
            {
                _leadStorage.TransactionStart();
                await _leadStorage.AccountDeleteById(Id);
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

        public async ValueTask<RequestResult<List<Account>>> GetAllAccounts()
        {
            var result = new RequestResult<List<Account>>();
            try
            {
                result.RequestData = await _leadStorage.GetAllAccounts();
                result.IsOkay = true;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
            }
            return result;
        }
    }
}
