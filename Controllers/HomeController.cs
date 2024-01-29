using Microsoft.AspNetCore.Mvc;

namespace FreshFarmMarket.Controllers
{
	public class HomeController : Controller
	{
		private readonly IHttpContextAccessor contxt;
		public HomeController(IHttpContextAccessor httpContextAccessor)
		{
			contxt = httpContextAccessor;
		}
		public IActionResult Index()
		{
            var userName = contxt.HttpContext.User.Identity.Name;
            contxt.HttpContext.Session.SetString("StudentName", userName);
            return View();
		}
	}
}
