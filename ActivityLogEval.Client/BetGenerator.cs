using System;
using System.Collections.Generic;
using System.Text;
using ActivityLogEval.Abstractions;

namespace ActivityLogEval.Client
{
    public interface IBetGenerator
    {
        Bet CreateRandomBet();
    }

    public class BetGenerator : IBetGenerator
    {
        private readonly Random _random = new Random();

        public Bet CreateRandomBet()
        {
            var bet = new Bet
            {
                BetId = DateTime.Now.ToString("yyMMddHHmmss" + _random.Next(1000).ToString("00000")),
                Timestamp = DateTimeOffset.Now,
                Legs = new List<Leg>
                { 
                    new Leg
                    {
                        Selection = new Selection
                        {
                            EventId = _random.Next(100, 200),
                            EventTypeId = _random.Next(10, 20),
                            MarketId = _random.Next(30, 40),
                            SelectionId = _random.Next(0, 20),
                            EventDate = DateTime.Today.AddDays(_random.Next(0, 100))
                        },
                        DecimalPrice = _random.Next(10, 10000),
                        Handicap = 0.1f * _random.Next(1, 9)
                    }
                }
            };

            return bet;
        }
    }
}
