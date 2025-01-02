using MVC_FinalTerm.Models.ViewModels;

namespace MVC_FinalTerm.Services.VnPay
{
    public interface IVnPayService
    {

        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);


    }
}
