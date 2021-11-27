using System;
using Amazon.DynamoDBv2.DataModel;

namespace ILV.Api.Models
{
    public class NFT
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
