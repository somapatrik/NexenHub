using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using NexenHub.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data;

namespace NexenHub.Class
{
    public static class GlobalSettings
    {
        public static bool Test;
        public static string DatabaseConnection;
        public static string DatabaseConnectionDevelopment;
        public static string DatabaseControlling;
        public static string DatabaseAktion;
        public static string DatabaseNXMSQL;

        public static string CzechNum = "### ### ### ###.##";

        public static string PLANT_ID = "P500";

        public static string[] IgnoredMachines = {
                "10004",
                "10005",
                "10006",
                "10007",
                "10017",
                "10024",
                "10032"
            };

        public static string[] OEEIgnoredMachines = {
                "10004",
                "10005",
                "10006",
                "10007",
                "10017",
                "10024",
                "10032",
                "10013",
                "10136",
                "10137",
                "10138",
                "10139",
            };

        public static string ImageTireLogo(string pattern)
        {
            switch (pattern.ToLower())
            {
                case "n'blue 4season":
                    return "nblue_4season.png";

                case "n'blue 4season 2":
                    return "nblue_4season2.png";

                case "n'blue hd Plus":
                    return "nblue_hd_plus.png";

                case "n'fera primus":
                    return "nfera_primus.png";

                case "n'fera sport":
                case "n'fera sport as":
                case "n'fera sport suv":
                    return "nfera_sport.png";

                case "n'fera ru1":
                case "n'fera au7":
                    return "nfera_su1.png";

                case "winguard snow'g 3":
                    return "wg_snowg3.png";

                case "winguard sport 2":
                case "winguard sport 2 SUV":
                    return "wg_sport2.png";

                default:
                    return "";
            }
        }

        public static async Task<bool> Login(HttpContext context, string username, string password)
        {
            bool logged = false;
            GlobalDatabase db = new GlobalDatabase();
            User user = db.Login(username, password);

            if (!string.IsNullOrEmpty(user.UserId))
            {

                var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim("USER_ID", user.UserId),
                            new Claim(ClaimTypes.Role, "Administrator"),
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
            }

            return logged;
        }

        public static string CodeToGroup(string NONWRK_CODE)
        {
            string group = "Other";

            switch (NONWRK_CODE)
            {
                case "N016":
                    group = "PM";
                    break;
                case "N058":
                case "N131":
                    group = "Test";
                    break;
                case "N065":
                    group = "No action";
                    break;
            }

            return group;
        }
    }
}
