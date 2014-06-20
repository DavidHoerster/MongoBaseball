using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoBaseball.Entity
{
    public class Fielding
    {
        public String Id { get { return String.Format("{0}_{1}_{2}_{3}", PlayerId, Year, Stint, Position); } }
        public String PlayerId { get; set; }
        public Int32 Year { get; set; }
        public Int32 Stint { get; set; }
        public String Team { get; set; }
        public String League { get; set; }
        public String Position { get; set; }
        public Int32 Games { get; set; }
        public Int32 GamesStarted { get; set; }
        public Int32 InningOuts { get; set; }
        public Int32 Putouts { get; set; }
        public Int32 Assists { get; set; }
        public Int32 Errors { get; set; }
        public Int32 DoublePlays { get; set; }
        public Int32 PassedBalls { get; set; }
        public Int32 WildPitches { get; set; }
        public Int32 OpponentStolenBases { get; set; }
        public Int32 OpponentsCaughtStealing { get; set; }
        public Int32 ZoneRating { get; set; }
    }
}
