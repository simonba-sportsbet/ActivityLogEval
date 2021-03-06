﻿using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ActivityLogEval.MongoDb
{
    public interface IDbConnectionFactory
    {
        IMongoDatabase GetDatabase();
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private MongoClient? _client = null;
        private IMongoDatabase? _database = null;

        public IMongoDatabase GetDatabase() => _database ??= ConnectDatabase();

        private IMongoDatabase ConnectDatabase()
        {
            //BsonClassMap.RegisterClassMap<Selection>();
            //BsonClassMap.RegisterClassMap<Leg>();
            //BsonClassMap.RegisterClassMap<Bet>();

            _client = _client ??= new MongoClient("mongodb://localhost:27017");
            return _client.GetDatabase("activity-log");
        }
    }
}
