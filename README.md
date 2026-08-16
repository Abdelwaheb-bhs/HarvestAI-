 
 ![HarvestAI Logo](HarvestAI-logo-animated.svg)


<p align="center">
  <strong>Web Scraping &amp; Data Cleaning for LLMs</strong><br />
  Production-grade .NET library for scraping, crawling, and cleaning web content — purpose-built for LLM pipelines.
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/HarvestAI"><img src="https://img.shields.io/nuget/v/HarvestAI?label=nuget" alt="NuGet Version"></a>
  <a href="https://www.nuget.org/packages/HarvestAI"><img src="https://img.shields.io/nuget/dt/HarvestAI?label=downloads" alt="NuGet Downloads"></a>
  <a href="https://opensource.org/licenses/MIT"><img src="https://img.shields.io/badge/license-MIT-green" alt="License: MIT"></a>
  <a href="https://github.com/Abdelwaheb-bhs/HarvestAI-/stargazers"><img src="https://img.shields.io/github/stars/Abdelwaheb-bhs/HarvestAI-?style=flat" alt="GitHub Stars"></a>
</p>

<p align="center">
  <a href="#why-harvestai">Why HarvestAI</a> ·
  <a href="#features">Features</a> ·
  <a href="#installation">Installation</a> ·
  <a href="#quick-start">Quick Start</a> ·
  <a href="#api-reference">API Reference</a> ·
  <a href="#output-format">Output Format</a>
</p>

---

## Why HarvestAI?

Most scrapers return raw HTML. HarvestAI returns **LLM-ready chunks** — cleaned, tokenized, and structured so you can feed them directly into your AI pipeline without any post-processing.

- Handles JavaScript-heavy sites via full browser automation (Playwright)
- Runs a real Chromium browser so lazy-loaded posts, infinite-scroll feeds, and JS-gated content are fully rendered before extraction
- Manages authenticated sessions end-to-end — you log in once in a visible browser, and HarvestAI carries your cookies into the scrape automatically
- Bypasses CDN-gated images by intercepting authenticated image responses at the browser network level, then rewriting markdown links to local paths so they never expire
- Converts HTML to clean Markdown with metadata preserved per chunk
- Splits content into token-aware chunks tuned for your model's context window
- Runs a real Chromium browser so lazy-loaded posts, infinite-scroll feeds, and JS-gated content are fully rendered before extraction
- Manages authenticated sessions end-to-end — you log in once in a visible browser, and HarvestAI carries your cookies into the scrape automatically
- Bypasses CDN-gated images by intercepting authenticated image responses at the browser network level, then rewriting markdown links to local paths so they never expire

## Features

| Feature | Description |
|---|---|
| **Browser Automation** | Full JS rendering via Playwright (Chromium) |
| **Recursive Crawling** | Auto-discovers and follows internal links |
| **HTML → Markdown** | Intelligent conversion with structure preservation |
| **Smart Chunking** | Token-aware segmentation optimised for LLM context windows |
| **Concurrent Processing** | Configurable parallelism for high-throughput scraping |
| **Session Management** | Headless and visible-browser modes; supports login-required sites |
| **Image Downloading** | Downloads authenticated images and rewrites markdown links to local paths |
| **Content Cleaning** | Normalise and sanitise content for AI consumption |

## Prerequisites

- .NET 8.0 or later
- Windows, Linux, or macOS

## Installation

```bash
dotnet add package HarvestAI --version 1.0.0
```

Or via the NuGet Package Manager Console:

```powershell
Install-Package HarvestAI
```

### Playwright Browser Setup

After installing the package, install the Playwright Chromium browser:

```bash
dotnet tool install --global Microsoft.Playwright.CLI
playwright install chromium

On Linux:

playwright install --with-deps chromium

</details>

<details>
<summary><strong>Using a local build instead of the NuGet feed?</strong></summary>

```bash
dotnet add package HarvestAI --source C:\path\to\HarvestAI\bin\Release
```

</details>

## Quick Start

### Scrape a Public Page

```csharp
using HarvestAI.WebScraping;
using System;
using System.IO;

var outputDirectory = Path.Combine(Environment.CurrentDirectory, "harvest-output");
Directory.CreateDirectory(outputDirectory);

