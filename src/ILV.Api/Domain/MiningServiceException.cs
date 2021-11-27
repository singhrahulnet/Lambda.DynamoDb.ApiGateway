using System;
using System.Net;

namespace ILV.Api.Domain
{
    public abstract class MiningServiceException : Exception
    {
        public MiningServiceException(string messageToLog = null, Exception actualException = null) : base(messageToLog, actualException) { }

        public abstract string ApiResponseMessage { get; }
        public abstract int StatusCode { get; }
    }

    public class MiningResultNotFoundException : MiningServiceException
    {
        public MiningResultNotFoundException(string messageToLog = null, Exception actualException = null) : base(messageToLog, actualException) { }

        public override string ApiResponseMessage => "The mining operation doesn't exist";

        public override int StatusCode => (int)HttpStatusCode.NotFound;
    }
}
