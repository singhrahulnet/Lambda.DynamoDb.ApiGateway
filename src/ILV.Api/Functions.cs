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
            _miningService = new Startup().GetService<IMiningService>();
        }

        /// <summary>
        /// Constructor used for integration testing by passing in custom service
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
        public async Task<APIGatewayProxyResponse> StartMiningAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var id = await _miningService.StartMining();

            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.Created,
                Body = id.ToString()
            };
        }

        /// <summary>
        /// A Lambda function that returns the mining status identified by id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> GetMiningStatusAsync(APIGatewayProxyRequest request, ILambdaContext context)
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

            int result;

            try
            {
                result = await _miningService.GetMiningResult(id);
            }
            catch (MiningServiceException ex)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = ex.StatusCode,
                    Body = ex.ApiResponseMessage
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
            string miningId = null;
            if (request.PathParameters != null && request.PathParameters.ContainsKey(ID_QUERY_STRING_NAME))
                miningId = request.PathParameters[ID_QUERY_STRING_NAME];
            else if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey(ID_QUERY_STRING_NAME))
                miningId = request.QueryStringParameters[ID_QUERY_STRING_NAME];
            return miningId;
        }
    }
}
