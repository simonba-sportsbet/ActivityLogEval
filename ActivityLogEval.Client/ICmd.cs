using System.Threading.Tasks;

namespace ActivityLogEval.Client
{
    public interface ICmd
    {
        Task Run(string[] args);
    }
}
