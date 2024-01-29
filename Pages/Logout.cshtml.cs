using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.Model;

namespace FreshFarmMarket.Pages
{
    public class LogoutModel : PageModel
    {

		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly IHttpContextAccessor _httpContextAccessor;
		public LogoutModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor)
		{
			this.signInManager = signInManager;
			_httpContextAccessor = httpContextAccessor;
		}
		public void OnGet() { }
		public async Task<IActionResult> OnPostLogoutAsync()
		{
			await signInManager.SignOutAsync();
			var session = _httpContextAccessor.HttpContext.Session;
			session.Clear();
			return RedirectToPage("Login");
		}
		public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("Index");
		}
	}
}
