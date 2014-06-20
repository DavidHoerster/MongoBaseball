using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoBaseball.Web.Models
{
    public class SimpleWRAA : IPlayBaseball
    {
        public String Id { get; set; }
        public Int32 Year { get; set; }
        public String TeamId { get; set; }
        public String PlayerId { get; set; }
        public String Name { get; set; }
        public Double WRAA { get; set; }
        public Double WOBA { get; set; }
    }
}