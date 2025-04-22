using BLL.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SpeakAI.Controllers
{
    [Route("api/premium")]
    [ApiController]
    public class PremiumController : ControllerBase
    {
        private readonly IPremiumSubscriptionService _premiumService;

        public PremiumController(IPremiumSubscriptionService premiumService)
        {
            _premiumService = premiumService;
        }

        [HttpPost("upgrade/{userId}")]
        public async Task<IActionResult> UpgradeToPremium(Guid userId)
        {
            var result = await _premiumService.UpgradeToPremium(userId);
            return StatusCode(200, result);
        }

        [HttpPost("confirm-upgrade/{orderId}")]
        public async Task<IActionResult> ConfirmUpgrade(Guid orderId)
        {
            var result = await _premiumService.ConfirmPremiumUpgrade(orderId);
            return StatusCode(200, result);
        }

        [HttpGet("check-access/{userId}")]
        public async Task<IActionResult> CheckPremiumAccess(Guid userId)
        {
            var hasPremiumAccess = await _premiumService.CheckPremiumAccess(userId);
            return Ok(new { hasPremiumAccess });
        }
    }
}
