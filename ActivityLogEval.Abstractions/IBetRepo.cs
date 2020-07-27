using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ActivityLogEval.Abstractions
{
    public interface IBetRepo
    {
        Task ResetAsync();

        Task InsertBetAsync(IBet bet);
        Task InsertBetsAsync(IEnumerable<IBet> bets);

        IEnumerable<IBet> GetAllBets();
        IEnumerable<IBet> QueryBetById(params string[] betId);

        IBet CreateNewBet();
        ILeg CreateNewLeg();
        ISelection CreateNewSelection();

        IBet[] DeserializeBet(string jsonStream);
    }
}