var service = new WebScrapingService(maxConcurrency: 2, outputDirectory: outputDirectory);
var browserSession = await service.LoadWebsite(needLogin: false);

try
{
    var url = "https://example.com";
    await browserSession.Page.GotoAsync(url);

    var userSession = new UserSession
    {
        UserId = "demo-user",
        NeedLogin = false,
        BrowserSession = browserSession
    };

    var (htmlContent, chunks) = await service.SinglePageScrapingAsync(userSession, metadata: true);

    var markdownPath = Path.Combine(outputDirectory, "example.com.md");
    using var writer = new StreamWriter(markdownPath, false);

    await writer.WriteLineAsync($"# Scraped result for {url}");
    await writer.WriteLineAsync();
    await writer.WriteLineAsync($"HTML length: {htmlContent.Length}");
    await writer.WriteLineAsync($"Chunk count: {chunks.Count}");
    await writer.WriteLineAsync();

    foreach (var chunk in chunks)
    {
        await writer.WriteLineAsync($"## Chunk {chunk.ChunkNumber}");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("### Metadata");

        if (chunk.Metadata.Count == 0)
        {
            await writer.WriteLineAsync("- None");
        }
        else
        {
            foreach (var item in chunk.Metadata)
            {
                await writer.WriteLineAsync($"- {item.Key}: {item.Value}");
            }
        }

        await writer.WriteLineAsync();
        await writer.WriteLineAsync("### Content");
        await writer.WriteLineAsync(chunk.Content);
        await writer.WriteLineAsync();
    }

    Console.WriteLine($"Saved markdown to: {markdownPath}");
}
finally
{
    await service.DisposeSessionAsync(browserSession);
}
```

<details>
<summary><strong>Scrape a login-required page</strong></summary>

The flow: open a visible browser → navigate to the target → let the user log in → `WaitForLoginAsync` returns once login is detected → navigate back to the target → scrape.

```csharp
using HarvestAI.WebScraping;
using System;
using System.IO;

var outputDirectory = Path.Combine(Environment.CurrentDirectory, "harvest-output");
Directory.CreateDirectory(outputDirectory);

var service = new WebScrapingService(maxConcurrency: 2, outputDirectory: outputDirectory);
var browserSession = await service.LoadWebsite(needLogin: true);
var url = "https://www.example.com/";
await browserSession.Page.GotoAsync(url);
bool loggedIn = await service.WaitForLoginAsync(browserSession, timeoutSeconds: 1200);

if (!loggedIn)
{
    Console.WriteLine("Login was not completed. Aborting.");
    await service.DisposeSessionAsync(browserSession);
    return;
}
await browserSession.Page.GotoAsync(url);

try
{
    var userSession = new UserSession
    {
        UserId = "demo-user",
        NeedLogin = true,
        BrowserSession = browserSession
    };

    var (htmlContent, chunks) = await service.SinglePageScrapingAsync(userSession, metadata: true);

    var markdownPath = Path.Combine(outputDirectory, "example.com.md");
    using var writer = new StreamWriter(markdownPath, false);

    await writer.WriteLineAsync($"# Scraped result for {url}");
    await writer.WriteLineAsync();
    await writer.WriteLineAsync($"HTML length: {htmlContent.Length}");
    await writer.WriteLineAsync($"Chunk count: {chunks.Count}");
    await writer.WriteLineAsync();

    foreach (var chunk in chunks)
    {
        await writer.WriteLineAsync($"## Chunk {chunk.ChunkNumber}");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("### Metadata");

        if (chunk.Metadata.Count == 0)
        {
            await writer.WriteLineAsync("- None");
        }
        else
        {
            foreach (var item in chunk.Metadata)
            {
                await writer.WriteLineAsync($"- {item.Key}: {item.Value}");
            }
        }

        await writer.WriteLineAsync();
        await writer.WriteLineAsync("### Content");
        await writer.WriteLineAsync(chunk.Content);
        await writer.WriteLineAsync();
    }

    Console.WriteLine($"Saved markdown to: {markdownPath}");
}
finally
{
    await service.DisposeSessionAsync(browserSession);
}
```

</details>

## API Reference

### Service Constructor

```csharp
new WebScrapingService(
    maxConcurrency: 5,      // Max parallel page downloads (default: 5)
    outputDirectory: null,  // Directory for saved Markdown files (optional)
    loggerFactory: null,    // Custom ILoggerFactory (optional)
    httpClient: null        // Custom HttpClient (optional)
);
```

### Scraping Methods

<details open>
<summary><strong>Scrape a single page (session-based)</strong></summary>

```csharp
var scrapingService = new WebScrapingService(maxConcurrency: 5, outputDirectory: "scraped_content");
var session = await scrapingService.LoadWebsite(needLogin: false);

