using BLL.Interface;
using BLL.Service;
using Common.DTO;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        private readonly ILogger<VoucherController> _logger;

        public VoucherController(IVoucherService voucherService, ILogger<VoucherController> logger)
        {
            _voucherService = voucherService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("id/{voucherId}")]
        public async Task<IActionResult> GetVoucherById(Guid voucherId)
        {
            var voucher = await _voucherService.GetVoucherById(voucherId);
            if (voucher == null)
                return NotFound("Voucher không tồn tại.");
            return Ok(voucher);
        }


        [HttpGet("{voucherCode}")]
        public async Task<ActionResult<VoucherResponseDTO>> GetVoucherByCode(string voucherCode)
        {
            var voucher = await _voucherService.GetVoucherByCode(voucherCode);
            if (voucher == null)
            {
                return NotFound();
            }

            var response = new VoucherResponseDTO
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

            return Ok(response);
        }


        [HttpGet]
        public async Task<ActionResult<List<Voucher>>> GetAllVouchers()
        {
            var vouchers = await _voucherService.GetAllVouchers();
            return Ok(vouchers);
        }

        // ✅ API để kiểm tra và vô hiệu hóa voucher thủ công
        [HttpGet("check-and-disable")]
        public async Task<IActionResult> CheckAndDisableVouchers()
        {
            _logger.LogInformation("API request received to check and disable vouchers.");

            await _voucherService.CheckAndDisableVouchersAsync();

            return Ok(new { message = "Voucher check completed." });
        }

        [HttpPost]
        public async Task<IActionResult> AddVoucher([FromBody] VoucherDTO voucherDTO)
        {
            if (voucherDTO == null)
            {
                return BadRequest("Voucher data is required.");
            }

            await _voucherService.AddVoucherFromDTO(voucherDTO);
            return CreatedAtAction(nameof(GetVoucherByCode), new { voucherCode = voucherDTO.VoucherCode }, voucherDTO);
        }




        [HttpPut("{voucherId}")]
        public async Task<IActionResult> UpdateVoucher(Guid voucherId, [FromBody] UpdateVoucherDTO updateDTO)
        {
            if (updateDTO == null)
                return BadRequest("Dữ liệu không hợp lệ.");

            await _voucherService.UpdateVoucherFromDTO(voucherId, updateDTO);

            var updatedVoucher = await _voucherService.GetVoucherById(voucherId);
            return Ok(updatedVoucher);
        }


        [HttpDelete("{voucherId}")]
        public async Task<ActionResult> RemoveVoucher(Guid voucherId)
        {
            var voucher = await _voucherService.GetVoucherById(voucherId);
            if (voucher == null)
            {
                return NotFound(new { message = "Voucher not found" });
            }

            await _voucherService.RemoveVoucher(voucherId);

            return Ok(new { message = "Voucher deleted successfully" });
        }




    }
}
