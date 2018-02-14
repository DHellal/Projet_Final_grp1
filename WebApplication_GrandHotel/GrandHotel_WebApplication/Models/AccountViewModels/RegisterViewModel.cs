using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GrandHotel_WebApplication.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email requis")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Mot de passe requis")]
        [StringLength(100, ErrorMessage = "Entre 6 et 100 caractères et doit contenir des majuscules et des minuscules", MinimumLength = 6)]
        //[RegularExpression("^(?=.*[a-z])(?=.*[A-Z]).+$", ErrorMessage = "Entre 6 et 100 caractères et doit contenir des majuscules et des minuscules")]
        [Display(Name = "Mot de passe")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        [Compare("Password", ErrorMessage = "Le mot de passe ne correspond pas...")]
        public string ConfirmPassword { get; set; }
    }
}
