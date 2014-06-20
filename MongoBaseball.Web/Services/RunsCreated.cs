using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoBaseball.Entity;
using MongoBaseball.Web.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoBaseball.Web.Services
{
    public class RunsCreated
    {
        private readonly MongoDatabase _db;
        private readonly MongoCollection<Batting> _coll;
        public RunsCreated(MongoDatabase db, String collectionName)
        {
            _db = db;
            _coll = _db.GetCollection<Batting>(collectionName);
        }

        public IList<SimpleRunsCreated> GetPlayerRunsCreated(Int32 year, Int32 limit = 25)
        {
            var pipeline = new List<BsonDocument>();
            pipeline.Add(CreateMatchPipeline(year));

            pipeline.Add(CreateProjectPipeline());

            pipeline.Add(CreateSortPipeline());
            if (limit > 0)
            {
                pipeline.Add(CreateLimitPipeline(limit));
            }

            var aggArgs = new AggregateArgs
            {
                Pipeline = pipeline
            };
            var results = _coll.Aggregate(aggArgs);
            var theResults = results.Select(r => new SimpleRunsCreated
            {
                Id = r["_id"].AsString,
                PlayerId = r.Contains("PlayerId") ? r["PlayerId"].AsString : r["_id"].AsString,
                RunsCreated = r["RC"].AsDouble,
                TeamId = r.Contains("TeamId") ? r["TeamId"].AsString : "FOO",
                Year = r.Contains("Year") ? r["Year"].AsInt32 : 0
            }).ToList();
            return theResults;
        }

        private BsonDocument CreateMatchPipeline(Int32 year)
        {
            return new BsonDocument{
                {"$match", new BsonDocument{
                    {"Year", year},
                    {"AtBats", new BsonDocument{ {"$gte", 1}}}
                }}
            };
        }

        private BsonDocument CreateProjectPipeline()
        {
            var lhs = new BsonDocument{
                {"$add", new BsonArray{"$Hits","$BaseOnBalls"}}
            };
            var rhs = new BsonDocument{
                {"$add", new BsonArray{"$Hits","$Doubles","$Triples","$Triples","$HomeRuns","$HomeRuns","$HomeRuns"}}
            };

            var divisor = new BsonDocument{
                {"$add", new BsonArray{"$AtBats","$BaseOnBalls"}}
            };

            var mult = new BsonDocument{
                {"$multiply", new BsonArray{lhs, rhs}}
            };

            return new BsonDocument{
                {"$project", new BsonDocument{
                    {"PlayerId", 1},
                    {"Year", 1},
                    {"TeamId", 1},
                    {"RC", new BsonDocument{
                        {"$divide", new BsonArray{mult, divisor}}
                    }}
                }}
            };
        }

        private BsonDocument CreateSortPipeline()
        {
            return new BsonDocument{
                {"$sort", new BsonDocument{ {"RC", -1}}}
            };
        }

        private BsonDocument CreateLimitPipeline(Int32 limit)
        {
            return new BsonDocument { { "$limit", limit } };
        }
    }
}