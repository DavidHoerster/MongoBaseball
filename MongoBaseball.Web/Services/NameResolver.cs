using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoBaseball.Entity;
using MongoBaseball.Web.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoBaseball.Web.Services
{
    public class NameResolver<T> where T : IPlayBaseball
    {
        private readonly MongoDatabase _db;
        private readonly MongoCollection<Master> _coll;
        public NameResolver(MongoDatabase db)
        {
            _db = db;
            _coll = _db.GetCollection<Master>("master");
        }

        public void ResolveNames(IList<T> players)
        {
            var qColl = _coll.AsQueryable();
            foreach (var player in players)
            {
                var master = qColl.Single(m => m.Id == player.PlayerId);
                player.Name = master.Name.First + " " + master.Name.Last;
            }
        }
    }
}