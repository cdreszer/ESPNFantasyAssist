using System;
using System.Collections.Generic;

namespace ESPNFantasyAssist {
    public class PlayerInfo : IEquatable<PlayerInfo> {
        private string name;
        private string team;
        private string position;
        private string handedness;

        public string Name { get => name; set => name = value; }
        public string Team { get => team; set => team = value; }
        public string Position { get => position; set => position = value; }
        public string Handedness { get => handedness; set => handedness = value; }

        /// overrides equitables equal method
        public bool Equals(PlayerInfo other) {
            if (other == null) {
                return false;
            }

            return this.Name.Equals(other.Name) && this.Position.Equals(other.Position) && this.Team.Equals(other.Team);
        }
    }
}