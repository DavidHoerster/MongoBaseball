using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoBaseball.Entity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoBaseball.Web.Services
{
    public class TotalBases
    {
        private readonly MongoDatabase _db;
        private readonly MongoCollection<Batting> _coll;
        public TotalBases(MongoDatabase db, String collectionName)
        {
            _db = db;
            _coll = _db.GetCollection<Batting>(collectionName);
        }

        public Boolean CreateTotalBasesCollection(Int32 year, Int32 minAtBats = 0)
        {
            var pipeline = new List<BsonDocument>();
            pipeline.Add(CreateMatchPipeline(year, minAtBats));

            pipeline.Add(CreateProjectPipeline());

            pipeline.Add(CreateSortPipeline());
            pipeline.Add(GetOutPipeline(year));

            var aggArgs = new AggregateArgs
            {
                Pipeline = pipeline
            };
            var results = _coll.Aggregate(aggArgs);
            return true;
        }

        public IList<SimpleTotalBases> GetTotalBases(Int32 year, Int32 minAtBats)
        {
            var pipeline = new List<BsonDocument>();
            pipeline.Add(CreateMatchPipeline(year, minAtBats));
            pipeline.Add(CreateGroupPipeline());

            pipeline.Add(CreateProjectPipeline());

            pipeline.Add(CreateSortPipeline());

            var aggArgs = new AggregateArgs
            {
                Pipeline = pipeline
            };
            var results = _coll.Aggregate(aggArgs);

            var theResults = results.Select(r => new SimpleTotalBases
            {
                Id = r["_id"].AsString,
                PlayerId = r["_id"].AsString, //.AsBsonDocument.Contains("PlayerId") ? r["_id"]["PlayerId"].AsString : r["_id"]["Id"].AsString,
                TotalBases = r["TB"].AsInt32,
                TeamId = "FOO",
                Year = year //r["_id"].AsBsonDocument.Contains("Year") ? r["_id"]["Year"].AsInt32 : 0
            }).ToList();
            return theResults;

        }


        private BsonDocument CreateMatchPipeline(Int32 year, Int32 minAtBats)
        {
            return new BsonDocument()
            {
                {"$match", new BsonDocument{
                    {"Year", year},
                    {"AtBats", new BsonDocument{
                        {"$gte", minAtBats}
                    }}
                }}
            };
        }

        private BsonDocument CreateGroupPipeline()
        {
            return new BsonDocument{
                {"$group", new BsonDocument{
                    {"_id", "$PlayerId"},
                    {"Hits", new BsonDocument{ {"$sum", "$Hits"}}},
                    {"Doubles", new BsonDocument{ {"$sum", "$Doubles"}}},
                    {"Triples", new BsonDocument{ {"$sum", "$Triples"}}},
                    {"HomeRuns", new BsonDocument{ {"$sum", "$HomeRuns"}}},
                    {"BaseOnBalls", new BsonDocument{ {"$sum", "$BaseOnBalls"}}},
                    {"HitByPitch", new BsonDocument{ {"$sum", "$HitByPitch"}}},
                    {"StolenBases", new BsonDocument{ {"$sum", "$StolenBases"}}},
                    {"CaughtStealing", new BsonDocument{ {"$sum", "$CaughtStealing"}}}
                }}
            };
        }

        private BsonDocument CreateProjectPipeline()
        {
            return new BsonDocument{
                {"$project", new BsonDocument{
                    {"PlayerId", 1},
                    {"Year", 1},
                    {"TeamId", 1},
                    {"TB", new BsonDocument{
                        {"$add", new BsonArray{ "$BaseOnBalls", 
                                                "$HitByPitch", 
                                                "$StolenBases", 
                                                new BsonDocument{ { 
                                                    "$multiply", new BsonArray{"$CaughtStealing",-1}
                                                }}, 
                                                "$Hits", 
                                                "$Doubles", 
                                                "$Triples", 
                                                "$Triples", 
                                                "$HomeRuns", 
                                                "$HomeRuns", 
                                                "$HomeRuns"}}
                    }}
                }}
            };
        }

        private BsonDocument CreateSortPipeline()
        {
            return new BsonDocument{
                {"$sort", new BsonDocument{
                    {"TB", -1}
                }}
            };
        }

        private BsonDocument GetOutPipeline(Int32 year)
        {
            return new BsonDocument { { "$out", String.Format("TotalBases{0}",year.ToString()) } };
        }

    }
}