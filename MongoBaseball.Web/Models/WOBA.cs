using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoBaseball.Entity
{
    public class WOBA
    {
        public Int32 Id { get; set; }
        public Double wOBA { get; set; }
        public Double wOBAScale { get; set; }
        public Double wBB { get; set; }
        public Double wHBP { get; set; }
        public Double w1B { get; set; }
        public Double w2B { get; set; }
        public Double w3B { get; set; }
        public Double wHR { get; set; }
        public Double runSB { get; set; }
        public Double runCS { get; set; }
        public Double R_PA { get; set; }
        public Double R_W { get; set; }
        public Double cFIP { get; set; }
    }
}
