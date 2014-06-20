using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoBaseball.Web.Models;

namespace MongoBaseball
{
    public class SimpleObp : IPlayBaseball
    {
        public String Id { get; set; }
        public String PlayerId { get; set; }
        public String Name { get; set; }
        public Int32 Year { get; set; }
        public String TeamId { get; set; }
        public Double OBP { get; set; }
    }
}
