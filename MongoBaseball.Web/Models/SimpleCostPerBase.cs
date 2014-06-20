using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoBaseball.Web.Models
{
    public class SimpleCostPerBase : IPlayBaseball
    {
        public String Id { get; set; }
        public String PlayerId { get; set; }
        public String Name { get; set; }
        public Int32 Year { get; set; }
        public String TeamId { get; set; }
        public Int32 TotalBases { get; set; }
        public Int32 Salary { get; set; }
        public Double CostPerBase { get {
            if (TotalBases==0)
            {
                return 0D;
            }
            if (Salary==0)
            {
                return 0D;
            }
            return ((Double)Salary / (Double)TotalBases); } }
    }
}