try
{
    var userSession = new UserSession
    {
        UserId = "user123",
        BrowserSession = session,
        NeedLogin = false
    };

    var (htmlContent, chunks) = await scrapingService.SinglePageScrapingAsync(userSession, metadata: false);

    Console.WriteLine(htmlContent);
    Console.WriteLine($"Chunk count: {chunks.Count}");
}
finally
{
    await scrapingService.DisposeSessionAsync(session);
}
```

The `withImages` parameter (default `false`) downloads authenticated images to disk and rewrites the markdown links to point at the local files. Use it for sites that gate images behind login (Instagram, Twitter, etc.) — otherwise the image links in your saved markdown will break once the session expires.

```csharp
// Download and localise images (useful for login-gated CDNs)
var (htmlContent, chunks) = await scrapingService.SinglePageScrapingAsync(
    userSession,
    metadata: true,
    withImages: true
);
```

</details>

<details>
<summary><strong>Scrape a URL directly (no session required)</strong></summary>

Spins up its own headless browser, scrapes, and disposes everything automatically.

```csharp
await scrapingService.SinglePageScrapingAsync("https://example.com", metadata: true);
```

</details>

<details>
<summary><strong>Recursively crawl an entire website</strong></summary>

Starts from the current page in the session, discovers all internal links, and scrapes every page concurrently up to `maxConcurrency`. Returns one `FileContent` per page.

```csharp
var session = await scrapingService.LoadWebsite(needLogin: false);
await session.Page.GotoAsync("https://example.com");

var userSession = new UserSession
{
    UserId = "user123",
    BrowserSession = session,
    NeedLogin = false
};

// login = false for public sites, true when WaitForLoginAsync was used
List<FileContent> pages = await scrapingService.ScrapeWebsiteAsync(userSession, login: false, metadata: true);

Console.WriteLine($"Scraped {pages.Count} pages");
foreach (var page in pages)
{
    Console.WriteLine($"  {page.Metadata["Url"]} — {page.Sections.Count} chunks");
}
```

</details>

<details>
<summary><strong>Scrape a specific set of pages</strong></summary>

```csharp
var urls = new List<string>
{
    "https://example.com/page1",
    "https://example.com/page2",
    "https://example.com/page3"
};

var results = await scrapingService.ScrapeSelectedPagesAsync(userSession, urls, metadata: true);
```

</details>

<details>
<summary><strong>Scrape by element attribute</strong></summary>

Extracts only the elements whose attributes contain the supplied value — useful when you only want a specific section of a page (e.g. `"article-content"`, `"main-body"`).

```csharp
var (html, chunks) = await scrapingService.ScrapeByValueAsync(
    userSession,
    attributeValue: "article-content",
    metadata: true
);
```

</details>

### HTML → Markdown Converter

`HtmlToMarkdownConverterPerfect` is the converter used internally by all scraping methods. You can also call it directly when you already have HTML and just need it converted and chunked — no browser needed.

```csharp
using HarvestAI.DataFormats;

var converter = new HtmlToMarkdownConverterPerfect();

string html = "<h1>Hello</h1><p>This is a paragraph with some content for the LLM.</p>";

// Basic conversion — no metadata, default 512-token chunks
FileContent result = converter.Convert(html, maxTokensPerChunk: 512, metadata: false);

Console.WriteLine($"Chunks: {result.Sections.Count}");
foreach (var chunk in result.Sections)
{
    Console.WriteLine($"--- Chunk {chunk.ChunkNumber} ---");
    Console.WriteLine(chunk.Content);
}
```

Pass `metadata: true` to have each chunk include source details in its `Metadata` dictionary:

```csharp
FileContent result = converter.Convert(html, maxTokensPerChunk: 512, metadata: true);

