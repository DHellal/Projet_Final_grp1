using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GrandHotel_WebApplication.Models;
using GrandHotel_WebApplication.Models.ManageViewModels;
using GrandHotel_WebApplication.Services;
using GrandHotel_WebApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace GrandHotel_WebApplication.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly GrandHotelContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder,
          GrandHotelContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your password has been set.";

            return RedirectToAction(nameof(SetPassword));
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ExternalLoginsViewModel { CurrentLogins = await _userManager.GetLoginsAsync(user) };
            model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
            model.StatusMessage = StatusMessage;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(LinkLoginCallback));
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                throw new ApplicationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "The external login was added.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "The external login was removed.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            return View(nameof(Disable2fa));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            _logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var model = new EnableAuthenticatorViewModel
            {
                SharedKey = FormatKey(unformattedKey),
                AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("model.Code", "Verification code is invalid.");
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            _logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
            return RedirectToAction(nameof(GenerateRecoveryCodes));
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            var model = new GenerateRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

            _logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ChangeAccount()
        {

            var user = await _userManager.GetUserAsync(User);
            CreationClientVM clientVM = new CreationClientVM();


            if (ViewBag.statutmssg == null)
                clientVM.StatusMessage = "Bonjour !";
            else
                clientVM.StatusMessage = ViewBag.statutmssg;

            if (user == null)
            {
                clientVM.StatusMessage = " Erreur : Pas de client avec votre Email ... ";
                return RedirectToAction("Create", "Clients", clientVM);
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var hasEmail = await _userManager.GetEmailAsync(user);
            if (hasEmail == null)
            {
                return RedirectToAction("Register", "Account");
            }


            Client client = (from c in _context.Client
                             where c.Email == user.Email
                             select new Client
                             {
                                 Id = c.Id,
                                 Nom = c.Nom,
                                 Prenom = c.Prenom,
                                 Civilite = c.Civilite,
                                 Adresse = c.Adresse,
                                 Telephone = c.Telephone
                             }).SingleOrDefault();


            if (client == null)
            {
                return RedirectToAction("Create", "Clients");
            }
            else
            {
                CreationClientVM clientInter = new CreationClientVM()
                {
                    id = client.Id,
                    Nom = client.Nom,
                    Prenom = client.Prenom,
                    Civilite = client.Civilite,
                    StatusMessage = clientVM.StatusMessage

                };

                if (client.Adresse != null)
                {
                    clientInter.AdresseRue = client.Adresse.Rue;
                    clientInter.AdresseCodePostal = client.Adresse.CodePostal;
                    clientInter.AdresseVille = client.Adresse.Ville;
                }
                if (client.Telephone.Where(t => t.CodeType == "F") != null)
                {
                    clientInter.TelephoneDom = client.Telephone.Where(t => t.CodeType == "F").Select(t => t.Numero).SingleOrDefault();
                    clientInter.ProDom = client.Telephone.Where(t => t.CodeType == "F").Select(t => t.Pro).SingleOrDefault();
                }
                if (client.Telephone.Where(t => t.CodeType == "M") != null)
                {
                    clientInter.TelephonePort = client.Telephone.Where(t => t.CodeType == "M").Select(t => t.Numero).SingleOrDefault();
                    clientInter.ProPort = client.Telephone.Where(t => t.CodeType == "M").Select(t => t.Pro).SingleOrDefault();
                }
                return View(clientInter);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeAccount(CreationClientVM clientVM)
        {
            if (!ModelState.IsValid)
            {
                clientVM.StatusMessage = "Erreur : Veuillez rensignez toutes les coordonnées correctement";
                return View(clientVM);
            }

            var user = await _userManager.GetUserAsync(User);

            try
            {
                //Update Client
                #region
                Client clientAncien = await _context.Client.Where(c => c.Email == user.Email).SingleOrDefaultAsync();

                clientAncien.Civilite = clientVM.Civilite;
                clientAncien.Nom = clientVM.Nom.ToUpper();
                // Le nom et la premiere lettre du prénom en majuscule
                clientAncien.Prenom = clientVM.Prenom[0].ToString().ToUpper() + clientVM.Prenom.Substring(1);
                clientAncien.Email = user.Email;

                clientVM.id = clientAncien.Id;
                _context.Update(clientAncien);
                await _context.SaveChangesAsync();
                #endregion

                //Update Adresse
                if (clientVM.AdresseVille != null && clientVM.AdresseRue != null && clientVM.AdresseCodePostal != null)
                {
                    Adresse adresseAncienne = await _context.Adresse.Where(a => a.IdClient == clientVM.id).SingleOrDefaultAsync();

                    adresseAncienne.IdClient = clientVM.id;
                    adresseAncienne.Rue = clientVM.AdresseRue;
                    adresseAncienne.CodePostal = clientVM.AdresseCodePostal;
                    adresseAncienne.Ville = clientVM.AdresseVille.ToUpper();

                    _context.Update(adresseAncienne);
                    await _context.SaveChangesAsync();
                }

                //Update Telephones
                #region
                //Si numéro non identique
                if (clientVM.TelephoneDom != clientVM.TelephonePort)
                {
                    //Domicile
                    if (clientVM.TelephoneDom.Length == 10)
                    {
                        string telClient = await _context.Telephone.Where(t => t.IdClient == clientVM.id && t.CodeType == "F").Select(t => t.Numero).SingleOrDefaultAsync();
                        string telExistDeja = await _context.Telephone.Where(t => t.Numero == clientVM.TelephoneDom).Select(t => t.Numero).SingleOrDefaultAsync();
                        Telephone telDom = await _context.Telephone.Where(t => t.IdClient == clientVM.id && t.CodeType == "F").SingleOrDefaultAsync();

                        // Si le client n'avait pas de numéro
                        if (telClient == null)
                        {
                            Telephone telNouveau = new Telephone()
                            {
                                IdClient = clientVM.id,
                                CodeType = "F",
                                Numero = clientVM.TelephoneDom,
                                Pro = clientVM.ProDom
                            };

                            _context.Telephone.Add(telNouveau);
                            await _context.SaveChangesAsync();
                        }
                        // si le numéro n'existe pas dans la BDD ET que le client a déja un numéro
                        else if (telExistDeja == null)
                        {
                            _context.Remove(telDom);
                            await _context.SaveChangesAsync();
                            telDom.IdClient = clientVM.id;
                            telDom.Numero = clientVM.TelephoneDom;
                            telDom.Pro = clientVM.ProDom;
                            telDom.CodeType = "F";

                            _context.Add(telDom);
                            await _context.SaveChangesAsync();
                        }
                        else if (clientVM.TelephoneDom != telDom.Numero)
                        {
                            clientVM.TelephonePort = "";
                            ViewBag.statutmssg = "Erreur : Numero de telephone Portable déja utilisé..";
                            return View();
                        }
                    }

                    //Portable
                    if (clientVM.TelephonePort.Length == 10)
                    {
                        string telClient = await _context.Telephone.Where(t => t.IdClient == clientVM.id && t.CodeType == "M").Select(t => t.Numero).SingleOrDefaultAsync();
                        string telExist = await _context.Telephone.Where(t => t.Numero == clientVM.TelephonePort).Select(t => t.Numero).SingleOrDefaultAsync();
                        Telephone telPort = await _context.Telephone.Where(t => t.IdClient == clientVM.id && t.CodeType == "M").SingleOrDefaultAsync();


                        // Si le client n'avait pas de numéro
                        if (telClient == null)
                        {
                            Telephone telNouveau = new Telephone()
                            {
                                IdClient = clientVM.id,
                                CodeType = "M",
                                Numero = clientVM.TelephonePort,
                                Pro = clientVM.ProPort
                            };

                            _context.Telephone.Add(telNouveau);
                            await _context.SaveChangesAsync();
                        }
                        // si le numéro n'existe pas dans la BDD ET que le client a déja un numéro
                        else if (telExist == null)
                        {
                            _context.Remove(telPort);
                            await _context.SaveChangesAsync();

                            telPort.IdClient = clientVM.id;
                            telPort.Numero = clientVM.TelephonePort;
                            telPort.Pro = clientVM.ProPort;
                            telPort.CodeType = "M";

                            _context.Add(telPort);
                            await _context.SaveChangesAsync();
                        }
                        else if (clientVM.TelephonePort != telPort.Numero)
                        {
                            clientVM.TelephonePort = "";
                            ViewBag.statutmssg = "Erreur : Numero de telephone Portable déja utilisé..";
                            return View();
                        }
                    }
                }
                else
                {
                    ViewBag.statutmssg = "Erreur : Numeros identiques...";
                    return View();
                }
                #endregion
            }
            catch (Exception e)
            {
                ViewBag.statutmssg = "Erreur : " + e.Message;
                return View();
            }

            ViewBag.statutmssg = "Compte modifié avec succés";
            return View();
        }




        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenicatorUriFormat,
                _urlEncoder.Encode("GrandHotel_WebApplication"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        #endregion
    }

}