using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoBaseball.Entity
{
    public class Location
    {
        public String Country { get; set; }
        public String State { get; set; }
        public String City { get; set; }
    }

    public class Name
    {
        public String First { get; set; }
        public String Last { get; set; }
        public String Given { get; set; }
    }
    public class Master
    {
        public String Id { get; set; }
        public DateTime? Born { get; set; }
        public DateTime? Died { get; set; }
        public Location BirthPlace { get; set; }
        public Location DeathPlace { get; set; }
        public Int32? Weight { get; set; }
        public Int32? Height { get; set; }
        public String Bats { get; set; }
        public String Throws { get; set; }
        public DateTime? Debut { get; set; }
        public DateTime? FinalGame { get; set; }
        public Name Name { get; set; }
        public String RetroId { get; set; }
        public String BbRefId { get; set; }
    }
}
