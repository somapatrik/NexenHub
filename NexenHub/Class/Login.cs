using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using NexenHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace NexenHub.Class
{
    public static class Login
    {

        public static bool IsLoggedIn(HttpContext context)
        {
            return context.User.Identity.IsAuthenticated;
        }

        public static string Username(HttpContext context)
        {
            return context.User.Identity.Name;
        }

        public static string Role(HttpContext context)
        {
            return context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault();
        }

        public static string NexenID(HttpContext context)
        {
            return context.User.Claims.Where(c => c.Type == "USER_ID").Select(c => c.Value).SingleOrDefault();
        }

        public static Member Member(HttpContext context)
        {
            string id = NexenID(context);
            return new Member(id);
        }

        public static async Task<bool> LogIn(HttpContext context, string username, string password)
        {
            bool logged = false;
            GlobalDatabase db = new GlobalDatabase();
            User user = db.Login(username, password);

            if (user.IsValid)
            {

                var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim("USER_ID", user.UserId),
                            new Claim(ClaimTypes.Role, "User")
                        };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    //IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                logged = true;

            }

            return logged;
        }

        public static async Task<bool> Logout(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return true;

        }

    }
}
