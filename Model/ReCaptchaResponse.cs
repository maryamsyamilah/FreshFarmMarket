using Newtonsoft.Json;
namespace FreshFarmMarket.Model
{
	public class ReCaptchaResponse
	{
		[JsonProperty("success")]
		public bool Success { get; set; }

		[JsonProperty("error_codes")]
		public string[] ErrorCodes { get; set; }
	}
}
