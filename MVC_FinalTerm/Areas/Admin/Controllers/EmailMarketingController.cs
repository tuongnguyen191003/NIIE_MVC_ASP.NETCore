using Microsoft.AspNetCore.Mvc;
using MVC_FinalTerm.Services.Email;
 // Đảm bảo bạn đã import đúng namespace cho EmailSender
namespace MVC_FinalTerm.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmailMarketingController : Controller
    {
        // GET: Admin/EmailMarketing/Index
        public IActionResult Index()
        {
            return View();
        }
        // POST: Admin/EmailMarketing/SendEmail
        [HttpPost]
        public IActionResult SendEmail(string receiverEmail, string subject, string message)
        {
            try
            {
                // Lấy thông tin người gửi từ appsettings.json
                string senderEmail = "2100004703@nttu.edu.vn";
                string senderName = "Cybercore MVC";
                // Gửi email
                EmailSender.SendEmail(senderEmail, senderName, receiverEmail, "Customer", subject, message);
                // Thông báo thành công
                TempData["SuccessMessage"] = "Email sent successfully!";
                return View("Index"); // Trả về lại cùng trang Index với thông báo thành công
            }
            catch (Exception ex)
            {
                // Thông báo lỗi
                TempData["ErrorMessage"] = $"Failed to send email: {ex.Message}";
                return View("Index"); // Trả về lại cùng trang Index với thông báo lỗi
            }
        }
    }
}