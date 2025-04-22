using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Interface;
using Common.DTO;
using DAL.Entities;
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VoucherService> _logger;

        public VoucherService(IUnitOfWork unitOfWork, ILogger<VoucherService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<VoucherResponseDTO> GetVoucherById(Guid voucherId)
        {
            var voucher = await _unitOfWork.Voucher.GetVoucherById(voucherId);

            if (voucher == null) return null;

            return new VoucherResponseDTO
            {
                VoucherId = voucher.VoucherId,
                VoucherCode = voucher.VoucherCode,
                Description = voucher.Description,
                DiscountPercentage = (decimal?)voucher.DiscountPercentage,
                IsActive = voucher.IsActive,
                StartDate = voucher.StartDate,
                EndDate = voucher.EndDate,
                Status = voucher.Status,
                RemainingQuantity = voucher.RemainingQuantity
            };
        }


        public async Task<Voucher> GetVoucherByCode(string voucherCode)
        {
            return await _unitOfWork.Voucher.GetVoucherByCode(voucherCode);
        }

        public async Task<List<Voucher>> GetAllVouchers()
        {
            return await _unitOfWork.Voucher.GetAllVouchers();
        }

        public async Task<Voucher> AddVoucherFromDTO(VoucherDTO voucherDTO)
        {
            await _unitOfWork.Voucher.AddVoucherFromDTO(voucherDTO);
            await _unitOfWork.SaveChangeAsync();
            return await _unitOfWork.Voucher.GetVoucherByCode(voucherDTO.VoucherCode);
        }

        public async Task UpdateVoucherFromDTO(Guid voucherId, UpdateVoucherDTO updateDTO)
        {
            if (updateDTO == null) throw new ArgumentNullException(nameof(updateDTO));

            await _unitOfWork.Voucher.UpdateVoucher(voucherId, updateDTO);
            await _unitOfWork.SaveChangeAsync();
        }





        public async Task RemoveVoucher(Guid voucherId)
        {
            await _unitOfWork.Voucher.RemoveVoucher(voucherId);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<bool> DisableExpiredOrDepletedVouchersAsync()
        {
            _logger.LogInformation("Checking for expired or depleted vouchers...");

            var vouchers = await _unitOfWork.Voucher.GetAllVouchers();
            var now = DateTime.UtcNow;
            var updatedVouchers = new List<Voucher>();

            foreach (var voucher in vouchers)
            {
                if (voucher.EndDate <= now || voucher.RemainingQuantity <= 0)
                {
                    if (voucher.IsActive)
                    {
                        voucher.IsActive = false;
                        updatedVouchers.Add(voucher);
                    }
                }
            }

            if (updatedVouchers.Any())
            {
                foreach (var voucher in updatedVouchers)
                {
                    await _unitOfWork.Voucher.UpdateVoucherStatusAsync(voucher);
                }
                await _unitOfWork.SaveChangeAsync();

                _logger.LogInformation($"{updatedVouchers.Count} vouchers have been disabled.");
                return true;
            }

            _logger.LogInformation("No vouchers were disabled.");
            return false;
        }


        public async Task CheckAndDisableVouchersAsync()
        {
            await DisableExpiredOrDepletedVouchersAsync();
        }





    }

}
