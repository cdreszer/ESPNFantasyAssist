using System;
using System.Collections.Generic;

namespace ESPNFantasyAssist {
    public static class Util {
        public static string[] BatterCategories = {"H", "AB", "R", "HR", "RBI", "SB"};
        public static string[] PitcherCategories = {"IP", "H", "ER", "BB", "K", "W", "SV"};
        public static string[] FinalBatterCategories = {"H", "AB", "R", "HR", "RBI", "SB", "AVG"};
        public static string[] FinalPitcherCategories = {"IP", "H", "ER", "BB", "K", "W", "SV", "ERA", "WHIP"};
        public static int NumActiveBatters = 13;
        public static int NumActivePitchers = 9;

        /// Returns dictionary with 0 for each batting category.
        public static Dictionary<string, double> GetDefaultBatterStats() {
            return new Dictionary<string, double>() {
                    {"H", 0},
                    {"AB", 0},
                    {"R", 0},
                    {"HR", 0},
                    {"RBI", 0},
                    {"SB", 0}};
        }

        /// Returns dictionary with 0 for each pitching category.
        public static Dictionary<string, double> GetDefaultPitcherStats() {
            return new Dictionary<string, double>() {
                    {"IP", 0},
                    {"H", 0},
                    {"ER", 0},
                    {"BB", 0},
                    {"K", 0},
                    {"W", 0},
                    {"SV", 0}};
        }

        public static Dictionary<int, string> BatterPositionByIndex() {
            return new Dictionary<int, string>() {
                    {0, "C"},
                    {1, "1B"},
                    {2, "2B"},
                    {3, "3B"},
                    {4, "SS"},
                    {5, "2B/SS"},
                    {6, "1B/3B"},
                    {7, "OF1"},
                    {8, "OF2"},
                    {9, "OF3"},
                    {10, "OF4"},
                    {11, "OF5"},
                    {12, "UTIL"}};
        }

        /// Batting average rounded to 3 decimal places.
        public static double CalculateAverage(double hits, double atBats) {
            if (atBats == 0) {
                return 0;
            }
            return Math.Round(hits / atBats, 3);
        }

        /// WHIP rounded to two decimal points.
        public static double CalculateWHIP(double walks, double hits, double inningsPitched) {
            if (inningsPitched == 0)
                return 0;

            return Math.Round((walks + hits) / inningsPitched, 2);
        }

        /// ERA rounded to two decimal points.
        public static double CalculateERA(double earnedRuns, double inningsPitched) {
            if (inningsPitched == 0)
                return 0;
                
            return Math.Round((earnedRuns * 9.0) / inningsPitched, 2);
        }

        /// Rough calculation of games started pace.
        public static double GamesStartedPace(int gamesStarted, int numStarters, int gamesLeft) {
            return gamesStarted + numStarters * (gamesLeft / 5);
        }

        /// ESPN uses 7.1 IP, 7.2 IP for 7 1/3 IP and 7 2/3 IP  
        public static double ModifyInningsPitched(double inningsPitched) {
            // Uses .001 delta for double inprecision
            if (Math.Abs(inningsPitched % 1 - .1) <= .001) {
                inningsPitched = inningsPitched + .23;
            }
            else if (Math.Abs(inningsPitched % 1 - .2) <= .001) {
                inningsPitched = inningsPitched + .47;
            }

            return inningsPitched;
        }
    }
}