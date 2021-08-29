using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MetadataEventApi.Services;
using metadata_api.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace MetadataEventApi
{
    public class ProcessMetadataEvent
    {
        private readonly ICosmosMetadataService _metadataProcessor;
        public ProcessMetadataEvent(ICosmosMetadataService metadataProcessor)
        {
            _metadataProcessor = metadataProcessor;
        }

        /// <summary>
        /// Expects a content metadata object that gets saved in Cosmos DB.
        /// In case of an exception the function retries 5 times, every 10 seconds before ultimately failing.
        /// </summary>
        [FunctionName("process-event")]
        [FixedDelayRetry(5, "00:00:10")]
        public async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Function, "put", "patch", "delete", Route = null)] HttpRequest req,
                ExecutionContext context,
                ILogger log
            )
        {
            try
            {
                var cosmosDbContainer = GetCosmosDbMetadataContainer(context);

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                // ToDo [mky] (29.08.2021) : Event processing happens in batches, but for simplicity's sake processing a single one.
                EventModel receivedEvent = JsonConvert.DeserializeObject<EventModel>(requestBody);

                // When processing events, the request object will not be available, hence storing the http verb in the message itself (emitted from the CMS)
                switch (receivedEvent.Verb)
                {
                    case HttpVerb.PUT:
                        return  await _metadataProcessor.CreateContentMetadata(receivedEvent.Metadata, cosmosDbContainer);
                    case HttpVerb.PATCH:
                        return new OkObjectResult(await _metadataProcessor.UpdateContentMetadata(receivedEvent.Metadata, cosmosDbContainer));
                    case HttpVerb.DELETE:
                        return new OkObjectResult(await _metadataProcessor.DeleteContentMetadata(receivedEvent.Metadata, cosmosDbContainer));
                    default:
                        return new BadRequestObjectResult("Unexpected http verb used! Use PUT, PATCH or DELETE!");
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "There was an issue processing an event hub event!");
                throw ex;
            }
        }

        public static Container GetCosmosDbMetadataContainer(ExecutionContext context)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var EndpointUri = configurationBuilder["CosmosDbEndpoint"];
            var PrimaryKey = configurationBuilder["CosmosDbPrimaryKey"];
            var databaseId = configurationBuilder["CosmosDbDatabaseId"];
            var containerId = configurationBuilder["CosmosDbContainer"];

            var cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);

            var container = cosmosClient.GetContainer(databaseId, containerId);

            return container;
        }
    }
}
