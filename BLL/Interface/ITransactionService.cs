using Common.DTO;
using Common.Query;
using DAL.Entities;
using DTO.DTO;


namespace BLL.IService
{
    public interface ITransactionService
    {
        /// <summary>
        /// Get all transactions
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<ResponseDTO> GetAllTransactions(TransactionParameters parameters);

        /// <summary>
        /// Add new transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task AddNewTransaction(Transaction transaction);

        /// <summary>
        /// Update transaction
        /// </summary>
        /// <param name="tranId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> UpdateTransaction(Guid tranId, Transaction transaction);

        /// <summary>
        /// Get last unpaid trans of user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Transaction?> GetUnpaidTransOfUser(Guid userId);

        /// <summary>
        /// Get trans of user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<ResponseDTO> GetTransOfUser(Guid userId, TransactionParameters parameters);

        /// <summary>
        /// Get all paid transaction of user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<Transaction>> GetPaidTransOfUser(Guid userId);

        /// <summary>
        /// Get all paid transactions
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Transaction>> GetAllPaidTransactions();
        Transaction? GetLastTransOfUser(Guid userId);

        /// <summary>
        /// Get transaction by transaction ID
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        Task<Transaction?> GetTransactionById(Guid transactionId);

    }
}
