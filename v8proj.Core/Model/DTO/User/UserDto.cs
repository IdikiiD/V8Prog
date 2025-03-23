using System;
using System.ComponentModel.DataAnnotations;
using v8proj.Core.Enums.Entinity;
using v8proj.Core.Enums.User;

namespace v8proj.Core.Model.DTO.User
{
    public class UserDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        [StringLength(32, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 32 characters")]
        [RegularExpression(@"^[a-zA-Z\s'-]+$", ErrorMessage = "Name must contain only letters, spaces, apostrophes, or hyphens")]
        public string FullName { get; set; }        
        
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Email must be less than 320 characters")]
        public string Email { get; set; }
        
        public DateTime DateRegistered { get; set; }
        public UserType UserType { get; set; }
        public EntityStatus UserStatus { get; set; }
        public AccountVerificationStatus IsVerified { get; set; }
        public SignUpForLettersStatus IsSignUpForLetters { get; set; }
    }
}   
