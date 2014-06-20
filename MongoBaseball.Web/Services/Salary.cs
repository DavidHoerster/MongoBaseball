using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoBaseball.Web.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoBaseball.Web.Services    
{
    public class Salary
    {
        private readonly MongoDatabase _db;
        private readonly MongoCollection<Salaries> _coll;
        private readonly Int32 _year;
        private readonly TotalBases _tb;

        public Salary(MongoDatabase db, String collectionName, Int32 year)
        {
            _db = db;
            _coll = _db.GetCollection<Salaries>(collectionName);
            _year = year;
            _tb = new TotalBases(_db, "batting");
        }

        public IList<SimpleCostPerBase> GetCostPerBase(Int32 limit)
        {
            var totalBases = _tb.GetTotalBases(_year, 300);
            var salaries = _coll.AsQueryable().Where(s => s.yearID == _year).ToList();

            var scpb = totalBases.Select(tb => new SimpleCostPerBase
            {
                Id = tb.Id,
                PlayerId = tb.PlayerId,
                TeamId = tb.TeamId,
                TotalBases = tb.TotalBases,
                Year = tb.Year,
                Salary = (salaries.Any(s => s.playerID == tb.PlayerId)) ? (salaries.FirstOrDefault(s => s.playerID == tb.PlayerId).salary) : 0
            }).OrderByDescending(s => s.CostPerBase)
            .Take(limit);

            return scpb.ToList();
        }
    }
}