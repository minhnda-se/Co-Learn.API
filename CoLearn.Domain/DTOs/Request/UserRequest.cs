using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.DTOs.Request
{
    public class UserRequest
    {
        public class CreateUserModel
        {
            [Required(ErrorMessage = "Full name is required.")]
            [MaxLength(100, ErrorMessage = "Full name must not exceed 100 characters.")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Password confirmation is required.")]
            [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
            public string PasswordConfirm { get; set; }

            [Phone(ErrorMessage = "Invalid phone number format.")]
            public string? Phone { get; set; }

            public DateTime? DateOfBirth { get; set; }

            [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other.")]
            public string? Gender { get; set; }

            [Required(ErrorMessage = "Primary role is required.")]
            public int PrimaryRoleId { get; set; }
        }

        public class UpdateUserModel
        {
            [MaxLength(100, ErrorMessage = "Full name must not exceed 100 characters.")]
            public string? FullName { get; set; }

            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string? Email { get; set; }

            [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
            public string? Password { get; set; }

            public string? Phone { get; set; }

            public DateTime? DateOfBirth { get; set; }

            [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other.")]
            public string? Gender { get; set; }

            public int? PrimaryRoleId { get; set; }

            public bool? IsActive { get; set; }
        }

        public class LoginRequest
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            public string Password { get; set; }
        }
    }
}