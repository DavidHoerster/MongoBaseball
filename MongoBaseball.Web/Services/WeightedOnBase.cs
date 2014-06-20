using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoBaseball.Entity;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoBaseball.Web.Services
{
    public class WeightedOnBase
    {
        private readonly MongoDatabase _db;
        private readonly MongoCollection<Batting> _coll;
        private readonly Int32 _year;

        public WeightedOnBase(MongoDatabase db, String collectionName, Int32 year)
        {
            _db = db;
            _coll = _db.GetCollection<Batting>(collectionName);
            _year = year;
        }

        public String CollectionName { get { return String.Format("WOBA{0}", _year.ToString()); } }

        public Boolean CreateWobaCollection()
        {
            var wobaColl = _db.GetCollection<WOBA>("WOBALookup");
            var woba = wobaColl.AsQueryable().First(w => w.Id == _year);

            var pipeline = new List<BsonDocument>();
            pipeline.Add(GetMatchPipeline(woba.Id));
            pipeline.Add(GetRedactPipeline(502));
            pipeline.Add(GetProjectPipeline(woba));
            pipeline.Add(GetSortPipeline());
            pipeline.Add(GetOutPipeline());

            var aggArgs = new AggregateArgs
            {
                Pipeline = pipeline
            };
            var results = _coll.Aggregate(aggArgs);

            return true;
        }

        public Boolean DropWobaCollection()
        {
            var result = _db.DropCollection(CollectionName);
            return result.Ok;
        }

        private BsonDocument GetMatchPipeline(Int32 year)
        {
            return new BsonDocument { { "$match", new BsonDocument { { "Year", year } } } };
        }

        private BsonDocument GetRedactPipeline(Int32 minAtBats)
        {
            return new BsonDocument{
                {"$redact", new BsonDocument{
                    {"$cond", new BsonDocument{
                        {"if", new BsonDocument{
                            {"$gte", new BsonArray{"$AtBats", minAtBats}}
                        }},
                        {"then", "$$KEEP"},
                        {"else", "$$PRUNE"}
                    }}
                }}
            };
        }

        private BsonDocument GetProjectPipeline(WOBA woba)
        {
            var numerator = new BsonDocument{
                {"$add", new BsonArray {
                    CreateMathBsonOp("$multiply", woba.wBB, "$BaseOnBalls"),
                    CreateMathBsonOp("$multiply", woba.wHBP, "$HitByPitch"),
                    CreateMathBsonOp("$multiply", woba.w1B, "$Hits"),
                    CreateMathBsonOp("$multiply", woba.w2B, "$Doubles"),
                    CreateMathBsonOp("$multiply", woba.w3B, "$Triples"),
                    CreateMathBsonOp("$multiply", woba.wHR, "$HomeRuns"),
                    CreateMathBsonOp("$multiply", woba.runSB, "$StolenBases"),
                    CreateMathBsonOp("$multiply", woba.runCS, "$CaughtStealing")
                }}
            };

            var denominator = new BsonDocument{
                {"$add", new BsonArray{"$AtBats","$BaseOnBalls","$HitByPitch","$SacrificeFlies",CreateMathBsonOp("$multiply", -1,"$IntentionalWalks")}}
            };

            return new BsonDocument
            {
                {"$project", new BsonDocument{
                    {"Id", "$_id"},
                    {"PlayerId", 1},
                    {"Year", 1},
                    {"TeamId", 1},
                    {"AtBats", 1}, 
                    {"BaseOnBalls", 1},
                    {"HitByPitch", 1},
                    {"SacrificeFlies", 1},
                    {"SacrificeHits", 1},
                    {"WOBA", new BsonDocument{
                        {"$divide", new BsonArray{numerator, denominator}}
                    }}
                }}
            };
        }

        private BsonDocument CreateMathBsonOp(String op, Double lhs, String rhs)
        {
            return new BsonDocument{
                {op, new BsonArray { lhs, rhs}}
            };
        }

        private BsonDocument GetLimitPipeline(Int32 limit)
        {
            return new BsonDocument { { "$limit", limit } };
        }

        private BsonDocument GetSortPipeline()
        {
            return new BsonDocument { { "$sort", new BsonDocument { { "WOBA", -1 } } } };
        }

        private BsonDocument GetOutPipeline()
        {
            return new BsonDocument { { "$out", CollectionName } };
        }
    }
}