
using BLL.IService;
using Common.DTO;
using Common.DTO.Query;

using Common;
using DAL.UnitOfWork;
using DAL.Entities;
using Common.Query;
using Common.Constant.Message;
using DTO.DTO;
using Common.Enum;


namespace BLL.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;


        public TransactionService(IUnitOfWork unitOfWork
                                 )
        {
            _unitOfWork = unitOfWork;

        }

        public async Task AddNewTransaction(Transaction transaction)
        {
            await _unitOfWork.Transaction.AddAsync(transaction);
        }

        public async Task<IEnumerable<Transaction>> GetAllPaidTransactions()
        {
            return await _unitOfWork.Transaction.GetPaidTransactions();
        }

        public async Task<ResponseDTO> GetAllTransactions(TransactionParameters parameters)
        {
            var transactions = await _unitOfWork.Transaction.GetTransactionsPagedList(parameters);
            var transactionDtos = transactions.Select(t => new TransactionDTO
            {
                TransactionId = t.TransactionId,
                TransactionNumber = t.TransactionNumber,
                PaymentMethod = t.PaymentMethod,
                TransactionInfo = t.TransactionInfo,
                UserId = t.UserId,
                TransactionDate = t.TransactionDate,
                OrderId = t.OrderId,
                Amount =t.Amount,
                Status = t.Status,
            
            }).ToList();

            var mappedResponse = new PaginationResponseDTO<TransactionDTO>
            {
                PageSize = transactions.PageSize,
                CurrentPage = transactions.CurrentPage,
                TotalPages = transactions.TotalPages,
                Items = transactions.ToList()
            };

            return new ResponseDTO(
                GeneralMessage.GetSuccess,
                StatusCodeEnum.OK,
                true,
              mappedResponse


            );
        }

        public async Task<Transaction?> GetUnpaidTransOfUser(Guid userId)
        {
            return await _unitOfWork.Transaction.GetUnPaidTransactionOfUser(userId);
        }

        public async Task<IEnumerable<Transaction>> GetPaidTransOfUser(Guid userId)
        {
            return await _unitOfWork.Transaction.GetPaidTransactionsOfUser(userId);
        }

        public async Task<ResponseDTO> GetTransOfUser(Guid userId, TransactionParameters parameters)
        {
            var transactions = await _unitOfWork.Transaction.GetTransOfUser(userId, parameters);
            var transactionDtos = transactions.Select(t => new TransactionDTO
            {
                TransactionId = t.TransactionId,
                TransactionNumber = t.TransactionReference,
                PaymentMethod = t.PaymentMethod,
                TransactionInfo = t.TransactionType,
                TransactionDate = t.TransactionDate,
                Amount = t.Amount,
                Status = t.Status,
                Email = t.User?.Email ?? string.Empty,
            }).ToList();

            var mappedResponse = new PaginationResponseDTO<TransactionDTO>
            {
                PageSize = transactions.PageSize,
                CurrentPage = transactions.CurrentPage,
                TotalPages = transactions.TotalPages,
                Items = transactionDtos
            };

            return new ResponseDTO(
                GeneralMessage.GetSuccess,
                StatusCodeEnum.OK,
                true,
                mappedResponse
            );
        }
        public async Task<bool> UpdateTransaction(Guid tranId, Transaction transaction)
        {
            return await _unitOfWork.Transaction.UpdateAsyncc(tranId, transaction);
        }
        public Transaction? GetLastTransOfUser(Guid userId)
        {
            return _unitOfWork.Transaction.GetLastTransactionOfUser(userId);
        }


        public async Task<Transaction?> GetTransactionById(Guid transactionId)
        {
            return await _unitOfWork.Transaction.GetByIdAsync(transactionId);
        }
    }
}
