using System;
using System.Collections.Generic;
using System.Text;
using ActivityLogEval.Abstractions;

namespace ActivityLogEval.Client
{
    public interface IBetGenerator
    {
        IBet CreateRandomBet();
    }

    public class BetGenerator : IBetGenerator
    {
        private readonly IBetRepo _repo;

        public BetGenerator(IBetRepo repo)
        {
            _repo = repo;
        }

        private readonly Random _random = new Random();

        public IBet CreateRandomBet()
        {
            var selection = _repo.CreateNewSelection();
            selection.EventId = _random.Next(100, 200);
            selection.EventTypeId = _random.Next(10, 20);
            selection.MarketId = _random.Next(30, 40);
            selection.SelectionId = _random.Next(0, 20);
            selection.EventDate = DateTime.Today.AddDays(_random.Next(0, 100));

            var leg = _repo.CreateNewLeg();
            leg.DecimalPrice = _random.Next(10, 10000);
            leg.Handicap = 0.1f * _random.Next(1, 9);
            leg.Selection = selection;

            var bet = _repo.CreateNewBet();

            bet.BetId = DateTime.Now.ToString("yyMMddHHmmss" + _random.Next(1000).ToString("00000"));
            bet.Timestamp = DateTimeOffset.Now;
            
            bet.AddLeg(leg);

            return bet;
        }
    }
}
