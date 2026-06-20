using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

using HarvestAI.DataFormats;
namespace HarvestAI.WebScraping;

/// <summary>
/// Represents a scraping response that includes raw and markdown-ready content.
/// </summary>
internal sealed class ScrapeResponse
{
    /// <summary>
    /// Gets or sets the raw scraped content.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }

    /// <summary>
    /// Gets or sets the markdown representation of the scraped content.
    /// </summary>
    [JsonPropertyName("markDownContent")]
    public string MarkDownContent { get; set; }

    /// <summary>
    /// Gets or sets the user session identifier.
    /// </summary>
    [JsonPropertyName("userId")]

    public string UserId { get; set; }

    /// <summary>
    /// Builds a scrape response from raw content and generated chunks.
    /// </summary>
    /// <param name="content">The raw scraped content.</param>
    /// <param name="chunks">The generated content chunks.</param>
    /// <returns>A response with raw and markdown content.</returns>
    public static async Task<ScrapeResponse> Build(string content,List<Chunk> chunks)
    {
        return new ScrapeResponse{
            Content=content,
            MarkDownContent=await GenerateMarkdownContentAsync(chunks)
        };
    }
    /// <summary>
    /// Generates markdown text from chunk content.
    /// </summary>
    /// <param name="chunks">The chunks to concatenate as markdown sections.</param>
    /// <returns>A markdown string representing all chunks.</returns>
    public static async Task<string> GenerateMarkdownContentAsync(List<Chunk> chunks)
        {
        
            var stringBuilder = new StringBuilder();

            foreach (var chunk in chunks)
            {
                // Append the chunk content and a separator
                stringBuilder.AppendLine(chunk.Content); // Add the chunk content
                stringBuilder.AppendLine("---");       
            }

            // Convert the StringBuilder content to a string and return it
            return stringBuilder.ToString();
        }
    

    
}