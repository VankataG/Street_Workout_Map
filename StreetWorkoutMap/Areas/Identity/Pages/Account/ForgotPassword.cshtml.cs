// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using StreetWorkoutMap.Data;

namespace StreetWorkoutMap.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
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
           style="
               width:100%;
               background-color:#0b0f0c;
               padding:32px 16px;
           ">

        <tr>
            <td align="center">

                <table role="presentation"
                       width="100%"
                       cellspacing="0"
                       cellpadding="0"
                       style="
                           width:100%;
                           max-width:560px;
                           background-color:#111713;
                           border:1px solid #28352b;
                           border-radius:18px;
                           overflow:hidden;
                       ">

                    <!-- Header -->
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


                    <!-- Content -->
                    <tr>
                        <td style="
                            padding:8px 30px 30px;
                        ">

                            <h1 style="
                                margin:0 0 16px;
                                color:#f2f7f3;
                                font-size:24px;
                                line-height:1.3;
                            ">
                                Задай нова парола
                            </h1>

                            <p style="
                                margin:0 0 14px;
                                color:#c5d0c7;
                                font-size:15px;
                                line-height:1.6;
                            ">
                                Получихме заявка за промяна на паролата
                                на твоя SW-MAP профил.
                            </p>

                            <p style="
                                margin:0 0 24px;
                                color:#c5d0c7;
                                font-size:15px;
                                line-height:1.6;
                            ">
                                Натисни бутона по-долу, за да избереш
                                нова парола.
                            </p>


                            <!-- Button -->
                            <div style="
                                margin:28px 0;
                                text-align:center;
                            ">

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
                                    Смени паролата
                                </a>

                            </div>


                            <!-- Security notice -->
                            <div style="
                                margin-top:26px;
                                padding:14px 16px;
                                border:1px solid #28352b;
                                border-radius:12px;
                                background-color:#18221a;
                            ">

                                <div style="
                                    margin-bottom:5px;
                                    color:#f2f7f3;
                                    font-size:13px;
                                    font-weight:700;
                                ">
                                    Не си поискал промяна?
                                </div>

                                <div style="
                                    color:#9eada1;
                                    font-size:13px;
                                    line-height:1.55;
                                ">
                                    Ако не си поискал нова парола,
                                    можеш спокойно да игнорираш този имейл.
                                    Паролата ти няма да бъде променена,
                                    докато не използваш линка по-горе.
                                </div>

                            </div>

                        </td>
                    </tr>


                    <!-- Footer -->
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
                    "Смяна на паролата в SW-MAP",
                    emailBody);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
