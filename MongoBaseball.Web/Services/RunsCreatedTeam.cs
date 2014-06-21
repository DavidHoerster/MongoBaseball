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
    public class RunsCreatedTeam
    {
        private readonly MongoDatabase _db;
        private readonly MongoCollection<Batting> _coll;
        public RunsCreatedTeam(MongoDatabase db, String collectionName)
        {
            _db = db;
            _coll = _db.GetCollection<Batting>(collectionName);
        }

        public IList<SimpleRunsCreatedTeam> GetTeamRunsCreated(Int32? year)
        {
            var pipeline = new List<BsonDocument>();
            if (year.HasValue)
            {
                pipeline.Add(CreateMatchPipeline(year.Value));
            }

            pipeline.Add(CreateGroupPipeline());

            pipeline.Add(CreateProjectPipeline());

            pipeline.Add(CreateSortPipeline());

            var aggArgs = new AggregateArgs
            {
                Pipeline = pipeline
            };
            var results = _coll.Aggregate(aggArgs);
            var theResults = results.Select(r => new SimpleRunsCreatedTeam
            {
                Id = r["_id"]["TeamId"].AsString,
                RunsCreated = r["RC"].AsDouble,
                TeamId = r["_id"]["TeamId"].AsString,
                Year = r["_id"]["Year"].AsInt32
            }).ToList();
            return theResults;

        }

        private BsonDocument CreateMatchPipeline(Int32 year)
        {
            return new BsonDocument{
                {"$match", new BsonDocument{
                    {"Year", year}
                }}
            };
        }

        private BsonDocument CreateGroupPipeline()
        {
            return new BsonDocument{
                {"$group", new BsonDocument{
                    {"_id", new BsonDocument{{"Year", "$Year"},{"TeamId", "$TeamId"}}},
                    {"Hits", new BsonDocument { {"$sum", "$Hits"}}},
                    {"Walks", new BsonDocument { {"$sum", "$BaseOnBalls"}}},
                    {"Doubles", new BsonDocument { {"$sum", "$Doubles"}}},
                    {"Triples",new BsonDocument { {"$sum", "$Triples"}}},
                    {"HR", new BsonDocument { {"$sum", "$HomeRuns"}}},
                    {"AtBats",new BsonDocument { {"$sum", "$AtBats"}}}
                }}
            };
        }

        private BsonDocument CreateProjectPipeline()
        {
            var lhs = new BsonDocument{
                {"$add", new BsonArray{"$Hits","$Walks"}}
            };
            var rhs = new BsonDocument{
                {"$add", new BsonArray{"$Hits","$Doubles","$Triples","$Triples","$HR","$HR","$HR"}}
            };

            var divisor = new BsonDocument{
                {"$add", new BsonArray{"$AtBats","$Walks"}}
            };

            var mult = new BsonDocument{
                {"$multiply", new BsonArray{lhs, rhs}}
            };

            return new BsonDocument{
                {"$project", new BsonDocument{
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

    }
}