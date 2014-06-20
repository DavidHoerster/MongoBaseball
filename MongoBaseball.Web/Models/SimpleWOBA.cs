using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoBaseball.Web.Models
{
    public class SimpleWOBA : IPlayBaseball
    {
        public Int32 Year { get; set; }
        public String TeamId { get; set; }
        public String PlayerId { get; set; }
        public String Name { get; set; }
        public Double WOBA { get; set; }
        public Int32 AtBats { get; set; }
        public Int32 BaseOnBalls { get; set; }
        public Int32 HitByPitch { get; set; }
        public Int32 SacrificeHits { get; set; }
        public Int32 SacrificeFlies { get; set; }
        public String Id { get; set; }
    }
}