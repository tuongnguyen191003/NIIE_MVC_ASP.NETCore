using MVC_FinalTerm.Models;
using MVC_FinalTerm.Models.Momo;
namespace MVC_FinalTerm.Service.Momo
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfo model);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
