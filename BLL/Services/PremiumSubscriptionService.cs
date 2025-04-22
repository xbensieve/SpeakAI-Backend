using BLL.Interface;
using Common.Enum;
using DAL.Entities;
using DAL.UnitOfWork;
using DTO.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PremiumSubscriptionService : IPremiumSubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public PremiumSubscriptionService(
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<ResponseDTO> UpgradeToPremium(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.User.FindAll(u => u.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                {
                    return new ResponseDTO("User not found", StatusCodeEnum.NotFound, false);
                }

                if (user.IsPremium)
                {
                    return new ResponseDTO("User is already premium", StatusCodeEnum.Created, false);
                }

                
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TotalAmount = 100, 
                    OrderDate = DateTime.UtcNow,
                };

                await _unitOfWork.Order.AddAsync(order);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseDTO("Premium upgrade order created", StatusCodeEnum.Created, true, order.Id);
            }
            catch (Exception ex)
            {
                return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
            }
        }

        public async Task<ResponseDTO> ConfirmPremiumUpgrade(Guid orderId)
        {
            try
            {
                var order = await _unitOfWork.Order
                    .FindAll(o => o.Id == orderId)
                    .Include(o => o.User)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    return new ResponseDTO("Order not found", StatusCodeEnum.NotFound, false);
                }

                order.User.IsPremium = true;
           
                order.User.PremiumExpiredTime = DateTime.UtcNow.AddDays(30);


                await _unitOfWork.SaveChangeAsync();

                _emailService.SendPremiumConfirmationEmail(order.User.Email, order.User.Username);
                _emailService.SendPremiumPurchaseReceiptEmail(
                    order.User.Email,
                    order.User.Username,
                    order.TotalAmount,
                    order.Id.ToString()
                );

                return new ResponseDTO("Premium upgrade successful", StatusCodeEnum.Created, true);
            }
            catch (Exception ex)
            {
                return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
            }
        }

        public async Task<bool> CheckPremiumAccess(Guid userId)
        {
            var user = await _unitOfWork.User.FindAll(u => u.Id == userId).FirstOrDefaultAsync();
            return user?.IsPremium ?? false;
        }
    }
}
