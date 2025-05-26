using System;
using System.ComponentModel.DataAnnotations;

namespace v8proj.Web.ViewModels
{
    public class ProfileViewModel
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public string PhoneNumber { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime RegistrationDate { get; set; }

        // Для формы добавления номера телефона
        public string NewPhoneNumber { get; set; }
    }
}