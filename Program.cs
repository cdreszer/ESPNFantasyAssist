using System;
using System.Collections.Generic;

namespace ESPNFantasyAssist
{
    class Program
    {
        static void Main(string[] args)
        {
            FantasyTeam team = HtmlParser.BuildFantasyTeam();
            Console.WriteLine("ACTIVE:");
            team.PrintStats(true);
            Console.WriteLine("\nINACTIVE:");
            team.PrintStats(false);
            Console.WriteLine("\nBY POSITION:");
            team.printStatsByPosition();
        }
    }
}
