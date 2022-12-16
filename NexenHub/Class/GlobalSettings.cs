using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Class
{
    public static class GlobalSettings
    {
        public static bool Test;
        public static string DatabaseConnection;
        public static string DatabaseConnectionDevelopment;
        public static string DatabaseControlling;
        public static string DatabaseAktion;

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

    }
}
