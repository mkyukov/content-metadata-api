using System;
using System.Collections.Generic;
using System.Text;

namespace metadata_api.Models
{
    public class EventModel
    {
        /// <summary>
        /// Used to determine what action the processor must take based on an HTTP verb
        /// </summary>
        public HttpVerb Verb { get; set; }
        public ContentMetadata Metadata { get; set; }
    }
}
