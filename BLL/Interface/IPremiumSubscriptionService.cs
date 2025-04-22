using DTO.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
   public interface IPremiumSubscriptionService
    {
       Task<ResponseDTO> UpgradeToPremium(Guid userId);
        Task<ResponseDTO> ConfirmPremiumUpgrade(Guid orderId);
        Task<bool> CheckPremiumAccess(Guid userId);
    }
}
