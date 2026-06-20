using System.Text.Json.Serialization;

namespace HarvestAI.WebScraping
{
    /// <summary>
    /// Represents the result of scraping a single URL.
    /// </summary>
    public class ScrapeResult
    {
        /// <summary>
        /// The URL that was scraped.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// The scraped content from the URL (e.g., Markdown or plain text).
        /// Null if the scraping failed.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// An error message if the scraping failed for this URL.
        /// Null if the scraping was successful.
        /// </summary>
        [JsonPropertyName("error")]
        public string Error { get; set; }

        /// <summary>
        /// The user identifier associated with this scraping result.
        /// </summary>
        [JsonPropertyName("userId")]

        public string UserId { get; set; }
    }
}