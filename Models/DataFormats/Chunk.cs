using System;
using System.Collections.Generic;

namespace HarvestAI.DataFormats
{
    /// <summary>
    /// Represents a chunk of processed content with associated metadata.
    /// Used for storing segments of text that have been extracted and cleaned for LLM models.
    /// </summary>
    public class Chunk
    {
        /// <summary>
        /// Gets or sets the processed content of this chunk.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the chunk number/index.
        /// </summary>
        public int ChunkNumber { get; set; }

        /// <summary>
        /// Gets or sets whether this chunk is a separator.
        /// </summary>
        public bool IsSeparator { get; set; }

        /// <summary>
        /// Gets or sets metadata associated with this chunk.
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();

        /// <summary>
        /// Initializes a new instance of the Chunk class.
        /// </summary>
        public Chunk() { }
        /// <param name="content">The chunk content</param>
        /// <param name="chunkNumber">The chunk number</param>
        /// <param name="metadata">Optional metadata dictionary</param>
        public Chunk(string content, int chunkNumber, Dictionary<string, object> metadata = null)
        {
            Content = content;
            ChunkNumber = chunkNumber;
            Metadata = metadata ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Creates a metadata dictionary with sentence completion status.
        /// </summary>
        /// <param name="sentencesAreComplete">Whether the chunk contains complete sentences</param>
        /// <returns>A metadata dictionary</returns>
        public static Dictionary<string, object> Meta(bool sentencesAreComplete = true)
        {
            return new Dictionary<string, object>
            {
                { "sentencesAreComplete", sentencesAreComplete }
            };
        }
    }
}
