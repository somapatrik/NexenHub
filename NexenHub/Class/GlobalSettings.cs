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
                case "N001":
                case "N002":
                case "N003":
                case "N004":
                case "N119":
                case "N120":
                case "N121":
                case "N122":
                case "N136":
                case "N137":
                case "N138":
                case "N139":
                case "N140":
                case "N141":
                case "N142":
                case "N143":
                case "N144":
                case "N145":
                case "N146":
                case "N155":
                case "N156":
                case "N273":
                case "N274":
                case "N275":
                case "N276":
                case "N277":
                case "N278":
                case "N279":
                case "N280":
                case "N281":
                case "N282":
                case "N283":
                case "N284":
                case "N285":
                case "N286":
                case "N287":
                case "N288":
                case "N289":
                case "N290":
                case "N291":
                case "N292":
                case "N293":
                case "N294":
                case "N295":
                case "N296":
                case "N297":
                case "N298":
                case "N299":
                case "N300":
                case "N301":
                case "N302":
                case "N303":
                case "N304":
                case "N305":
                case "N306":
                case "N307":
                case "N308":
                case "N309":
                case "N310":
                case "N311":
                case "N312":
                case "N313":
                case "N314":
                case "N315":
                case "N316":
                case "N317":
                case "N318":
                case "N319":
                case "N320":
                case "N321":
                case "N322":
                case "N323":
                case "N324":
                case "N325":
                case "N326":
                case "N327":
                case "N328":
                case "N329":
                case "N444":
                case "N445":
                case "N453":
                case "N454":
                case "N455":
                case "N456":
                case "N457":
                case "N458":
                case "N459":
                case "N460":
                case "N461":
                case "N463":
                case "N464":
                case "N465":
                case "N466":
                case "N467":
                case "N468":
                case "N469":
                case "N470":
                case "N471":
                case "N472":
                case "N473":
                case "N474":
                case "N475":
                case "N530":
                case "N531":
                case "N532":
                case "N533":
                case "N534":
                case "N552":
                case "N123":
                case "N124":
                case "N125":
                case "N126":
                case "N147":
                case "N148":
                case "N149":
                case "N150":
                case "N151":
                case "N152":
                case "N153":
                case "N154":
                case "N535":
                case "N536":
                case "N537":
                case "N538":
                case "N539":
                case "N540":
                case "N541":
                case "N542":
                case "N543":
                case "N186":
                case "N187":
                case "N188":
                case "N189":
                case "N190":
                case "N191":
                case "N192":
                case "N193":
                case "N209":
                case "N210":
                case "N211":
                case "N212":
                case "N213":
                case "N220":
                case "N221":
                case "N176":
                case "N177":
                case "N178":
                case "N179":
                case "N180":
                case "N181":
                case "N182":
                case "N185":
                case "N214":
                case "N217":
                case "N218":
                case "N219":
                case "N222":
                case "N488":
                case "N489":
                    group = "Malfunction";
                    break;
                case "N000":
                case "N005":
                case "N006":
                case "N007":
                case "N008":
                case "N009":
                case "N010":
                case "N011":
                case "N012":
                case "N013":
                case "N014":
                case "N127":
                case "N128":
                case "N133":
                case "N157":
                case "N160":
                case "N161":
                case "N226":
                case "N336":
                case "N337":
                case "N338":
                case "N339":
                case "N340":
                case "N341":
                case "N342":
                case "N446":
                case "N447":
                case "N448":
                case "N449":
                case "N450":
                case "N451":
                case "N548":
                case "N549":
                case "N550":
                case "N559":
                    group = "Quality";
                    break;
                case "N016":
                    group = "PM";
                    break;
                case "N058":
                case "N131":
                case "N563":
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
