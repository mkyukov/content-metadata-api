using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MetadataEventApi.Services
{
    public class CosmosMetadataService : ICosmosMetadataService
    {
        private readonly ILogger _logger;

        public CosmosMetadataService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> CreateContentMetadata(ContentMetadata metadata, Container container)
        {
            try
            {
                var response = await container.CreateItemAsync(metadata);

                return new CreatedResult(container.Id,response.Resource);
            }
            catch (CosmosException ex)
            {
                if(ex.StatusCode == HttpStatusCode.Conflict)
                {
                    return new ConflictObjectResult("This item already exists!");
                } else
                {
                    _logger.LogError(ex, "There was an error creating a new ContentMetadata item in Cosmos DB!");
                    throw;
                }
            }
        }
        public async Task<IActionResult> DeleteContentMetadata(ContentMetadata metadata, Container container)
        {
            try
            {
                await container.DeleteItemAsync<ContentMetadata>(metadata.id, new PartitionKey(metadata.id));

                return new OkObjectResult("Item deleted successfully!");
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return new ConflictObjectResult("Could not find the item to delete!");
                }
                else
                {
                    _logger.LogError(ex, "There was an error deleting a ContentMetadata item from Cosmos DB!");
                    throw;
                }
            }
            
        }      

        public async Task<IActionResult> UpdateContentMetadata(ContentMetadata metadata, Container container)
        {
            try
            {
                await container.UpsertItemAsync<ContentMetadata>(metadata, new PartitionKey(metadata.id));

                return new OkObjectResult("Item updated successfully!");
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return new ConflictObjectResult("Could not find the item to update!");
                }
                else
                {
                    _logger.LogError(ex, "There was an error updating the ContentMetadata item in Cosmos DB!");
                    throw;
                }
            }

        }
    }
}