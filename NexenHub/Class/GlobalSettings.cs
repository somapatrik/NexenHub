﻿using System;
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
    }
}
