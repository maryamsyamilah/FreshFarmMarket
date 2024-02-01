using FreshFarmMarket.Model;
using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FreshFarmMarket.Pages
{
	public class RegisterModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		[BindProperty]
		public Register RModel { get; set; }

		public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public void OnGet()
		{
		}

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == RModel.Email);
				if (existingUser == null)
				{
					ModelState.AddModelError(string.Empty, "Email has already been registered.");
					return Page();
				}

				var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");

				var user = new ApplicationUser
				{
					UserName = WebUtility.HtmlEncode(RModel.Email),
					Email = WebUtility.HtmlEncode(RModel.Email),
					Password = WebUtility.HtmlEncode(RModel.Password),
					FullName = WebUtility.HtmlEncode(RModel.FullName),
					CreditCardNo = protector.Protect(RModel.CreditCardNo),
					Gender = WebUtility.HtmlEncode(RModel.Gender),
					MobileNo = WebUtility.HtmlEncode(RModel.MobileNo),
					DeliveryAddress = WebUtility.HtmlEncode(RModel.DeliveryAddress),
					AboutMe = WebUtility.HtmlEncode(RModel.AboutMe),
					Photo = WebUtility.HtmlEncode(RModel.Photo),
					LastPasswordChangeDate = DateTime.UtcNow
				};

				var result = await _userManager.CreateAsync(user, RModel.Password);
				if (result.Succeeded)
				{
					

					await _signInManager.SignInAsync(user, isPersistent: false);
					return RedirectToPage("Login");
				}

				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			return Page();
		}
    }
}