using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Net;
using FreshFarmMarket.Model;

namespace FreshFarmMarket.Pages
{
    public class LoginModel : PageModel
    {
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
		public Login LModel { get; set; }
		public LoginModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
		{
			this.signInManager = signInManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
			_configuration = configuration;
		}
		public void OnGet()
        {
		}

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
                //var existingUser = await _userManager.FindByEmailAsync(LModel.Email);
                //if (existingUser == null)
                //{
                    //ModelState.AddModelError(string.Empty, "Email does not exist, please register for an account.");
                    //return Page();
                //}

                string password = LModel.Password;
				string userid = LModel.Email;

				var result = await signInManager.PasswordSignInAsync(userid, password, false, lockoutOnFailure: false);
				if (result.Succeeded)
				{
					var session = _httpContextAccessor.HttpContext.Session;
					session.SetString("LoggedIn", userid);

					string guid = Guid.NewGuid().ToString();
					session.SetString("AuthToken", guid);

					Response.Cookies.Append("AuthToken", guid);

					return RedirectToPage("Index");
				}
			}
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }
            return Page();
		}
	
	}
}
