using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MetadataApi
{
    public static class MetadataApi
    {
        [FunctionName("content-metadata")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "ContentMetadataDb",
                collectionName: "ContentMetadata",
                Id = "{Query.id}",
                PartitionKey = "{Query.id}",
                ConnectionStringSetting = "CosmosDbConnectionString")] ContentMetadata metadata,
            ILogger log)
        {
            if (metadata is null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(metadata);
        }
    }
}
