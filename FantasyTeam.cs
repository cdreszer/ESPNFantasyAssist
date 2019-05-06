using System;
using System.Linq;
using System.Collections.Generic;

namespace ESPNFantasyAssist {
    public class FantasyTeam {
        private string teamName;
        private int gamesStarted;
        private List<Player> batters;
        private List<Player> pitchers;
        private int lastDayUpdated;
        private Dictionary<string, Player> statsByPosition;

        // auto-generated getters and setters
        public string TeamName { get => teamName; set => teamName = value; }
        public List<Player> Batters { get => batters; set => batters = value; }
        public List<Player> Pitchers { get => pitchers; set => pitchers = value; }
        public int GamesStarted { get => gamesStarted; set => gamesStarted = value; }
        public int LastDayUpdated { get => lastDayUpdated; set => lastDayUpdated = value; }
        public Dictionary<string, Player> StatsByPosition { get => statsByPosition; set => statsByPosition = value; }

        public FantasyTeam(string name, List<Player> batters, List<Player> pitchers) {
            TeamName = name;
            Batters = batters;
            Pitchers = pitchers;

            statsByPosition = new Dictionary<string, Player>();
        }

        public FantasyTeam(string name, List<Player> players) {
            TeamName = name;
            batters = new List<Player>();
            pitchers = new List<Player>();

            // sorts players into pitchers and batters and discards empty 'Player'
            foreach(Player player in players) {
                if (player.Name != "Player") {
                    if (player.IsBatter) {
                        batters.Add(player);
                    }
                    else {
                        pitchers.Add(player);
                    }
                }
            }

            statsByPosition = new Dictionary<string, Player>();
        }

        /// given a batter and daily totals, updates the teams batter
        // public void UpdateBatter(Player batter, Dictionary<string, double> dailyTotal) {
        //     int index = Batters.IndexOf(batter);
        //     if (index > 0) {
        //         Batters[index].UpdateActiveTotals(dailyTotal);
        //     }
        //     else {
        //         batter.UpdateActiveTotals(dailyTotal);
        //         Batters.Add(batter);
        //     }
        // }

        /// Sorts player by specified stat category.. need to pass in Batters/Pitchers and whether or not desc
        /// NOTE: Currently only sorting active stats.
        public List<Player> SortPlayersByCategory(List<Player> players, string sortOrder, bool descending) {
            // No null checking: assumes player list has valid sort order string... ex: ("HR")
            switch (sortOrder)
            {
                case "Name":
                    if (descending) {
                        players = players.OrderByDescending(p => p.Name).ToList();
                    }
                    else  {
                        players = players.OrderBy(p => p.Name).ToList();
                    }
                    break;
                default:
                    if (descending) {
                        players = players.OrderByDescending(p => p.ActiveTotals[sortOrder]).ToList();
                    }
                    else {
                        players = players.OrderBy(p => p.ActiveTotals[sortOrder]).ToList();
                    }
                    break;
            }

            return players;
        }

        /// Returns batters and pitchers in single list.
        public List<Player> GetAllPlayers() {
            List<Player> allPlayers = new List<Player>();
            allPlayers.AddRange(Batters);
            allPlayers.AddRange(Pitchers);
            return allPlayers;
        }

        public void printStatsByPosition() {
            PrintStats(StatsByPosition.Values.ToList(), true, true);
        }

        /// Prints batter and pitcher active or inactive stats.
        public void PrintStats(bool activeStats) {
            // sorts by total hits
            Batters = SortPlayersByCategory(Batters, "AB", true);
            Console.WriteLine();
            PrintStats(Batters, true, activeStats);

            // sorts by IP
            Pitchers = SortPlayersByCategory(Pitchers, "IP", true);
            Console.WriteLine();
            PrintStats(Pitchers, false, activeStats);
        }

        /// Given a list of batters or pitchers, prints out active stats.
        public void PrintStats(List<Player> players, bool areBatters, bool activeStats) {
            if (players == null) {
                Console.WriteLine("No Players....");
                return;
            }

            string[] categories = areBatters ? Util.FinalBatterCategories : Util.FinalPitcherCategories;

            Console.WriteLine("\t\t\t" + String.Join("\t", categories));
            foreach(Player player in players) {
                if (activeStats) {
                    player.PrintActiveStats();
                }
                else {
                    player.PrintInactiveStats();
                }
            }
        }

        /// Gets all the stats from the passed in players as a list of players as a list of stats
        public List<IList<object>> GetStatsAsListOfLists(List<Player> players, bool activeStats, bool areBatters) {
            List<IList<object>> playerStats = new List<IList<object>>();

            // Build header
            string[] statCategories = areBatters ? Util.FinalBatterCategories : Util.FinalPitcherCategories;
            List<object> categories = new List<object>();
            categories.Add("Name");
            categories.AddRange(statCategories);
            playerStats.Add(categories);

            foreach (Player player in players) {
                if (player.HasStats(activeStats)) {
                    playerStats.Add(player.GetStatsAsList(activeStats));
                }
            }

            return playerStats;
        }

        public List<IList<object>> GetStatsByPositionAsListOfLists() {
            return GetStatsAsListOfLists(StatsByPosition.Values.ToList(), true, true);
        }

        /// Returns a totals column in list format spreadsheet data format)
        public List<IList<object>> GetTotalAsListOfLists(List<Player> players, bool activeStats, bool areBatters) {
            List<Player> totals = new List<Player>();
            Player total = new Player("TOTALS ", "", "", areBatters);

            foreach (Player player in players) {
                if (activeStats) {
                    total.UpdateActiveTotals(player.ActiveTotals);
                }
                else {
                    total.UpdateInactiveTotals(player.InactiveTotals);
                }
            }

            totals.Add(total);

            List<IList<object>> totalsAsListOfLists = GetStatsAsListOfLists(totals, activeStats, areBatters);

            // removes header
            if (totalsAsListOfLists.Count > 1) {
                totalsAsListOfLists = totalsAsListOfLists.GetRange(1, 1);
            }
            else {
                totalsAsListOfLists = new List<IList<object>>();
            }

            // returns just stats with no header
            return totalsAsListOfLists;
        }
    }
}