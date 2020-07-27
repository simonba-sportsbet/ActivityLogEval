using System;
using System.Collections.Generic;
using ActivityLogEval.Abstractions;
using MongoDB.Bson.Serialization.Attributes;

namespace ActivityLogEval.MongoDb
{
    public class Recommendation : IRecommendation
    {
        [BsonId]
        public string RecommendationId { get; set; } = string.Empty;

        public DateTimeOffset Timestamp { get; set; }

        public long EventId { get; set; }

        public long MarketId { get; set; }

        public long SelectionId { get; set; }

        public float MarketHandicap { get; set; }

        public double SelectionProbability { get; set; }

        public double Prediction { get; set; }

        public double ThresholdBreached { get; set; }

        public RecommendationType SelectionChangeRecommendation { get; set; }

        public IList<string> InputBetIds { get; set; } = new List<string>();
    }
}
