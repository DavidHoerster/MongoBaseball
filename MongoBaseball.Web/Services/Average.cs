﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoBaseball.Entity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoBaseball.Web.Services
{
    public class Average
    {
        private readonly MongoDatabase _db;
        private readonly MongoCollection<Batting> _coll;
        public Average(MongoDatabase db, String collectionName)
        {
            _db = db;
            _coll = _db.GetCollection<Batting>(collectionName);
        }

        public IList<SimpleBatter> GetAverage(Int32 minAtBats, Int32 limit, Int32 year = 0)
        {
            var pipeline = new List<BsonDocument>();
            pipeline.Add(CreateMatchPipeline(minAtBats, year));

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

            var theResults = results.Select(r => new SimpleBatter
            {
                Id = r["_id"].AsString,
                PlayerId = r.Contains("PlayerId") ? r["PlayerId"].AsString : r["_id"].AsString,
                AVG = r["AVG"].AsDouble,
                TeamId = r.Contains("TeamId") ? r["TeamId"].AsString : "FOO",
                Year = r.Contains("Year") ? r["Year"].AsInt32 : 0
            }).ToList();
            return theResults;

        }


        private BsonDocument CreateMatchPipeline(Int32 minAtBats, Int32 year)
        {
            var matchBson = new BsonDocument();
            if (year > 0)
            {
                matchBson.Add("Year", year);
            }
            matchBson.Add("AtBats", new BsonDocument{
                    {"$gte", minAtBats}
                });

            var match = new BsonDocument{
                {"$match", matchBson
                }
            };

            return match;
        }

        private BsonDocument CreateProjectPipeline()
        {
            var numerator = new BsonDocument{
                {"$add", new BsonArray{"$Hits"}}
            };

            var denominator = new BsonDocument{
                {"$add", new BsonArray{"$AtBats"}}
            };

            var project = new BsonDocument
            {
                {"$project", new BsonDocument{
                    {"PlayerId", 1},
                    {"Year", 1},
                    {"TeamId", 1},
                    {"AVG", new BsonDocument{
                        {"$cond", new BsonDocument{
                            {"if", new BsonDocument{
                                {"$eq", new BsonArray{"$AtBats", "0"}}
                            }},
                            {"then", 0},
                            {"else", new BsonDocument{
                                {"$divide", new BsonArray{numerator, denominator}}
                            }}
                        }}
                    }}
                }}
            };

            return project;
        }

        private BsonDocument CreateSortPipeline()
        {
            var sort = new BsonDocument{
                {"$sort", new BsonDocument{
                    {"AVG", -1}
                }
                }
            };
            return sort;
        }

        private BsonDocument CreateLimitPipeline(Int32 limit)
        {
            var limitDoc = new BsonDocument{
                {"$limit", limit}
            };
            return limitDoc;
        }


    }
}