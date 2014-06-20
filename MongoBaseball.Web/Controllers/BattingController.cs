using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoBaseball.Entity;
using MongoBaseball.Web.Models;
using MongoBaseball.Web.Services;
using MongoDB.Driver;

namespace MongoBaseball.Web.Controllers
{
    public class BattingController : Controller
    {
        private readonly MongoClient _client;
        private readonly MongoServer _svr;
        private readonly MongoDatabase _db;

        public BattingController()
        {
            _client = new MongoClient("mongodb://localhost:27020");
            _svr = _client.GetServer();
            _db = _svr.GetDatabase("Baseball2013");
        }

        [HttpGet]
        public ActionResult Main()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Average()
        {
            var model = new List<SimpleBatter>();
            return View(model);
        }

        [HttpPost]
        public ActionResult Average(Int32? id)
        {
            var collName = id.HasValue ? "batting" : "battingLifetime";
            var minAtBats = id.HasValue ? 502 : 3000;
            var year = id.HasValue ? id.Value : 0;

            var avg = new Average(_db, collName);
            var results = avg.GetAverage(502, 25, year);

            var nameResolver = new NameResolver<SimpleBatter>(_db);
            nameResolver.ResolveNames(results);

            return View(results);
        }


        [HttpGet]
        public ActionResult OnBase()
        {
            var model = new List<SimpleObp>();
            return View(model);
        }

        [HttpPost]
        public ActionResult OnBase(Int32? id)
        {
            var collName = id.HasValue ? "batting" : "battingLifetime";
            var minAtBats = id.HasValue ? 502 : 3000;
            var year = id.HasValue ? id.Value : 0;

            var avg = new OnBasePercentage(_db, collName);
            var results = avg.GetOBP(502, 25, year);

            var nameResolver = new NameResolver<SimpleObp>(_db);
            nameResolver.ResolveNames(results);
            
            return View(results);
        }


        [HttpGet]
        public ActionResult wRAA()
        {
            var model = new List<SimpleWRAA>();
            return View(model);
        }

        [HttpPost]
        public ActionResult wRAA(Int32? id)
        {
            var wraa = new RunsAboveAverage(_db, "batting", id.Value);
            var results = wraa.GetRunsAboveAverage();

            var nameResolver = new NameResolver<SimpleWRAA>(_db);
            nameResolver.ResolveNames(results);

            return View(results);
        }

        [HttpGet]
        public ActionResult TotalBases()
        {
            var model = new List<SimpleTotalBases>();
            return View(model);
        }

        [HttpPost]
        public ActionResult TotalBases(Int32? id)
        {
            var tb = new TotalBases(_db, "batting");
            var results = tb.GetTotalBases(id.Value, 0);

            var nameResolver = new NameResolver<SimpleTotalBases>(_db);
            nameResolver.ResolveNames(results);

            return View(results);
        }


        [HttpGet]
        public ActionResult CostPerBase()
        {
            var model = new List<SimpleCostPerBase>();
            return View(model);
        }

        [HttpPost]
        public ActionResult CostPerBase(Int32? id)
        {
            var sal = new Salary(_db, "salaries", id.Value);
            var results = sal.GetCostPerBase(25);

            var nameResolver = new NameResolver<SimpleCostPerBase>(_db);
            nameResolver.ResolveNames(results);

            return View(results);
        }



        [HttpGet]
        public ActionResult RunsCreated()
        {
            var model = new List<SimpleRunsCreated>();
            return View(model);
        }

        [HttpPost]
        public ActionResult RunsCreated(Int32? id)
        {
            var rc = new RunsCreated(_db, id.HasValue ? "batting" : "battingLifetime");
            var results = rc.GetPlayerRunsCreated(id.Value);

            var nameResolver = new NameResolver<SimpleRunsCreated>(_db);
            nameResolver.ResolveNames(results);

            return View(results);
        }

        [HttpGet]
        public ActionResult TeamRunsCreated()
        {
            var model = new List<SimpleRunsCreatedTeam>();
            return View(model);
        }

        [HttpPost]
        public ActionResult TeamRunsCreated(Int32? id)
        {
            var rc = new RunsCreatedTeam(_db, id.HasValue ? "batting" : "battingLifetime");
            var results = rc.GetTeamRunsCreated(id.Value);

            return View(results);
        }

    }
}