using System.Net;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

using ILV.Api.Domain;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ILV.Api
{
    public class Functions
    {
        public const string ID_QUERY_STRING_NAME = "Id";
        IMiningService _miningService;

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
            _miningService = Startup.GetService<IMiningService>();
        }

        /// <summary>
        /// Constructor used for testing by passing in custom service
        /// </summary>
        /// <param name="miningService"></param>
        public Functions(IMiningService miningService)
        {
            _miningService = miningService;
        }

        /// <summary>
        /// A Lambda function that starts the mining process
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> AddNFTAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {

            var id = await _miningService.StartMining();

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = id.ToString()
            };
        }

        /// <summary>
        /// A Lambda function that returns the nft identified by id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> GetNFTAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string id = ExtractIdFromRequest(request);

            if (string.IsNullOrEmpty(id))
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Body = $"Missing required parameter {ID_QUERY_STRING_NAME}"
                };
            }

            var result = await _miningService.GetMiningResult(id);

            if (result == -1)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = result.ToString()
            };
        }

        private static string ExtractIdFromRequest(APIGatewayProxyRequest request)
        {
            string nftId = null;
            if (request.PathParameters != null && request.PathParameters.ContainsKey(ID_QUERY_STRING_NAME))
                nftId = request.PathParameters[ID_QUERY_STRING_NAME];
            else if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey(ID_QUERY_STRING_NAME))
                nftId = request.QueryStringParameters[ID_QUERY_STRING_NAME];
            return nftId;
        }
    }
}
