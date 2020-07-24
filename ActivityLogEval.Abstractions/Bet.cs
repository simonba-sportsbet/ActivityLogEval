using System;

namespace ActivityLogEval.Abstractions
{
    public class Selection
    {
        public long? EventTypeId { get; set; }
        public long? EventId { get; set; }
        public long? MarketId { get; set; }
        public long? SelectionId { get; set; }
        public decimal DecimalPrice { get; set; }
        public float? Handicap { get; set; }
        public DateTime? EventDate { get; set; }
    }

    public class Bet
    {
        public string BetId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }

    public class Leg
    {
        public long EventId { get; set; }
        public long EvenTypetId { get; set; }
    }
}
