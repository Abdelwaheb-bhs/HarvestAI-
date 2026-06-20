using Microsoft.Playwright;

namespace HarvestAI.WebScraping;

/// <summary>
/// Represents an active Playwright browser session used by scraping operations.
/// </summary>
public class BrowserSession
{
    /// <summary>
    /// Gets or sets the Playwright runtime instance.
    /// </summary>
    public IPlaywright Playwright { get; set; }

    /// <summary>
    /// Gets or sets the browser instance.
    /// </summary>
    public IBrowser Browser { get; set; }

    /// <summary>
    /// Gets or sets the browser context associated with this session.
    /// </summary>
    public IBrowserContext Context { get; set; } // Added

    /// <summary>
    /// Gets or sets the currently active page.
    /// </summary>
    public IPage Page { get; set; }

    /// <summary>
    /// Gets or sets whether the session has already been disposed.
    /// </summary>
    public bool IsClosed { get; set; }

    /// <summary>
    /// Initializes a new browser session without an explicit browser context.
    /// </summary>
    /// <param name="playwright">The Playwright runtime instance.</param>
    /// <param name="browser">The browser instance.</param>
    /// <param name="page">The active browser page.</param>
    public BrowserSession(IPlaywright playwright, IBrowser browser, IPage page)
        : this(playwright, browser, null, page)
    {
    }

    /// <summary>
    /// Initializes a new browser session.
    /// </summary>
    /// <param name="playwright">The Playwright runtime instance.</param>
    /// <param name="browser">The browser instance.</param>
    /// <param name="context">The browser context.</param>
    /// <param name="page">The active browser page.</param>
    public BrowserSession(IPlaywright playwright, IBrowser browser, IBrowserContext context, IPage page)
    {
        Playwright = playwright;
        Browser = browser;
        Context = context;
        Page = page;
        IsClosed = false;
    }

}