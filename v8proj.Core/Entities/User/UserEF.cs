using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using v8proj.Core.Enums.Entinity;
using v8proj.Core.Enums.User;

namespace v8proj.Core.Entities.User
{
    public class UserEf
    {
        [Key]
        public int UserId { get; set; } 
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        
        public string AvatarUrl { get; set; }
        
        public string PhoneNumber { get; set; }
        public DateTime RegistrationDate { get; set; }


        

        
        public DateTime DateRegistered { get; set; } = DateTime.UtcNow;
        public UserType UserType { get; set; } = UserType.User;
        public EntityStatus UserStatus { get; set; } = EntityStatus.Active;
        public AccountVerificationStatus IsVerified { get; set; } = AccountVerificationStatus.NotVerified;
        public SignUpForLettersStatus IsSignUpForLetters { get; set; } = SignUpForLettersStatus.No;
        
        public virtual ICollection<Post> FavoritePosts { get; set; } = new List<Post>();
        
        

        
        
        
    }
}