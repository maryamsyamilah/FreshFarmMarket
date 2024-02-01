using FreshFarmMarket.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Net;

namespace FreshFarmMarket.Pages
{
	[Authorize]
	public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
		private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserDbContext _dbContext;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public IndexModel(ILogger<IndexModel> logger, 
            IHttpContextAccessor httpContextAccessor, 
            UserDbContext DbContext,
            IDataProtectionProvider dataProtectionProvider)
        {
            _logger = logger;
			_httpContextAccessor = httpContextAccessor;
            _dbContext = DbContext;
            _dataProtectionProvider = dataProtectionProvider;


        }
		public string LoggedInUser { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CreditCardNo { get; set; }
        public string Gender { get; set; }
        public string MobileNo { get; set; }
        public string DeliveryAddress { get; set; }
        public string AboutMe { get; set; }
        public string Photo { get; set; }

        public string AuthToken { get; set; }
		public string AuthTokenCookie { get; set; }

		public async Task<IActionResult> OnGetAsync()
        {
            var session = _httpContextAccessor.HttpContext.Session;
			LoggedInUser = session.GetString("LoggedIn");
			AuthToken = session.GetString("AuthToken");
			AuthTokenCookie = Request.Cookies["AuthToken"];

            var userId = User.Identity.Name;
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userId);

            var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
            var protector = dataProtectionProvider.CreateProtector("MySecretKey");

            FullName = WebUtility.HtmlDecode(user.FullName);
            try
            {
                Console.WriteLine(user.CreditCardNo);
                CreditCardNo = protector.Unprotect(user.CreditCardNo);
                Console.WriteLine(user.CreditCardNo);
            } catch (CryptographicException)
            {
                CreditCardNo = user.CreditCardNo;
                Console.WriteLine("Decryption error");
            }
            // CreditCardNo = user.CreditCardNo;
            Email = WebUtility.HtmlDecode(user.Email);
            Gender = WebUtility.HtmlDecode(user.Gender);
            MobileNo = WebUtility.HtmlDecode(user.MobileNo);
            DeliveryAddress = WebUtility.HtmlDecode(user.DeliveryAddress);
            AboutMe = WebUtility.HtmlDecode(user.AboutMe);
            Photo = WebUtility.HtmlDecode(user.Photo);

            return Page();
        }

		public async Task<IActionResult> Page_Load()
		{
			if (LoggedInUser != null && AuthToken != null && AuthTokenCookie != null)
			{
				if (!AuthToken.Equals(AuthTokenCookie))
                { 


                    return RedirectToPage("Login");
				}
				else
				{
					LoggedInUser = User.Identity.Name.ToString();
					return Page();
				}
			}
			else
			{
				return RedirectToPage("Login");
			}
		}

        protected void Logout(object sender, EventArgs e)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.Clear();

            RedirectToPage("Login");

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies.Delete("ASP.NET_SessionId");
            }
            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies.Delete("AuthToken");
            }
           
        }
    }
}