using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Mvc;

namespace MetadataEventApi.Services
{
    public class CosmosMetadataService : ICosmosMetadataService
    {
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
                    throw;
                }
            }
        }
        public async Task<IActionResult> DeleteContentMetadata(ContentMetadata metadata, Container container)
        {
            await container.DeleteItemAsync<ContentMetadata>(metadata.id, new PartitionKey(metadata.id));

            return new OkObjectResult("Item deleted successfully!");
        }      

        public async Task<IActionResult> UpdateContentMetadata(ContentMetadata metadata, Container container)
        {
            await container.UpsertItemAsync<ContentMetadata>(metadata, new PartitionKey(metadata.id));

            return new OkObjectResult("Item updated successfully!");
        }
    }
}