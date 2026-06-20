using System.Text.Json.Serialization;
namespace HarvestAI.WebScraping;

/// <summary>
/// Represents the response payload that contains discovered internal links.
/// </summary>
internal sealed class InternalLinksListResponse
{
    /// <summary>
    /// Gets or sets the list of collected internal links.
    /// </summary>
    [JsonPropertyName("internalLinks")]
    public List<string> InternalLinks { get; set; }

    /// <summary>
    /// Gets or sets the user session identifier.
    /// </summary>
    [JsonPropertyName("userId")]

    public string UserId { get; set; }

    /// <summary>
    /// Creates an <see cref="InternalLinksListResponse"/> from a links list.
    /// </summary>
    /// <param name="list">The discovered internal links.</param>
    /// <returns>A response containing the provided links.</returns>
    public static async Task<InternalLinksListResponse> Build(List<string> list)
    
    {
        return  new InternalLinksListResponse{
            InternalLinks=list
        };
    }
}