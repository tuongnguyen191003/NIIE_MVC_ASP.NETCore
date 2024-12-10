using Microsoft.AspNetCore.Mvc;
using MVC_FinalTerm.Models;
using MVC_FinalTerm.Models.VnPay;
using MVC_FinalTerm.Service.Momo;
using MVC_FinalTerm.Services.VnPay;

namespace MVC_FinalTerm.Controllers
{
    public class PaymentController : Controller
    {

        private IMomoService _momoService;
        private readonly IVnPayService _vnPayService;
        public PaymentController(IMomoService momoService, IVnPayService vnPayService)
        {
            _momoService = momoService;
            _vnPayService = vnPayService;
        }
        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrl(OrderInfo model)
        {
            var response = await _momoService.CreatePaymentAsync(model); if (string.IsNullOrEmpty(response.PayUrl))
            { // Xử lý lỗi khi PayUrl bị null hoặc rỗng
                return View("Error");
            }
            return Redirect(response.PayUrl);
        }
        [HttpGet]
        public IActionResult PaymentCallBack()
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            return View(response);
        }

        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }




    }

}

