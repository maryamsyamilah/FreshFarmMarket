using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Net;
using System.IdentityModel.Tokens.Jwt;

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
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError(string.Empty, "Invalid email or password.");
				return Page();
			}

			var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == LModel.Email);
			if (existingUser == null)
			{
				ModelState.AddModelError(string.Empty, "Email does not exist, please register for an account.");
				return Page();
			}

			string password = LModel.Password;
			string userid = LModel.Email;

			var result = await signInManager.PasswordSignInAsync(userid, password, false, lockoutOnFailure: true);
			if (result.Succeeded)
			{
				var session = _httpContextAccessor.HttpContext.Session;
				session.SetString("LoggedIn", userid);

				string guid = Guid.NewGuid().ToString();
				session.SetString("AuthToken", guid);

				Response.Cookies.Append("AuthToken", guid);

				var maxPasswordAge = TimeSpan.FromMinutes(7);
				var timeSinceLastChange = DateTime.UtcNow - existingUser.LastPasswordChangeDate;
				if (timeSinceLastChange > maxPasswordAge)
				{
					TempData["PasswordExpiredMessage"] = "Your password has expired, please change it";
					return RedirectToPage("ChangePassword");
				}
				else
				{
					return RedirectToPage("Index");
				}

			}

			else if (result.IsLockedOut)
			{
				ModelState.AddModelError(string.Empty, "Your account has been locked out");
				await Task.Delay(TimeSpan.FromSeconds(30));
				await _userManager.SetLockoutEndDateAsync(existingUser, null);
				await _userManager.ResetAccessFailedCountAsync(existingUser);
				ModelState.AddModelError(string.Empty, "Your account has been automatically unlocked. Please try again.");
				return Page();
			}
			ModelState.AddModelError(string.Empty, "Invalid email or password.");
			return Page();
		}


		public bool ValidateCaptcha()
		{
			string Response = Request.Form["g-recaptcha-response"];
			bool Valid = false;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create
				("https://www.google.com/recaptcha/api/siteverify?secret=6LcQ_2IpAAAAAFsJoCm6zWpRxQ28QAQsJZvpBN_Q &response=" + Response);
			try
			{
				using (WebResponse wResponse = request.GetResponse())
				{
					using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
					{
						string jsonResponse = readStream.ReadToEnd();

						ReCaptchaResponse data = JsonConvert.DeserializeObject<ReCaptchaResponse>(jsonResponse);

						Valid = Convert.ToBoolean(data.Success);
					}
				}

				return Valid;
			}
			catch (WebException ex)
			{
				throw ex;
			}
		}
	}
}
