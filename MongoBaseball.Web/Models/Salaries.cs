using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoBaseball.Web.Models
{
    public class Salaries
    {
        public String _id { get; set; }
        public Int32 yearID { get; set; }
        public String teamID { get; set; }
        public String lgID { get; set; }
        public String playerID { get; set; }
        public Int32 salary { get; set; }
    }
}