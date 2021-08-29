using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MetadataEventApi.Services
{
    public interface ICosmosMetadataService
    {
        Task<IActionResult> CreateContentMetadata(ContentMetadata metadata, Container container);
        Task<IActionResult> DeleteContentMetadata(ContentMetadata metadata, Container container);
        Task<IActionResult> UpdateContentMetadata(ContentMetadata metadata, Container container);
    }
}
