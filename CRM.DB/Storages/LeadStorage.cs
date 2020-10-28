using CRM.Core;
using CRM.DB.Models;
using Dapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.DB.Storages
{
    public class LeadStorage : ILeadStorage
    {
        private IDbConnection connection;

        private IDbTransaction transaction;

        public LeadStorage(IOptions<StorageOptions> storageOptions)
        {
            this.connection = new SqlConnection(storageOptions.Value.DBConnectionString);
        }

        public void TransactionStart()
        {
            connection.Open();
            transaction = this.connection.BeginTransaction();
        }

        public void TransactionCommit()
        {
            this.transaction?.Commit();
            connection?.Close();
        }

        public void TransactioRollBack()
        {
            this.transaction?.Rollback();
            connection?.Close();
        }

        internal static class SpName
        {
            public const string LeadMerge = "Lead_Merge";
            public const string LeadSoftDeleteById = "Lead_SoftDeleteById";
            public const string LeadGetById = "Lead_SelectByID";
            public const string LeadsGetAll = "Lead_SelectAll";
            public const string LeadSearch = "Lead_Search";
            public const string AccountMerge = "Account_Merge";
            public const string AccountSelectByID = "Account_SelectByID";
            public const string AccountDeleteById = "Account_SoftDeleteById";
            public const string Account_SelectAll = "Account_SelectAll";
            public const string BalanceSelectByID = "Balance_GetByAccountId";

        }

        public async ValueTask<Lead> LeadAddOrUpdate(Lead lead)
        {
            int CityId = (int)lead.City.Id;
            if (lead.Id == null) { lead.Id = -1; }
            DynamicParameters leadModelParams = new DynamicParameters(new
            {
                lead.Id,
                lead.LastName,
                lead.FirstName,
                lead.BirthDate,
                lead.Patronymic,
                lead.Email,
                lead.Phone,
                lead.Password,
                lead.Login,
                CityId,
            });
            if (lead.Id == -1)
            {
                var result = await connection.QueryAsync<long>(
                    SpName.LeadMerge,
                    leadModelParams,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure);
                lead.Id = (long)result.FirstOrDefault();
            }
            else
            {
                await connection.QueryAsync(
                    SpName.LeadMerge,
                    leadModelParams,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure);
            }
            return await LeadGetById(lead.Id);
        }

        public async ValueTask LeadDeleteById(long id)
        {
            await connection.QueryAsync<long>(
                SpName.LeadSoftDeleteById,
                new { id },
                transaction: transaction,
                commandType: CommandType.StoredProcedure
            );
        }

        public async ValueTask<Lead> LeadGetById(long? id)
        {
            var leadDictionary = new Dictionary<long, Lead>();
            var result = await connection.QueryAsync<Lead, City, Account, Currency, Lead>(
                SpName.LeadGetById,
                (l, c, a, cur) =>
                {
                    if (!leadDictionary.TryGetValue((long)l.Id, out Lead lead))
                    {
                        lead = l;
                        lead.Accounts = new List<Account>();
                        leadDictionary.Add((long)lead.Id, lead);
                    }
                    lead.City = c;
                    if (a != null)
                    {
                        lead.Accounts.Add(a);
                        a.Currency = cur;
                    }
                    return lead;
                },
                param: new { id },
                transaction: transaction,
                commandType: CommandType.StoredProcedure,
                splitOn: "Id");
            return result.FirstOrDefault();
        }

        public async ValueTask<List<Lead>> SearchLead(LeadSearchModel dataModel)
        {
            DynamicParameters parameters = new DynamicParameters(new
            {
                dataModel.Id,
                dataModel.IdOperator,
                dataModel.IdEnd,
                dataModel.FirstName,
                dataModel.FirstNameOperator,
                dataModel.LastName,
                dataModel.LastNameOperator,
                dataModel.Patronymic,
                dataModel.PatronymicOperator,
                dataModel.BirthDate,
                dataModel.BirthDateOperator,
                dataModel.BirthDateDateEnd,
                dataModel.Phone,
                dataModel.PhoneOperator,
                dataModel.Email,
                dataModel.EmailOperator,
                dataModel.Login,
                dataModel.LoginOperator,
                dataModel.CityId,
                dataModel.CitiesValues,
                dataModel.RegistrationDate,
                dataModel.RegistrationDateOperator,
                dataModel.RegistrationDateEnd,
                dataModel.IsDeletedInclude,
                dataModel.AccountId
            });
            var leadDictionary = new Dictionary<long, Lead>();
            var result = await connection.QueryAsync<Lead, City, Account, Currency, Lead>(
                SpName.LeadSearch,
                (l, c, a, cur) =>
                {
                    if (!leadDictionary.TryGetValue((long)l.Id, out Lead lead))
                    {
                        lead = l;
                        lead.Accounts = new List<Account>();
                        leadDictionary.Add((long)lead.Id, lead);
                    }
                    lead.City = c;
                    if (a != null)
                    {
                        lead.Accounts.Add(a);
                        a.Currency = cur;
                    }
                    return lead;
                },
                parameters,
                transaction: transaction,
                commandType: CommandType.StoredProcedure,
                splitOn: "Id");
            return result.Distinct().ToList();
        }

        public async ValueTask<List<Lead>> LeadsGetAll()
        {
            var leadDictionary = new Dictionary<long, Lead>();
            var result = await connection.QueryAsync<Lead, City, Account, Currency, Lead>(
                SpName.LeadsGetAll,
                (l, c, a, cur) =>
                {
                    if (!leadDictionary.TryGetValue((long)l.Id, out Lead lead))
                    {
                        lead = l;
                        lead.Accounts = new List<Account>();
                        leadDictionary.Add((long)lead.Id, lead);
                    }
                    lead.City = c;
                    if (a != null)
                    {
                        lead.Accounts.Add(a);
                        a.Currency = cur;
                    }
                    return lead;
                },
                param: null,
                transaction: transaction,
                commandType: CommandType.StoredProcedure,
                splitOn: "Id");
            return result.ToList();
        }

        public async ValueTask<Account> AccountAdd(Account account)
        {
            int currencyId = account.Currency.Id;
            if (account.Id == null) { account.Id = -1; }
            DynamicParameters ModelParams = new DynamicParameters(new
            {
                account.Id,
                account.LeadId,
                currencyId
            });

            var result = await connection.QueryAsync<long>(
                SpName.AccountMerge,
                ModelParams,
                transaction: transaction,
                commandType: CommandType.StoredProcedure);
            account.Id = (long)result.FirstOrDefault();

            return await AccountGetById((long)account.Id);
        }

        public async ValueTask<Account> AccountGetById(long id)
        {
            var result = await connection.QueryAsync<Account, Currency, Account>(
                SpName.AccountSelectByID,
                (a, c) =>
                {
                    Account newAccount = a;
                    newAccount.Currency = c;
                    return newAccount;
                },
                param: new { id },
                transaction: transaction,
                commandType: CommandType.StoredProcedure,
                splitOn: "Id");
            return result.FirstOrDefault();
        }

        public async ValueTask AccountDeleteById(long Id)
        {
            await connection.QueryAsync<long>(
                SpName.AccountDeleteById,
                new { Id },
                transaction: transaction,
                commandType: CommandType.StoredProcedure
            );
        }

        public async ValueTask<List<Account>> GetAllAccounts()
        {
            var result = await connection.QueryAsync<Account, Currency, Account>(
                SpName.Account_SelectAll,
                (account, currency) =>
                {
                    Account newAccount = account;
                    newAccount.Currency = currency;
                    return newAccount;
                },
                param: null,
                transaction: transaction,
                commandType: CommandType.StoredProcedure,
                splitOn: "Id");
            return result.ToList();
        }
    }
}
