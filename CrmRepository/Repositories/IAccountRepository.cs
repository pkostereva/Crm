using CRM.DB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransactionStore.Repository;

namespace CrmRepository.Repositories
{
	public interface IAccountRepository
	{
		ValueTask<RequestResult<Account>> AddAccount(Account dataModel);
		ValueTask<RequestResult<Account>> DeleteAccountLeadById(long Id);
		ValueTask<RequestResult<Account>> GetAccountById(long id);
		ValueTask<RequestResult<List<Account>>> GetAllAccounts();
	}
}