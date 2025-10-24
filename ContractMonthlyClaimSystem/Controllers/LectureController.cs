using Microsoft.AspNetCore.Mvc;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class LectureController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
