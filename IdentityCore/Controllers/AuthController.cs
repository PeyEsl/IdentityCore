﻿using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Authentication.Google;
using IdentityCore.Models.ViewModels;
using IdentityCore.Models.DTOs;
using IdentityCore.Services;

namespace IdentityCore.Controllers
{
    //[AllowAnonymous, Route("authentication")]
    public class AuthController : Controller
    {
        #region Ctor

        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IViewRenderService _viewRenderService;
        private readonly IEmailService _emailService;

        public AuthController(IAuthService authService,
            IUserService userService,
            IViewRenderService viewRenderService,
            IEmailService emailService)
        {
            _authService = authService;
            _userService = userService;
            _viewRenderService = viewRenderService;
            _emailService = emailService;
        }

        #endregion

        public IActionResult LoginFailed()
        {
            return View();
        }
        
        public IActionResult Register()
        {
            return View(new UserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password == model.ConfirmPassword && model.AcceptPolicy)
                {
                    var userDto = new UserDto
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        UserName = model.UserName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Password = model.Password,
                        ConfirmPassword = model.ConfirmPassword,
                        AcceptPolicy = model.AcceptPolicy,
                    };

                    var result = await _userService.RegisterUserAsync(userDto);
                    if (result.Succeeded)
                    {
                        if (model.SendLinkByEmail)
                        {
                            var token = await _authService.GenerateEmailConfirmationAsync(model.Email!);

                            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                            string? callBackUrl = Url.ActionLink("ConfirmEmail", "Auth", new { email = model.Email, token = token!, resetPassword = false }, Request.Scheme);

                            var confirmModel = new ConfirmViewModel
                            {
                                Email = model.Email,
                                Code = string.Empty,
                                Token = token,
                                CallBackUrl = callBackUrl,
                                ResetPassword = false,
                                IsConfirmed = false,
                            };

                            string body = await _viewRenderService.RenderToStringAsync("_ConfirmationEmail", confirmModel);

                            _emailService.SendEmail(model.Email!, "Account Verification", body);

                            return RedirectToAction("Index", "Home", new { isSent = true });
                        }
                        else
                        {
                            var user = await _userService.GetUserByPhoneAsync(model.PhoneNumber!);
                            if (user == null)
                            {
                                ModelState.AddModelError(string.Empty, "User not found.");

                                return View();
                            }

                            var code = await _authService.GenerateTwoFactorAsync(user, "Phone");
                            if (code != "Code not found.")
                            {
                                return RedirectToAction("ConfirmMobile", new { phoneNumber = model!.PhoneNumber, token = code, resetPassword = false });
                            }
                        }
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);

                        return View();
                    }
                }
            }

            return View(model);
        }
        
        public IActionResult RegisterWithPhone()
        {
            return View(new RegisterWithPhoneViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterWithPhone(RegisterWithPhoneViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.AcceptPolicy)
                {
                    var registerWithPhoneDto = new RegisterWithPhoneDto
                    {
                        PhoneNumber = model.PhoneNumber,
                        AcceptPolicy = model.AcceptPolicy,
                    };

                    var result = await _userService.RegisterWithPhoneAsync(registerWithPhoneDto);
                    if (result.Succeeded)
                    {
                        var user = await _userService.GetUserByPhoneAsync(model.PhoneNumber!);

                        var code = await _authService.GenerateTwoFactorAsync(user, "Phone");
                        if (user != null && code != "Code not found.")
                        {
                            return RedirectToAction("ConfirmMobile", new { phoneNumber = model!.PhoneNumber, token = code, resetPassword = false });
                        }
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);

                        return View();
                    }
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(ConfirmViewModel model)
        {
            if (model.Email != null && model.Token != null)
            {
                var user = await _userService.GetUserByEmailAsync(model.Email);
                if (user != null)
                {
                    if (model.ResetPassword)
                    {
                        model.ResetPassword = false;

                        var resetPasswordDto = new ResetPasswordDto
                        {
                            UserId = user.Id,
                            Email = model.Email,
                        };

                        var confirmModel = new ConfirmViewModel
                        {
                            Email = model.Email,
                            Token = model.Token,
                            Code = model.Code,
                            CallBackUrl = model.CallBackUrl,
                            ResetPassword =true,
                            IsConfirmed = true,
                        };

                        return View(confirmModel);
                    }
                    else
                    {
                        model.Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));

                        var result = await _authService.EmailConfirmAsync(user, model.Token);

                        model.IsConfirmed = result.Succeeded ? true : false;

                        return View(model);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmEmail(ConfirmViewModel model, bool resetPassword = false)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByEmailAsync(model.Email!);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found.");

                    model.Email = model.Email;
                    model.Token = model.Token;
                    model.ResetPassword = true;
                    model.IsConfirmed = true;

                    return View(model);
                }

                var confirmDto = new ConfirmDto
                {
                    UserId = user.Id,
                    Email = model.Email,
                    Token = model.Token,
                    Code = model.Code,
                    ResetPassword = resetPassword,
                    TokenProvider = "Email",
                };

                var result = await _authService.VerifyTwoFactorAsync(confirmDto);
                if (result)
                {
                    model.ResetPassword = false;

                    return RedirectToAction("ResetPassword", "Auth", new { phoneNumber = string.Empty, email = model.Email, token = model.Token });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The entered code is not valid.");

                    return View(model);
                }
            }
            else
            {
                return BadRequest();
            }
        }

        public IActionResult ConfirmMobile(string phoneNumber, string token, bool resetPassword)
        {
            if (phoneNumber != null && token != null)
            {
                var model = new ConfirmViewModel
                {
                    PhoneNumber = phoneNumber,
                    ResetPassword = resetPassword,
                    Code = token,
                };

                return View(model);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmMobile(ConfirmViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByPhoneAsync(model.PhoneNumber!);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found.");

                    return View();
                }

                var confirmDto = new ConfirmDto
                {
                    UserId = user.Id,
                    PhoneNumber = model.PhoneNumber,
                    ResetPassword = model.ResetPassword,
                    Token = model.Token,
                    Code = model.SendCode,
                    TokenProvider = "Phone",
                };

                var result = await _authService.VerifyTwoFactorAsync(confirmDto);
                if (result)
                {
                    if (model.ResetPassword)
                    {
                        model.ResetPassword = false;

                        var resetPasswordDto = new ResetPasswordDto
                        {
                            UserId = user.Id,
                            PhoneNumber = model.PhoneNumber,
                        };

                        var code = await _authService.GeneratePasswordResetAsync(resetPasswordDto);

                        return RedirectToAction("ResetPassword", "Auth", new { phoneNumber = model.PhoneNumber, email = string.Empty, token = code });
                    }
                    else
                    {
                        await _authService.PhoneNumberConfirmedAsync(user.Id!);

                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The entered code is not valid.");

                    return View(model);
                }
            }

            return View(model);
        }

        public IActionResult Login(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            model.ReturnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var userByName = await _userService.GetUserByUserNameAsync(model.UserName!);

                var userByPhone = await _userService.GetUserByPhoneAsync(model.UserName!);

                var userByEmail = await _userService.GetUserByEmailAsync(model.UserName!);

                if (userByName == null && userByPhone == null && userByEmail == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found.");

                    return View(model);
                }

                var loginDto = new LoginDto();

                if (userByName!.UserName != null)
                {
                    loginDto.UserName = userByName.UserName;
                    loginDto.Password = model.Password;
                    loginDto.RememberMe = model.RememberMe;
                }
                else if (userByPhone!.PhoneNumber != null)
                {
                    loginDto.UserName = userByPhone.PhoneNumber;
                    loginDto.Password = model.Password;
                    loginDto.RememberMe = model.RememberMe;
                }
                else
                {
                    loginDto.UserName = userByEmail.Email;
                    loginDto.Password = model.Password;
                    loginDto.RememberMe = model.RememberMe;
                }

                var result = await _authService.LoginUserAsync(loginDto);
                if (result.Succeeded)
                {
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else if (result.RequiresTwoFactor)
                {
                    return View("LoginFailed");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Your account is locked.");

                    return View(model);
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                return RedirectToAction("LoginConfirmEmail");
            }

            return View(model);
        }

        public IActionResult LoginByMobile(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var model = new LoginWithPhoneViewModel
            {
                ReturnUrl = returnUrl,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginByMobile(LoginWithPhoneViewModel model)
        {
            model.ReturnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var loginWithPhoneDto = new LoginWithPhoneDto
                {
                    Phone = model.Phone,
                    RememberMe = model.RememberMe,
                };

                var user = await _userService.GetUserByPhoneAsync(model.Phone!);

                var code = await _authService.GenerateTwoFactorAsync(user, "Phone");
                if (user != null && code != "Code not found.")
                {
                    return RedirectToAction("LoginConfirmMobile", new { phone = model!.Phone, token = code });
                }
            }

            return View(model);
        }

        public IActionResult LoginConfirmMobile(string phone, string token)
        {
            if (phone != null && token != null)
            {
                var model = new ConfirmViewModel
                {
                    PhoneNumber = phone,
                    Code = token,
                };

                return View(model);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginConfirmMobile(ConfirmViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByPhoneAsync(model.PhoneNumber!);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found.");

                    return View(model);
                }

                var confirmDto = new ConfirmDto
                {
                    UserId = user.Id,
                    PhoneNumber = model.PhoneNumber,
                    ResetPassword = model.ResetPassword,
                    Token = model.Token,
                    Code = model.SendCode,
                    TokenProvider = "Phone",
                };

                var result = await _authService.VerifyTwoFactorAsync(confirmDto);
                if (result)
                {
                    await _authService.SignInAsync(user, true);

                    return Redirect("/");
                }

                ModelState.AddModelError(string.Empty, "The entered code is not valid.");

                return View(model);
            }

            return View(model);
        }

        public IActionResult LoginConfirmEmail()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginConfirmEmail(string email)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found.");

                    return View(email);
                }

                var token = await _authService.GenerateEmailConfirmationAsync(email);

                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                string? callBackUrl = Url.ActionLink("ConfirmEmail", "Auth", new { email = email, token = token!, resetPassword = false }, Request.Scheme);

                var confirmModel = new ConfirmViewModel
                {
                    Email = email,
                    Code = string.Empty,
                    Token = token,
                    CallBackUrl = callBackUrl,
                    ResetPassword = false,
                    IsConfirmed = false,
                };

                string body = await _viewRenderService.RenderToStringAsync("_ConfirmationEmail", confirmModel);

                _emailService.SendEmail(email!, "Account Verification", body);

                return RedirectToAction("Index", "Home", new { isSent = true });
            }

            return View(email);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutUserAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SendResetLinkByEmail)
                {
                    var user = await _userService.GetUserByEmailAsync(model.Email!);

                    var code = await _authService.GenerateTwoFactorAsync(user, "Email");

                    var resetPasswordDto = new ResetPasswordDto
                    {
                        UserId = user.Id,
                        Email = model.Email,
                    };

                    var token = await _authService.GeneratePasswordResetAsync(resetPasswordDto);
                    if (user == null && code == "Code not found." && token == null)
                    {
                        ModelState.AddModelError(string.Empty, "Attempt to send code failed.");

                        return View();
                    }

                    string? callBackUrl = Url.ActionLink("ConfirmEmail", "Auth", new { email = model.Email, token = token!, code = code!, resetPassword = true }, Request.Scheme);

                    var confirmModel = new ConfirmViewModel
                    {
                        Email = model.Email,
                        Code = code,
                        Token = token,
                        CallBackUrl = callBackUrl,
                        ResetPassword = true,
                        IsConfirmed = true,
                    };

                    string body = await _viewRenderService.RenderToStringAsync("_ConfirmationEmail", confirmModel);

                    _emailService.SendEmail(model.Email!, "Forgot Password", body);

                    return RedirectToAction("ConfirmEmail", "Auth", confirmModel);
                }
                else
                {
                    if (model.PhoneNumber != null)
                    {
                        var user = await _userService.GetUserByPhoneAsync(model.PhoneNumber!);

                        var code = await _authService.GenerateTwoFactorAsync(user, "Phone");
                        if (user == null && code == "Code not found.")
                        {
                            ModelState.AddModelError(string.Empty, "Attempt to send code failed.");

                            return View();
                        }

                        var confirmModel = new ConfirmViewModel
                        {
                            PhoneNumber = user!.PhoneNumber,
                            Email = user.Email,
                            ResetPassword = true,
                            Token = code,
                        };

                        return RedirectToAction("ConfirmMobile", "Auth", confirmModel);
                    }
                    else
                    {
                        var user = await _userService.GetUserByEmailAsync(model.Email!);

                        var code = await _authService.GenerateTwoFactorAsync(user, "Phone");
                        if (user == null && code == "Code not found.")
                        {
                            ModelState.AddModelError(string.Empty, "Attempt to send code failed.");

                            return View();
                        }

                        var confirmModel = new ConfirmViewModel
                        {
                            PhoneNumber = user!.PhoneNumber,
                            Email = user.Email,
                            ResetPassword = true,
                            Token = code,
                        };

                        return RedirectToAction("ConfirmMobile", "Auth", confirmModel);
                    }
                }
            }

            return View(model);
        }

        public IActionResult ResetPassword(string phoneNumber, string email, string token)
        {
            var resetPasswordVM = new ResetPasswordViewModel
            {
                PhoneNumber = phoneNumber,
                Email = email,
                Token = token,
            };

            return View(resetPasswordVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var resetPasswordDto = new ResetPasswordDto
                {
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    OldPassword = model.OldPassword,
                    NewPassword = model.NewPassword,
                    ConfirmPassword = model.ConfirmPassword,
                    Token = model.Token,
                };

                var resultPassword = await _authService.CheckPasswordAsync(resetPasswordDto);
                if (resultPassword && model.NewPassword == model.ConfirmPassword)
                {
                    var result = await _authService.ResetPasswordAsync(resetPasswordDto);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsAnyUserName(string userName)
        {
            bool any = await _userService.IsAnyUserName(userName);
            if (!any)
                return Json(true);

            return Json("Username is already registered.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsAnyPhone(string phone)
        {
            bool any = await _userService.IsAnyPhone(phone);
            if (!any)
                return Json(true);

            return Json("Phone is already registered.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsAnyEmail(string email)
        {
            bool any = await _userService.IsAnyEmail(email);
            if (!any)
                return Json(true);

            return Json("Email is already registered.");
        }
    }
}
