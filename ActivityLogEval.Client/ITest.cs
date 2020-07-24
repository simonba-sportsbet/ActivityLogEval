using System.Threading.Tasks;

namespace ActivityLogEval.Client
{
    public interface ITest
    {
        Task Run(string[] args);
    }
}