foreach (var chunk in result.Sections)
{
    foreach (var item in chunk.Metadata)
        Console.WriteLine($"{item.Key}: {item.Value}");

    Console.WriteLine(chunk.Content);
}
```

Control chunk size to match your model's context window:

```csharp
// Smaller chunks for models with tight context limits
FileContent result = converter.Convert(html, maxTokensPerChunk: 256, metadata: false);

// Larger chunks to reduce total chunk count
FileContent result = converter.Convert(html, maxTokensPerChunk: 1024, metadata: false);
```

### Link Discovery

```csharp
// Returns all internal links found on the current page
var internalLinks = await scrapingService.GetInternalLinksList(userSession);

foreach (var link in internalLinks)
    Console.WriteLine(link);
```

```csharp
// Returns a snapshot of every URL the service has visited so far
IEnumerable<string> visited = scrapingService.GetVisitedUrls();
```

## Output Format

All scraping methods return `(string HtmlContent, List<Chunk> Chunks)`. The chunk list is what you feed to your LLM pipeline.

```csharp
public class FileContent
{
    public List<Chunk> Sections { get; set; }
    public string MimeType { get; set; }                       // "text/markdown"
    public Dictionary<string, string> Metadata { get; set; }  // URL, UserId, etc.
}

public class Chunk
{
    public string Content { get; set; }
    public int ChunkNumber { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}
```

### Saved File Layout

When `outputDirectory` is set, Markdown files are saved under a per-user subfolder:

```
outputDirectory/
├── userId1/
│   ├── page-slug-1.md
│   ├── page-slug-2.md
│   └── ...
└── userId2/
    └── ...
```

## The `metadata` Parameter

All scraping methods and the converter accept a `metadata` parameter (default `false`).

| Value | Behaviour |
|---|---|
| `metadata: false` | Each chunk contains only the extracted text content. |
| `metadata: true` | Each chunk's `Metadata` dictionary is populated with source details (URL, page title, etc.), useful for RAG pipelines where you need to cite the origin of each chunk. |

## Performance Tips

1. **Tune concurrency** — increase `maxConcurrency` (up to ~20) for large sites, but watch memory and CPU usage.
2. **Reuse sessions** — a single `BrowserSession` can scrape many pages from the same domain without re-launching a browser.
3. **Prefer `ScrapeSelectedPagesAsync`** for batches — it respects the concurrency limit automatically.
4. **Lower `maxTokensPerChunk`** if your model has a small context window; raise it to reduce chunk count for models with large windows.

## Error Handling

```csharp
try
{
    var content = await scrapingService.ScrapeWebsiteAsync(userSession, login: false);
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"Invalid session: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Scraping failed: {ex.Message}");
}
```

## Dependencies

| Package | Version | Purpose |
|---|---|---|
| Microsoft.Playwright | 1.45.0+ | Browser automation |
| HtmlAgilityPack | 1.11.61+ | HTML parsing |
| Microsoft.Extensions.Logging.Abstractions | 8.0.1+ | Logging interface |
| Tiktoken | 1.0.0+ | Token counting for LLM models |

## Known Limitations

- **Rate limiting** — aggressive concurrency may trigger blocks on some sites; lower `maxConcurrency` if you see 429s.
- **Very large sites** — 1000+ pages will require significant memory; consider scraping in batches with `ScrapeSelectedPagesAsync`.
- **Network timeouts** — default per-page timeout is 90 seconds; configure via Playwright browser options if needed.
- **Login detection** — `WaitForLoginAsync` detects login by watching for navigation away from known auth URLs. For unusual SSO flows, press Enter in the console to signal login manually.

## Contributing

Contributions and feedback are welcome. This package contains production-tested code from real scraping projects — if you find an edge case, open an issue or a PR.

## License

MIT — see `LICENSE` for details.

## Support

For issues, questions, or feature requests, open a ticket on the [GitHub repository](https://github.com/Abdelwaheb-bhs/HarvestAI-).
