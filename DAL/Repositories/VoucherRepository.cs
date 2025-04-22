using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.DTO;
using DAL.Data;
using DAL.Entities;
using DAL.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class VoucherRepository : GenericRepository<Voucher>, IVoucherRepository
    {
        private readonly SpeakAIContext _context;

        public VoucherRepository(SpeakAIContext speakAIContext) : base(speakAIContext)
        {
            _context = speakAIContext;
        }


        public async Task<Voucher> GetVoucherByCode(string voucherCode)
        {
            return await _dbSet.FirstOrDefaultAsync(v => v.VoucherCode == voucherCode);
        }

        public async Task<Voucher> GetVoucherById(Guid voucherId)
        {
            return await _dbSet.FirstOrDefaultAsync(v => v.VoucherId == voucherId);
        }


        public async Task<List<Voucher>> GetAllVouchers()
        {
            return await _dbSet.ToListAsync();
        }


        public async Task AddVoucherFromDTO(VoucherDTO voucherDTO)
        {
            if (voucherDTO == null) throw new ArgumentNullException(nameof(voucherDTO));

            var voucher = new Voucher
            {
                VoucherId = Guid.NewGuid(),
                VoucherCode = voucherDTO.VoucherCode,
                Description = voucherDTO.Description,
                DiscountPercentage = voucherDTO.DiscountPercentage ?? 0,
                IsActive = voucherDTO.IsActive,
                StartDate = voucherDTO.StartDate,
                EndDate = voucherDTO.EndDate,
                RemainingQuantity = voucherDTO.RemainingQuantity,
                DiscountAmount = 0,
                MinPurchaseAmount = 0,
                VoucherType = "Discount",
                Status = voucherDTO.Status,
            };

            await _dbSet.AddAsync(voucher);
            await _context.SaveChangesAsync();
        }



        public async Task UpdateVoucher(Guid voucherId, UpdateVoucherDTO updateDTO)
        {
            if (updateDTO == null) throw new ArgumentNullException(nameof(updateDTO));

            var existingVoucher = await _dbSet.FindAsync(voucherId);
            if (existingVoucher == null) throw new KeyNotFoundException("Voucher không tồn tại");

            existingVoucher.VoucherCode = updateDTO.VoucherCode;
            existingVoucher.Description = updateDTO.Description;
            existingVoucher.DiscountPercentage = updateDTO.DiscountPercentage;
            existingVoucher.IsActive = updateDTO.IsActive;

            if (updateDTO.StartDate != default(DateTime))
            {
                existingVoucher.StartDate = updateDTO.StartDate;
            }

            if (updateDTO.EndDate != default(DateTime))
            {
                existingVoucher.EndDate = updateDTO.EndDate;
            }

            // ✅ Chỉ cập nhật Status nếu có giá trị hợp lệ
            existingVoucher.Status = updateDTO.Status;

            _dbSet.Update(existingVoucher);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveVoucher(Guid voucherId)
        {
            var voucher = await _dbSet.FindAsync(voucherId);
            if (voucher != null)
            {
                _dbSet.Remove(voucher);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Voucher?> GetVoucherByCodeAsync(string voucherCode)
        {
            return await _context.Vouchers.FirstOrDefaultAsync(v => v.VoucherCode == voucherCode);
        }

        public async Task UpdateVoucherStatusAsync(Voucher voucher)
        {
            _context.Vouchers.Update(voucher);
            await _context.SaveChangesAsync();
        }
    }
}
