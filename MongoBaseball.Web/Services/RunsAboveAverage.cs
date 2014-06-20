using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoBaseball.Entity;
using MongoBaseball.Web.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoBaseball.Web.Services
{
    public class RunsAboveAverage
    {
        private readonly MongoDatabase _db;
        private readonly MongoCollection<Batting> _coll;
        private readonly Int32 _year;
        private readonly WeightedOnBase _woba;
        public RunsAboveAverage(MongoDatabase db, String collectionName, Int32 year)
        {
            _db = db;
            _coll = _db.GetCollection<Batting>(collectionName);
            _year = year;
            _woba = new WeightedOnBase(_db, collectionName, _year);
        }

        public String CollectionName { get { return String.Format("WRAA{0}", _year.ToString()); } }

        public IList<SimpleWRAA> GetRunsAboveAverage(){
            var wobaColl = _db.GetCollection<WOBA>("WOBALookup");
            var woba = wobaColl.AsQueryable().First(w => w.Id == _year);

            _woba.CreateWobaCollection();

            var wobaYearColl = _db.GetCollection<SimpleWOBA>(_woba.CollectionName);
            var pipeline = new List<BsonDocument>();

            pipeline.Add(GetMatchPipeline());
            pipeline.Add(GetProjectPipeline(woba));
            pipeline.Add(GetSortPipeline());
            pipeline.Add(GetLimitPipeline(25));
            pipeline.Add(GetOutPipeline());

            var aggArgs = new AggregateArgs
            {
                Pipeline = pipeline
            };
            var results = wobaYearColl.Aggregate(aggArgs);

            var wraaColl = _db.GetCollection<SimpleWRAA>(CollectionName);
            var wraaResults = wraaColl.AsQueryable().ToList();

            _woba.DropWobaCollection();

            return wraaResults;
        }

        private BsonDocument GetMatchPipeline()
        {
            return new BsonDocument { { "$match", new BsonDocument { { "Year", _year } } } };
        }

        private BsonDocument GetProjectPipeline(WOBA woba)
        {
            var lhs = new BsonDocument{
                { "$divide", new BsonArray {
                    new BsonDocument{{"$subtract", new BsonArray{"$WOBA", woba.wOBA}}},
                    woba.wOBAScale
                }}};
            var rhs = new BsonDocument{
                {"$add", new BsonArray{"$AtBats","$BaseOnBalls","$HitByPitch","$SacrificeFlies","$SacrificeHits"}}
            };

            return new BsonDocument{
                {"$project", new BsonDocument{
                    {"PlayerId", 1},
                    {"Year", 1},
                    {"TeamId", 1},
                    {"WOBA", 1},
                    {"WRAA", new BsonDocument{
                        {"$multiply", new BsonArray{lhs, rhs}}
                    }}
                }}
            };
        }


        private BsonDocument GetLimitPipeline(Int32 limit)
        {
            return new BsonDocument { { "$limit", limit } };
        }

        private BsonDocument GetSortPipeline()
        {
            return new BsonDocument { { "$sort", new BsonDocument { { "WRAA", -1 } } } };
        }

        private BsonDocument GetOutPipeline()
        {
            return new BsonDocument { { "$out", CollectionName } };
        }
    }
}