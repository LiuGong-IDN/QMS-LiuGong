using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure;
using QCard.ManagementSystem.Infrastructure.Cache;
using QCard.ManagementSystem.Infrastructure.Session;
using QCard.ManagementSystem.Service;
using QCard.ManagementSystem.Service.Modules.Core.Service;
using QCard.ManagementSystem.Web;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Areas.Core.Controllers
{
    [Area("Core")]
    public class AccountController : WebController
    {
        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("~/");
            }
            var model = new LoginViewModel { ReturnUrl = returnUrl };

            return View("Login", model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string SignOnToken, LoginViewModel model)
        {
            // Update menu cache
            ApplicationCacheManager.RefreshRolePermissions();
            ApplicationCacheManager.RefreshMenu();

            IDbHelper dbHelper = new DbHelper();

            try
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    IsEssential = true                          
                };

                if (string.IsNullOrEmpty(model.PhoneNumberOrEmail)) throw new ModelException("PhoneNumberOrEmail", "Please fill in the Email / Mobile Phone field");
                if (string.IsNullOrEmpty(model.UserPwd)) throw new ModelException("UserPwd", "Please fill in the Password field");

                var (phoneNumber, email) = Service<DataSourceService>().ParseLoginInput(model.PhoneNumberOrEmail);

                if (User.Identity.IsAuthenticated)
                {
                    // Redirect to homepage.
                    return Redirect("~/Home");
                }

                var authenticate = Service<UserService>().Authenticate(model);
                if (authenticate.Success)
                {
                    var user = Service<UserService>().GetUserByPhoneEmail(phoneNumber, email);
                    var userRoles = Service<UserService>().GetUserRoleByUsername(user.ID);
                    userRoles = (userRoles.Count == 1 && userRoles[0] == "BLANK") ? [] : userRoles;
                    if (userRoles.Count == 0)
                    {
                        return Redirect("~/Error?statusCode=401");
                    }
                    var claims = new[]
                    {
                            new Claim("Name", user.FIRST_NAME),
                            new Claim("PhoneNumber", user.PHONE_NUMBER),
                            new Claim("Email", user.EMAIL),
                            new Claim("Roles", string.Join(",", userRoles))
                        };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    });

                    var apiAuth = Service<UserService>().GetToken(user);
                    string token = string.Empty;
                    if (apiAuth != null)
                    {
                        if (!string.IsNullOrEmpty(apiAuth.Token))
                        {
                            token = apiAuth.Token;
                        }
                    }

                    var userSession = new UserSession
                    {
                        Id = user.ID,
                        Current = user.ID.ToString(),
                        Name = user.FIRST_NAME,
                        Password = MD5Helper.Encode(user.PASSWORD),
                        RoleStr = string.Join(",", userRoles),
                        FirstName = user.FIRST_NAME,
                        LastName = user.LAST_NAME,
                        Company = user.COMPANY,
                        CompanyId = user.COMPANY_ID,
                        CardNo = user.CARD_NO,
                        Email = user.EMAIL,
                        Token = token,
                        Pdc = user.PDC,
                        PdcId = user.PDC_ID,
                    };

                    HttpContext.Session.SetString(AppConstants.SessionKey, JsonConvert.SerializeObject(userSession));

                    var userSessionJson = JsonConvert.SerializeObject(userSession);
                    var encodedValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(userSessionJson));
                    HttpContext.Response.Cookies.Append(AppConstants.SessionKey, encodedValue, cookieOptions);
                }
                else
                {
                    throw new ModelException("UserPwd", "Incorrect Username or Password");
                }

                if (string.IsNullOrEmpty(model.ReturnUrl) || !Url.IsLocalUrl(model.ReturnUrl))
                {
                    model.ReturnUrl = "~/Home";
                }

                new ActivityLogService(dbHelper).CreateSuccessLog(ControllerContext.ActionDescriptor.ActionName, ControllerContext.ActionDescriptor.ControllerName);
                return Redirect(model.ReturnUrl);
            }
            catch (ModelException ex)
            {
                var title = "";
                if (ex.Title == "Password")
                {
                    title = "UserPwd";
                }
                else
                {
                    title = ex.Title;
                }
                ModelState.AddModelError(ex.Title, ex.Message);

                new ActivityLogService(dbHelper).CreateErrorLog(ControllerContext.ActionDescriptor.ActionName, ControllerContext.ActionDescriptor.ControllerName, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            IDbHelper dbHelper = new DbHelper();

            return Redirect("~/");
        }

        [HttpPost]
        public async Task<IActionResult> CheckTokenExpired()
        {
            string token = string.Empty;
            var usersession = SessionManager.UserSession;

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(usersession.Token);
            var validTo = jwtSecurityToken.ValidTo;
            token = usersession.Token;

            if (validTo < DateTime.UtcNow)
            {
                //// expired
                //var user = Service<UserService>().GetUserByUsername(usersession.Username);
                //var apiAuth = Service<UserService>().GetToken(user);
                //if (apiAuth != null)
                //{
                //    if (!string.IsNullOrEmpty(apiAuth.Token))
                //    {
                //        token = apiAuth.Token;
                //    }
                //}
                //return Json(new { token = token });
                Logout();
            }

            return Json(new { token });
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new UserRequest();

            return View("Register", model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRequest model)
        {
            var result = await Service<UserService>().Register(model);
            return Json(result);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            var model = new ForgotPasswordViewModel();

            return View("ForgotPassword", model);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ForgotPassword(ResetPasswordLinkRequest request)
        {
            return new JsonResult(Service<UserService>().SendResetLink(request.Email, request.BaseUrl));
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View("ResetPassword");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ResetPassword(ResetPasswordUpdateRequest request)
        {
            return new JsonResult(Service<UserService>().ResetPassword(request));
        }
    }
}