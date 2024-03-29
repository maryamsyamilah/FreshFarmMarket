﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FreshFarmMarket.Model
{
	public class ApplicationUser : IdentityUser
	{
		[Required(ErrorMessage = "Full Name is required")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "Credit Card Number is required")]
		[RegularExpression(@"^\d{16}$", ErrorMessage = "Credit Card Number must be 16 digits")]
		public string CreditCardNo { get; set; }

		[Required(ErrorMessage = "Gender is required")]
		public string Gender { get; set; }

		[Required(ErrorMessage = "Mobile Number is required")]
		[RegularExpression(@"^\d{8}$", ErrorMessage = "Mobile Number must be 8 digits")]
		public string MobileNo { get; set; }

		[Required(ErrorMessage = "Delivery Address is required")]
		public string DeliveryAddress { get; set; }

		[Required(ErrorMessage = "Email Address is required")]
		[EmailAddress(ErrorMessage = "Invalid Email address")]
		[RegularExpression("/^\\w+[\\+\\.\\w-]*@([\\w-]+\\.)*\\w+[\\w-]*\\.([a-z]{2,4}|\\d+)$/i", ErrorMessage = "Email Address must be a valid email address.")]
		public override string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[DataType(DataType.Password)]
		[MinLength(12)]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$",
			ErrorMessage = "The password must contain at least one lowercase letter, one uppercase letter, one digit, one special character, and be at least 12 characters long")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Photo is required")]
		[DataType(DataType.Upload)]
		[RegularExpression(@"^.*\.jpg$", ErrorMessage = "Photo must be in JPG format")]
		public string Photo { get; set; }

		[Required(ErrorMessage = "About Me is required")]
		public string AboutMe { get; set; }

		[Required]
		public DateTime LastPasswordChangeDate {  get; set; }
	}
}
