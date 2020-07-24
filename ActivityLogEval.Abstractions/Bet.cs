using System;
using System.Collections.Generic;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

namespace ActivityLogEval.Abstractions
{
    public class Selection
    {
        public long? EventTypeId { get; set; }
        public long? EventId { get; set; }
        public long? MarketId { get; set; }
        public long? SelectionId { get; set; }
        public DateTime? EventDate { get; set; }
    }
    public class Leg
    {
        public Selection Selection { get; set; }
        public decimal DecimalPrice { get; set; }
        public float? Handicap { get; set; }
    }

    public class Bet
    {
        public string BetId { get; set; } 
        public DateTimeOffset Timestamp { get; set; }

        public IList<Leg> Legs { get; set; }
    }
}
