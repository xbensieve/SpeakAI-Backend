using Common.DTO;
using Common.DTO.Query;
using Common.Query;
using DAL.Entities;
using DAL.IRepositories;
using System.Linq.Expressions;


namespace DAL.GenericRepository.IRepository
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        /// <summary>
        /// Get unpaid trans of user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Transaction?> GetUnPaidTransactionOfUser(Guid userId);

        /// <summary>
        /// Get paid transactions
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Transaction>> GetPaidTransactions();

        /// <summary>
        /// Get paid transactions of user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<Transaction>> GetPaidTransactionsOfUser(Guid userId);

        /// <summary>
        /// get all trans with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<PagedList<TransactionDTO>> GetTransactionsPagedList(TransactionParameters parameters);

        /// <summary>
        /// get trans of user with pagination
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<PagedList<Transaction>> GetTransOfUser(Guid userId, TransactionParameters parameters);
        Transaction? GetLastTransactionOfUser(Guid userId);
        Task<bool> UpdateAsync(Transaction transaction);
        Task<bool> UpdateAsyncc(Guid tranId, Transaction transaction);
    

    }
}
