using Common.Constant.Message;
using Common.DTO;
using Common.DTO.Payment;
using Common.Enum;
using DAL.Data;
using DAL.Entities;
using DTO.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.IService;
using Service.Service;
using System;

namespace Api_InnerShop.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IVnPayService _vpnPayService;
        private SpeakAIContext _speakAIContext;

        public PaymentController(IPaymentService paymentService, IVnPayService vpnPayService, SpeakAIContext speakAIContext)
        {
            _paymentService = paymentService;
            _vpnPayService = vpnPayService;
            _speakAIContext = speakAIContext;
        }

        [HttpPost("requests")]
        public async Task<IActionResult> CreatePaymentRequest([FromBody] PaymentRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDTO(
                    ModelState.ToString()!,
                    StatusCodeEnum.BadRequest
                ));
            }

            var response = await _paymentService.CreatePaymentRequest(request, HttpContext);

            if (!string.IsNullOrEmpty(response))
            {
                return Ok(new ResponseDTO(
                    GeneralMessage.GetSuccess,
                    StatusCodeEnum.Created,
                    true,
                    response
                ));
            }

            return StatusCode(500, new ResponseDTO(
                GeneralMessage.BadRequest,
                StatusCodeEnum.InteralServerError
            ));
        }

        [HttpPost("handle-response")]
        public async Task<IActionResult> HandleResponse([FromBody] PaymentResponseDTO responseInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDTO(
                    ModelState.ToString()!,
                    StatusCodeEnum.BadRequest
                ));
            }

            var response = await _paymentService.HandlePaymentResponse(responseInfo);

            if (response)
            {
                return Ok(new ResponseDTO(
                    GeneralMessage.GetSuccess,
                    StatusCodeEnum.Created,
                    true
                ));
            }

            return StatusCode(500, new ResponseDTO(
                GeneralMessage.BadRequest,
                StatusCodeEnum.InteralServerError
            ));
        }


    }
}
