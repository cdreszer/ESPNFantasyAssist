using HtmlAgilityPack;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ESPNFantasyAssist {
    public static class HtmlParser {

        
        static DateTime FirstDay = new DateTime(2019, 03, 20);
        static int DayOfSeason = 40; //Convert.ToInt32((DateTime.Today - FirstDay).TotalDays); // up to date day of season

        // modify to ./HTMLFiles/Dad/ for dads
        static string HtmlPath = "./HTMLFiles/Dad/";
        // modify to Chris Sale for dads 
        static string FirstPitcher = "Chris Sale";//"Jacob deGrom";

        /// CSS classes used to idetify fantasy baseball stat categories
        static String[] ScoringCat = {" stat-r ", " stat-hr ", " stat-rbi ", " stat-sb ", " stat-w ", " stat-k ", " stat-sv "};
        static String[] BatterCatClasses = {" stat-h/ab ", " stat-r ", " stat-hr ", " stat-rbi ", " stat-sb "};
        static String[] PitcherCatClasses = {" stat-ip ", " stat-er ", " stat-h ", " stat-bb ", " stat-w ", " stat-k ", " stat-sv "};
        static String[] AllCat = {" stat-h/ab ", " stat-r ", " stat-hr ", " stat-rbi ", " stat-sb ", " stat-ip ", " stat-er ", " stat-h ", " stat-bb ", " stat-w ", " stat-k ", " stat-sv "};
        
        /// Maps stat category CSS class to stat category in player's active stats dictionary
        static Dictionary<string, string> CategoryByHTMLClass = new Dictionary<string, string> {
                {" stat-r ", "R"},
                {" stat-hr ", "HR"},
                {" stat-rbi ", "RBI"},
                {" stat-sb ", "SB"},
                {" stat-ip ", "IP"},
                {" stat-er ", "ER"},
                {" stat-h ", "H"},
                {" stat-bb ", "BB"},
                {" stat-w ", "W"},
                {" stat-k ", "K"},
                {" stat-sv ", "SV"}
            };

        public static FantasyTeam BuildFantasyTeam() {
            // Store players in dictionary
            Dictionary<string, Player> PlayersById = new Dictionary<string, Player>();
            Dictionary <string, Player> StatsByPosition = new Dictionary<string, Player>();
            return BuildFantasyTeam(PlayersById, StatsByPosition, 1, DayOfSeason);
        }

        /// Updates fantasy team provided starting at the last day updated until current day of the season.
        public static FantasyTeam UpdateTeam(FantasyTeam team) {
            Dictionary<string, Player> PlayersById = new Dictionary<string, Player>();

            foreach (Player player in team.GetAllPlayers()) {
                PlayersById.Add(player.Id, player);
            }

            return BuildFantasyTeam(PlayersById, team.StatsByPosition, team.LastDayUpdated, DayOfSeason);
        }

        /// Given a dictionary of player ids to players builds a fantasy team starting from start date to end date.
        /// PlayersById can be an empty Dictionary or one to be updated.
        public static FantasyTeam BuildFantasyTeam(Dictionary<string, Player> PlayersById, Dictionary <string, Player> StatsByPosition, int startDate, int endDate) {
            // for range of days combines players active stats
            for (int day = startDate; day <= endDate; day++) {
                Player[] array = GetPlayersFromDay(day);

                int index = 0;
                // Update active stats for each player in that day
                foreach(Player player in array) {
                    if (player != null) {
                        StatsByPosition = UpdateStatsByPosition(StatsByPosition, index++, player);
                        PlayersById = UpdatePlayersById(PlayersById, player);
                    }
                }
            }

            // Creates fantasy team and sets last day updated
            FantasyTeam team = new FantasyTeam("", PlayersById.Values.ToList()); 
            team.StatsByPosition = StatsByPosition;
            team.LastDayUpdated = DayOfSeason;
            return team;
        }

        /// Updates player id -> player stats map.
        public static Dictionary<string, Player> UpdatePlayersById(Dictionary<string, Player> PlayersById, Player player) {
            // Fill PlayersById dictionary
            // IF player is already in dictionary, update active/inactive stats.
            if (PlayersById.TryGetValue(player.Id, out Player value)) {
                player.UpdateActiveTotals(value.ActiveTotals);
                player.UpdateInactiveTotals(value.InactiveTotals);
                PlayersById[player.Id] = player;
            }
            // ELSE add to dictionary
            else {
                PlayersById.Add(player.Id, player);
            }

            return PlayersById;
        }

        /// Updates posiition -> totals stats at position map...  C -> stats, 1B -> stats, 2B -> stats map.
        public static Dictionary<string, Player> UpdateStatsByPosition(Dictionary<string, Player> StatsByPosition, int index, Player player) {
            // Fill Stats By Position dictionary
            if (index < Util.NumActiveBatters) {
                string pos = Util.BatterPositionByIndex()[index];
                Player posPlayer = new Player(pos, "", "", true);
                posPlayer.UpdateActiveTotals(player.ActiveTotals);

                // IF position is already in dictionary, update stat values
                if (StatsByPosition.TryGetValue(pos, out Player value)) {
                    posPlayer.UpdateActiveTotals(value.ActiveTotals);
                    StatsByPosition[pos] = posPlayer;
                }
                // ELSE add stat values into dictionary
                else {
                    StatsByPosition.Add(pos, posPlayer);
                }
            }

            return StatsByPosition;
        }

        /// Gets an array of players with updated daily (active/inactive) stats from a certain day
        public static Player[] GetPlayersFromDay(int day) {
            HtmlDocument doc = LoadDocument(day);
            
            // searches for nodes with class 'player__column' that are not headers
            var playerColumn = doc.DocumentNode.SelectNodes("//*[contains(@class, 'player__column') and not(contains(@class, 'header'))]");

            Player[] players = new Player[playerColumn.Count];

            int index = 0;
            int pitcherIndex = 0;
            
            // Creates a player for each player name found in document and adds to players array.
            foreach (var player in playerColumn) {
                string name = player.Attributes["title"].Value;
                // Checks when pitchers start (should be better way)
                if (name == FirstPitcher)
                    pitcherIndex = index;

                Player temp = new Player(name, "", "", pitcherIndex == 0);
                players[index++] = temp;
            }

            index = 0;
            int playerIndex = 0;

            // iterates through each category updating each player
            foreach (string catClass in AllCat) {
                var category = doc.DocumentNode.SelectNodes(String.Concat("//*[contains(@class, '", catClass, "')]"));
                
                // if pitcher update index
                if (Array.IndexOf(PitcherCatClasses, catClass) >= 0) {
                    playerIndex = pitcherIndex;
                }

                if (category != null) {
                    foreach (var playerStat in category) {
                        // skip headers: scoring cat have 2 extra, non scoring cat (h/ab, ip, er, h, bb) have one extra
                        if (index == 0 || (index == 1 && Array.IndexOf(ScoringCat, catClass) >= 0)) {
                            index++;
                        } 
                        // Update player with new stats.
                        else {
                            Player player = UpdatePlayer(((Player)players.GetValue(playerIndex)), catClass, playerStat.InnerText.ToString(), playerIndex, pitcherIndex);
                            players[playerIndex++] = player;
                            index++;
                        }
                    }
                }

                index = 0;
                playerIndex = 0;
            }

            return players;
        }

        /// Updates the given player in specified category (" stat-h/ab ") with string value ("0/4")
        ///     - uses player index and pitcher index to decide whether active or inactive
        /// returns: updated player
        public static Player UpdatePlayer(Player player, string catClass, string value, int playerIndex, int pitcherIndex) {
            Dictionary<string, double> dailyTotal = new Dictionary<string, double>();

            // Seperates H/AB category into H and AB
            if (catClass.Equals(" stat-h/ab ")) {
                dailyTotal = SeperateHitAndAbs(value);
            }
            else {
                // checks if -- (did not play)
                if (value == "--") {
                    value = "0";
                }

                dailyTotal.Add(CategoryByHTMLClass[catClass], double.Parse(value));
            }

            // adjusts IP: 7.1 -> 7.33 , 7.2 -> 7.67
            if (dailyTotal.ContainsKey("IP")) {
                dailyTotal["IP"] = Util.ModifyInningsPitched(dailyTotal["IP"]);
            }

            // Updates active or inactive totals.
            if (!IsPlayerActive(playerIndex, pitcherIndex)) {
                player.UpdateInactiveTotals(dailyTotal);
            }
            else {
                player.UpdateActiveTotals(dailyTotal);
            }

            return player;
        }

        // Seperates Hits and Abs from 1/4 and returns them in a dictionary.
        public static Dictionary<string, double> SeperateHitAndAbs(string value) {
            Dictionary<string, double> dailyTotal = new Dictionary<string, double>();
            double hits = 0;
            double ab = 0;

            if (value != "--/--") {
                int slashIndex = value.IndexOf("/");
                hits = double.Parse(value.Substring(0, slashIndex));
                ab = double.Parse(value.Substring(slashIndex + 1));
            }

            dailyTotal.Add("H", hits);
            dailyTotal.Add("AB", ab);

            return dailyTotal;
        }

        // Checks to see if the player is active.
        public static Boolean IsPlayerActive(int playerIndex, int pitcherIndex) {
            return !((playerIndex < pitcherIndex && playerIndex >= Util.NumActiveBatters) 
                    || (playerIndex >= pitcherIndex + Util.NumActivePitchers));
        }

        public static HtmlDocument LoadDocument(int day) {
            HtmlDocument doc = new HtmlDocument();
            string path = HtmlPath + "day" + day + "HTML.html";
            doc.Load(path);

            return doc;
        }
    }
}