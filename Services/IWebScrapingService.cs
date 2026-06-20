using Microsoft.Playwright;
using HarvestAI.DataFormats;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarvestAI.WebScraping;

/// <summary>
/// Interface for web scraping service providing methods to crawl, scrape, and clean web content
/// for Large Language Model (LLM) preprocessing.
/// </summary>
public interface IWebScrapingService
{
    /// <summary>
    /// Loads a website and creates a browser session for scraping.
    /// </summary>
    Task<BrowserSession> LoadWebsite(bool needLogin);

    /// <summary>
    /// Disposes a browser session and cleans up resources.
    /// </summary>
    Task DisposeSessionAsync(BrowserSession session);

    /// <summary>
    /// Scrapes a website recursively, following internal links and cleaning content.
    /// </summary>
    Task<List<FileContent>> ScrapeWebsiteAsync(UserSession userSession, bool login, bool metadata = false);

    /// <summary>
    /// Scrapes a single page with the current browser session.
    /// </summary>
    Task<(string HtmlContent, List<Chunk> Chunks)> SinglePageScrapingAsync(UserSession userSession, bool metadata = false);

    /// <summary>
    /// Scrapes a single page from a URL.
    /// </summary>
    Task SinglePageScrapingAsync(string url, bool metadata = false);

    /// <summary>
    /// Retrieves all internal links from the current page.
    /// </summary>
    Task<List<string>> GetInternalLinksList(UserSession userSession);

    /// <summary>
    /// Checks if a browser session is still valid for scraping.
    /// </summary>
    Task<bool> IsSessionInvalid(BrowserSession session);

    /// <summary>
    /// Gets all URLs that have been visited during scraping.
    /// </summary>
    IEnumerable<string> GetVisitedUrls();

    /// <summary>
    /// Scrapes page elements matching a specific attribute value.
    /// </summary>
    Task<(string HtmlContent, List<Chunk> Chunks)> ScrapeByValueAsync(UserSession userSession, string attributeValue, bool metadata = false);

    /// <summary>
    /// Scrapes a specific list of URLs and returns cleaned content for each.
    /// </summary>
    Task<List<FileContent>> ScrapeSelectedPagesAsync(UserSession userSession, List<string> urls, bool metadata = false);
}