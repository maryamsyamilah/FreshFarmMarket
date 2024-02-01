using FreshFarmMarket.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.ViewModels;
using System.Security.Claims;

namespace FreshFarmMarket.Pages
{
    public class ChangePasswordModel : PageModel
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly UserDbContext _dbContext;
		private readonly SignInManager<ApplicationUser> _signInManager;

		[BindProperty]
		public ChangePassword ChangeModel { get; set; }

		public ChangePasswordModel(
			UserManager<ApplicationUser> userManager,
			UserDbContext DbContext,
			SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_dbContext = DbContext;
			_signInManager = signInManager;
		}
		public void OnGet()
        {
            if (TempData.ContainsKey("PasswordExpiredMessage"))
            {
                ModelState.AddModelError(string.Empty, TempData["PasswordExpiredMessage"].ToString());
            }
        }

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
				{
					return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
				}

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, ChangeModel.OldPassword, ChangeModel.NewPassword);
				
				if (changePasswordResult.Succeeded)
				{
					var newPasswordChangeTime = DateTime.UtcNow;
					user.LastPasswordChangeDate = newPasswordChangeTime;
					Console.WriteLine("this is the new time:", newPasswordChangeTime);
					Console.WriteLine("password change successfull!");

					// audit log
					var auditLog = new AuditLog
					{
						Action = "Password Changed",
						UserEmail = user.Email,
						Timestamp = DateTime.UtcNow,
						EntityName = "Password",
						Changes = "Changed to New Password"
					};

					_dbContext.Add(auditLog);

					await _dbContext.SaveChangesAsync();

					await _signInManager.RefreshSignInAsync(user);
					return RedirectToPage("Index");
				}

				foreach (var error in changePasswordResult.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			return Page();
		}
	}
}
