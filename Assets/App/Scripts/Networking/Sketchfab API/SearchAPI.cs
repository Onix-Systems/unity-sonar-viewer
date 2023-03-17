
using System.Threading;
using System.Threading.Tasks;
using RestHTTP;
using SketchfabAPI.Entities;

namespace SketchfabAPI
{
    public class SearchAPI : SketchfabAPIClientBase
    {
        public const string ModelsSearchType = "models";
        public const string SearchEndpoint = Constants.BaseURL + "/v3/search";

        public async Task<Result<SearchResultsEntity>> Search(SearchDTO searchDTO, CancellationToken cancellationToken)
        {
            Result<SearchResultsEntity> searchResults =
               await RESTClient.GetAsync<SearchResultsEntity>(
                   url: SearchEndpoint, 
                   parameters: searchDTO, 
                   cancellationToken: cancellationToken);

            return searchResults;
        }
    }
}