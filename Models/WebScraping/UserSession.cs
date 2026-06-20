using HarvestAI.DataFormats;
namespace HarvestAI.WebScraping;

/// <summary>
/// Represents per-user state for a scraping workflow.
/// </summary>
public class UserSession
{
    /// <summary>
    /// Gets or sets the unique user identifier.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gets or sets whether the session requires interactive login.
    /// </summary>
    public bool NeedLogin { get; set; }

    /// <summary>
    /// Gets or sets the active browser session.
    /// </summary>
    public BrowserSession BrowserSession { get; set; }

    /// <summary>
    /// Gets or sets pages scraped during this session.
    /// </summary>
    public List<FileContent> ScrapedPages { get; set; } = new();

    /// <summary>
    /// Gets or sets discovered internal links for this session.
    /// </summary>
    public List<string> InternalLinks { get; set; } = new();

    /// <summary>
    /// Gets or sets whether the session has been closed.
    /// </summary>
    public bool IsClosed { get; set; }
}