using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActivityLogEval.Abstractions
{
    public interface IRecommendationRepo
    {
        Task ResetAsync();

        Task InsertRecommendationAsync(IRecommendation recommendation);
        Task InsertRecommendationsAsync(IEnumerable<IRecommendation> recommendations);

        IEnumerable<IRecommendation> GetAllRecommendations();
        IEnumerable<IRecommendation> QueryRecommendationById(params string[] recIds);

        IRecommendation CreateNewRecommendation();

        IRecommendation[] DeserializeRecommendation(string jsonStream);
    }
}
