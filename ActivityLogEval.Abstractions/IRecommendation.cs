using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityLogEval.Abstractions
{
    public enum RecommendationType
    {
        NoChange,
        LowerPrice
    }

    public interface IRecommendation
    {
        public string RecommendationId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public long EventId { get; set; }

        public long MarketId { get; set; }

        public long SelectionId { get; set; }

        public float MarketHandicap { get; set; }

        public double SelectionProbability { get; set; }

        public double Prediction { get; set; }

        public double ThresholdBreached { get; set; }

        public RecommendationType SelectionChangeRecommendation { get; set; }

        public IList<string> InputBetIds { get; set; }
    }
}
