using System;
using System.Collections.Generic;

namespace HarvestAI.DataFormats
{
    /// <summary>
    /// Represents the structured content extracted from a file or web page.
    /// Contains chunks of processed content along with metadata.
    /// </summary>
    public class FileContent
    {
        /// <summary>
        /// Gets or sets the collection of chunks that make up the content.
        /// </summary>
        public List<Chunk> Sections { get; set; } = new();

        /// <summary>
        /// Gets or sets the MIME type of the original content.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets metadata associated with the content.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
