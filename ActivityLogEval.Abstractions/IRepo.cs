using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActivityLogEval.Abstractions
{
    public interface IRepo
    {
        Task ResetAsync();

        Task InsertBetAsync(Bet bet);
        Task InsertBetsAsync(IEnumerable<Bet> bets);

        IEnumerable<Bet> QueryBetById(params string[] betId);
    }
}
