using System.Collections.Concurrent;

namespace HarvestAI.WebScraping;

/// <summary>
/// Stores shared state used while crawling pages concurrently.
/// </summary>
public class ScrapingContext
{
    /// <summary>
    /// Gets the set of URLs that have already been visited.
    /// </summary>
    public HashSet<string> VisitedUrls { get; } = new HashSet<string>();

    /// <summary>
    /// Gets the queue of URLs waiting to be scraped.
    /// </summary>
    public ConcurrentQueue<string> UrlQueue { get; } = new ConcurrentQueue<string>();

    /// <summary>
    /// Gets the semaphore that limits crawl concurrency.
    /// </summary>
    public SemaphoreSlim ThrottleSemaphore { get; }
    
    /// <summary>
    /// Initializes a new scraping context.
    /// </summary>
    /// <param name="maxConcurrency">Maximum number of concurrent scrape tasks.</param>
    public ScrapingContext(int maxConcurrency)
    {
        ThrottleSemaphore = new SemaphoreSlim(maxConcurrency);
    }
}