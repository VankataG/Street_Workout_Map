// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using StreetWorkoutMap.Data;

namespace StreetWorkoutMap.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required(ErrorMessage = "Моля въведете име.")]
            [StringLength(50)]
            [Display(Name = "Име")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Моля въведете фамилия.")]
            [StringLength(50)]
            [Display(Name = "Фамилия")]
            public string LastName { get; set; } = string.Empty;


            [Required(ErrorMessage = "Моля въведете имейл адрес.")]
            [EmailAddress]
            [Display(Name = "Имейл")]
            public string Email { get; set; }

            
            [Required]
            [StringLength(100, ErrorMessage = "Паролата трябва да бъде между {2} и {1} символа.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Парола")]
            public string Password { get; set; }

            
            [DataType(DataType.Password)]
            [Display(Name = "Потвърди парола")]
            [Compare("Password", ErrorMessage = "Паролите не съвпадат.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.FirstName = Input.FirstName.Trim();
                user.LastName = Input.LastName.Trim();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var roleResult = await _userManager.AddToRoleAsync(user, "User");

                    if (!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        return Page();
                    }

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    var encodedCallbackUrl =
    HtmlEncoder.Default.Encode(callbackUrl);

                    var emailBody = $"""
<!DOCTYPE html>
<html lang="bg">
<head>
    <meta charset="UTF-8">
</head>
<body style="
    margin:0;
    padding:0;
    background-color:#0b0f0c;
    font-family:Arial,Helvetica,sans-serif;
    color:#f2f7f3;
">

    <table role="presentation"
           width="100%"
           cellspacing="0"
           cellpadding="0"
           style="background-color:#0b0f0c;padding:32px 16px;">

        <tr>
            <td align="center">

                <table role="presentation"
                       width="100%"
                       cellspacing="0"
                       cellpadding="0"
                       style="
                           max-width:560px;
                           background-color:#111713;
                           border:1px solid #28352b;
                           border-radius:18px;
                           overflow:hidden;
                       ">

                    <tr>
                        <td style="
                            padding:28px 30px 20px;
                            text-align:center;
                        ">

                            <div style="
                                color:#72ff6a;
                                font-size:28px;
                                font-weight:900;
                                letter-spacing:1px;
                            ">
                                SW-MAP
                            </div>

                            <div style="
                                margin-top:5px;
                                color:#9eada1;
                                font-size:13px;
                            ">
                                Street Workout Map Bulgaria
                            </div>

                        </td>
                    </tr>

                    <tr>
                        <td style="padding:8px 30px 30px;">

                            <h1 style="
                                margin:0 0 16px;
                                color:#f2f7f3;
                                font-size:24px;
                                line-height:1.3;
                            ">
                                Потвърди имейл адреса си
                            </h1>

                            <p style="
                                margin:0 0 14px;
                                color:#c5d0c7;
                                font-size:15px;
                                line-height:1.6;
                            ">
                                Здравей, {HtmlEncoder.Default.Encode(Input.FirstName)}!
                            </p>

                            <p style="
                                margin:0 0 24px;
                                color:#c5d0c7;
                                font-size:15px;
                                line-height:1.6;
                            ">
                                Благодарим ти, че се регистрира в SW-MAP.
                                Потвърди имейл адреса си, за да активираш профила си.
                            </p>

                            <div style="text-align:center;margin:28px 0;">

                                <a href="{encodedCallbackUrl}"
                                   style="
                                       display:inline-block;
                                       padding:14px 24px;
                                       border-radius:12px;
                                       background-color:#72ff6a;
                                       color:#071008;
                                       font-size:15px;
                                       font-weight:800;
                                       text-decoration:none;
                                   ">
                                    Потвърди имейла
                                </a>

                            </div>

                            <p style="
                                margin:24px 0 0;
                                color:#9eada1;
                                font-size:13px;
                                line-height:1.6;
                            ">
                                Ако не си създавал профил в SW-MAP,
                                можеш спокойно да игнорираш този имейл.
                            </p>

                        </td>
                    </tr>

                    <tr>
                        <td style="
                            padding:18px 30px;
                            border-top:1px solid #28352b;
                            color:#77857a;
                            font-size:12px;
                            text-align:center;
                        ">
                            © 2026 SW-MAP
                        </td>
                    </tr>

                </table>

            </td>
        </tr>

    </table>

</body>
</html>
""";

                    await _emailSender.SendEmailAsync(
                        Input.Email,
                        "Потвърди регистрацията си в SW-MAP",
                        emailBody);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
