using System;
using System.Collections.Generic;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

namespace ActivityLogEval.Abstractions
{
    public interface ISelection
    {
        public long? EventTypeId { get; set; }
        public long? EventId { get; set; }
        public long? MarketId { get; set; }
        public long? SelectionId { get; set; }
        public DateTime? EventDate { get; set; }
    }
    public interface ILeg
    {
        public ISelection Selection { get; set; }
        public decimal DecimalPrice { get; set; }
        public float? Handicap { get; set; }
    }

    public interface IBet
    {
        public string BetId { get; set; } 
        public DateTimeOffset Timestamp { get; set; }

        public IReadOnlyCollection<ILeg> Legs { get; }
        public void AddLeg(ILeg leg);
    }
}
