using System;
using System.Collections.Generic;

namespace ESPNFantasyAssist
{
    class Program
    {
        // static string date = DateTime.Today.ToString();

        public static void Main(string[] args)
        {
            string path = "./HTMLFiles/";

            if (args.Length > 0) {
                path = args[0];
            }

            // creates fantasy team.
            FantasyTeam team = HtmlParser.BuildFantasyTeam(path);
            // sort batters and pitchers.
            team.Batters = team.SortPlayersByCategory(team.Batters, "AB", true);
            team.Pitchers = team.SortPlayersByCategory(team.Pitchers, "IP", true);

            SpreadsheetGenerator.ExportToGoogleDocsSpreadsheet(path, team);
            Console.WriteLine("Succesfully updated https://docs.google.com/spreadsheets/d/1DB3CNvXUxtK8S7ZZwKHaQ177CV1XqFAOeTK5TdhJqUs/edit#gid=0");
            // PrintStatsToConsole(team);
        }

        /// Prints the active stats, inactive stats, and stats by position to the console.
        public static void PrintStatsToConsole(FantasyTeam team) {
            Console.WriteLine("ACTIVE STATS");
            team.PrintStats(true);
            Console.WriteLine("INACTIVE STATS");
            team.PrintStats(false);
            Console.WriteLine("STATS BY POSITION");
            team.printStatsByPosition();
        }
    }
}
