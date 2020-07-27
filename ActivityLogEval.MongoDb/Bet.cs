using System;
using System.Collections.Generic;
using ActivityLogEval.Abstractions;
using MongoDB.Bson.Serialization.Attributes;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS8601 // Possible null reference assignment.

namespace ActivityLogEval.MongoDb
{
    public class Selection : ISelection
    {
        public long? EventTypeId { get; set; }
        public long? EventId { get; set; }
        public long? MarketId { get; set; }
        public long? SelectionId { get; set; }
        public DateTime? EventDate { get; set; }
    }
    public class Leg : ILeg
    {
        public Selection Selection { get; set; }
        ISelection ILeg.Selection { get => Selection; set => Selection = value as Selection; }

        public decimal DecimalPrice { get; set; }
        public float? Handicap { get; set; }
    }

    public class Bet : IBet
    {
        [BsonId]
        public string BetId { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public List<Leg> Legs { get; set; } = new List<Leg>();
        IReadOnlyCollection<ILeg> IBet.Legs { get => Legs; }

        public void AddLeg(ILeg leg)
        {
            if (leg is Leg rec)
                Legs.Add(rec);
            else
                throw new ArgumentException("Mast be created by this asm");
        }
    }
}
