using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoBaseball.Web.Models
{
    public interface IPlayBaseball
    {
        String PlayerId { get; set; }
        String Name { get; set; }
    }
}