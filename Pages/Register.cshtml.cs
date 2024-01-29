using FreshFarmMarket.Model;
using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;

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
				var existingUser = await _userManager.FindByEmailAsync(RModel.Email);
				if (existingUser != null)
				{
					ModelState.AddModelError(string.Empty, "Email is already registered.");
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
					Photo = WebUtility.HtmlEncode(RModel.Photo)
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