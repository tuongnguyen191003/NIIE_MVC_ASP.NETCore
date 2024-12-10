using MVC_FinalTerm.Models.VnPay;

namespace MVC_FinalTerm.Services.VnPay
{
    public interface IVnPayService
    {

        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);



    }
}
