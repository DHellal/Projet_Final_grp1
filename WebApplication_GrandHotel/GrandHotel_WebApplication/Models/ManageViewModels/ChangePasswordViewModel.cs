using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GrandHotel_WebApplication.Models.ManageViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe actuel")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Nouveau mot de passe requis")]
        [StringLength(100, ErrorMessage = "Entre 6 et 100 caractères et doit contenir des majuscules et des minuscules", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe ")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmez le mot de passe ")]
        [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas...")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }
    }
}
