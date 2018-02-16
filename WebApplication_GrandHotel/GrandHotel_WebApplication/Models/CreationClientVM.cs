using GrandHotel_WebApplication.Models.AccountViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace GrandHotel_WebApplication.Models
{
    public class CreationClientVM : IValidatableObject
    {

        [Required(ErrorMessage = "Requis")]
        public string Civilite { get; set; }
        [Required(ErrorMessage = "Requis")]
        public string Nom { get; set; }
        [Required(ErrorMessage = "Requis")]
        public string Prenom { get; set; }

        [Display(Name = "Rue")]
        public string AdresseRue { get; set; }
        [Display(Name = "Code Postal")]
        [RegularExpression("^(?:[0-9]{5}|)$", ErrorMessage = "Code de 5 chiffres")]
        public string AdresseCodePostal { get; set; }
        [Display(Name = "Ville")]
        public string AdresseVille { get; set; }

        [Display(Name ="Telephone Domicile")]
        [RegularExpression("(0[1-6]{1}[0-9]{8})|", ErrorMessage = "10 chiffres commençant par 01-06")]
        public string TelephoneDom { get; set; }

        [Display(Name = "Cochez si numéro professionel")]
        public bool ProDom { get; set; }

        [RegularExpression("(0[6-7]{1}[0-9]{8})|", ErrorMessage = "10 chiffres commençant par 06 ou 07")]
        [Display(Name = "Telephone Portable")]
        public string TelephonePort { get; set; }
        [Display(Name = "Cochez si numéro professionel")]
        public bool ProPort { get; set; }

        public string Email { get; set; }

        public string StatusMessage { get; set; }

        public bool MAJ { get; set; }

        public int id;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            CreationClientVM cli = (CreationClientVM)validationContext.ObjectInstance;

            if(cli.TelephoneDom == null)
            {
                cli.TelephoneDom = "";
            }
            if (cli.TelephonePort == null)
            {
                cli.TelephonePort = "";
            }


            if (cli.TelephoneDom.Length != 10 && cli.TelephonePort.Length != 10)
            {
                yield return new ValidationResult("Au moins un numéro doit etre renseigné correctement", new string[] { "TelephoneDom", "TelephonePort" });
            }
        }
    }


}




