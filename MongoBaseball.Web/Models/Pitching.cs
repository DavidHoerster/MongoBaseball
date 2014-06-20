using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoBaseball.Entity
{
    public class Pitching
    {
        public String Id { get { return String.Format("{0}_{1}_{2}", PlayerId, Year, Stint); } }
        public String PlayerId { get; set; }
        public Int32 Year { get; set; }
        public Int32 Stint { get; set; }
        public String TeamId { get; set; }
        public String League { get; set; }
        public Int32 Wins { get; set; }
        public Int32 Losses { get; set; }
        public Int32 Games { get; set; }
        public Int32 GamesStarted { get; set; }
        public Int32 CompleteGames { get; set; }
        public Int32 Shutouts { get; set; }
        public Int32 Saves { get; set; }
        public Int32 OutsPitched { get; set; }
        public Int32 Hits { get; set; }
        public Int32 EarnedRuns { get; set; }
        public Int32 Homeruns { get; set; }
        public Int32 Walks { get; set; }
        public Int32 Strikeouts { get; set; }
        public Decimal OpponentBattingAverage { get; set; }
        public Decimal EarnedRunAverage { get; set; }
        public Int32 IntentionalWalks { get; set; }
        public Int32 WildPitches { get; set; }
        public Int32 BattersHitByPitch { get; set; }
        public Int32 Balks { get; set; }
        public Int32 BattersFaced { get; set; }
        public Int32 GamesFinished { get; set; }
        public Int32 RunsAllowed { get; set; }
        public Int32 SacrificeHits { get; set; }
        public Int32 SacrificeFlies { get; set; }
        public Int32 GroundedIntoDoublePlays { get; set; }
    }
}
