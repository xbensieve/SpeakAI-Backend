
using Common.Constant.Payment;
using Common.DTO;
using Common.DTO.Query;
using Common.Query;
using DAL.Data;
using DAL.Entities;
using DAL.GenericRepository.IRepository;
using DAL.Repositories;

using Microsoft.EntityFrameworkCore;

namespace DAL.GenericRepository.Repository
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly SpeakAIContext _peakAIContext;
        public TransactionRepository(SpeakAIContext context) : base(context) { 
            _peakAIContext = context;
        }
        public async Task<IEnumerable<Transaction>> GetPaidTransactions()
        {
            return await _peakAIContext.Transactions.Where(t => t.Status == PaymentConst.PaidStatus).ToListAsync();
        }

        public async Task<PagedList<TransactionDTO>> GetTransactionsPagedList(TransactionParameters parameters)
        {
            var trans = _peakAIContext.Transactions
      .Include(t => t.User)
      .Where(t => t.Status != PaymentConst.CancelStatus)
      .Select(t => new TransactionDTO
      {
          TransactionId = t.Id,
          TransactionDate = t.TransactionDate,
          Status = t.Status,
          OrderId = t.OrderId,
          UserId = t.UserId,
          Amount = t.Amount,
      });

            if (parameters.Status != null)
            {
                trans = trans.Where(u => u.Status.ToLower() == parameters.Status.ToLower());
            }

            return await PagedList<TransactionDTO>.ToPagedList(
                trans.OrderByDescending(p => p.TransactionDate),
                parameters.PageNumber,
                parameters.PageSize);
        }

        public async Task<Transaction?> GetUnPaidTransactionOfUser(Guid userId)
        {
            return await _peakAIContext.Transactions.FirstOrDefaultAsync(t => t.UserId == userId && t.Status == PaymentConst.PendingStatus);
        }

        public async Task<IEnumerable<Transaction>> GetPaidTransactionsOfUser(Guid userId)
        {
            return await _peakAIContext.Transactions.Where(t => t.UserId == userId && t.Status == PaymentConst.PaidStatus).ToListAsync();
        }

        public async Task<PagedList<Transaction>> GetTransOfUser(Guid userId, TransactionParameters parameters)
        {
            var trans = _peakAIContext.Transactions
                .Include(t => t.User)
                .Include(t => t.Order)
                .Where(t => t.UserId == userId && t.Status != PaymentConst.CancelStatus);

            if (parameters.Status != null)
            {
                trans = trans.Where(u => u.Status.ToLower() == parameters.Status.ToLower());
            }

            return await PagedList<Transaction>.ToPagedList(trans.OrderByDescending(p => p.TransactionDate), parameters.PageNumber, parameters.PageSize);
        }
        public async Task<bool> UpdateAsync(Transaction entity)
        {
            _dbSet.Update(entity);
            await _peakAIContext.SaveChangesAsync();
            return true;
        }
        public Transaction? GetLastTransactionOfUser(Guid userId)
        {
            var pendingTransactions = _peakAIContext.Transactions.Where(p => p.UserId == userId && p.Status == PaymentConstant.PendingStatus);
            return pendingTransactions.FirstOrDefault();
        }
        public async Task<bool> UpdateAsyncc    (Guid tranId, Transaction transaction)
        {
            var existingTransaction = await _peakAIContext.Transactions.FindAsync(tranId);
            if (existingTransaction == null) return false;

       
            existingTransaction.Status = transaction.Status;
            existingTransaction.Amount = transaction.Amount;

            _peakAIContext.Transactions.Update(existingTransaction);
            await _peakAIContext.SaveChangesAsync();
            return true;
        }

    }
}
