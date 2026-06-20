# HarvestAI - Web Scraping & Data Cleaning for LLMs
![alt text](logoHarvest.png)
A production-grade NuGet package for web scraping, crawling, and data cleaning specifically designed to prepare web content for Large Language Model (LLM) processing.

## Features

- **Advanced Web Scraping**: Leverage Playwright for browser automation and JavaScript rendering
- **Recursive Website Crawling**: Automatically discover and scrape internal links
- **HTML to Markdown Conversion**: Intelligent conversion with formatting preservation
- **Smart Content Chunking**: Automatic text segmentation optimized for LLM token limits
- **Concurrent Processing**: Process multiple pages simultaneously for maximum performance
- **Session Management**: Support for login-required websites with headless and non-headless modes
- **Content Cleaning**: Normalize and sanitize content specifically for AI model consumption

## Prerequisites

- .NET 8.0 or later
- Windows, Linux, or macOS

## Installation

```bash
dotnet add package HarvestAI --prerelease
```

Or via NuGet Package Manager:

```
Install-Package HarvestAI -Prerelease
```

### Playwright Browser Setup

Install the companion setup tool and run it once to download the Chromium browser binaries required by HarvestAI:

```bash
dotnet tool install --global HarvestAI.Setup
harvestai-setup
```

On Linux, use this form if you want the Playwright system dependencies installed too:

```bash
harvestai-setup --with-deps
```

### Install In Another Project

If you are testing against a local build instead of NuGet, point your project at the local package source:

```bash
dotnet add package HarvestAI --source C:\Users\User\Desktop\Abdelwaheb\HarvestAI\bin\Release
```

## Demo Usage

The snippet below follows the same overall flow as the external smoke test: create the service, open a browser session, scrape content, and dispose the session. It is intentionally shorter and more general than the test project code.

The `metadata` parameter is optional on the scraping methods. Use `metadata: false` when you only want the extracted content, and `metadata: true` when you want each chunk to include metadata such as source details.

### Console Demo

```csharp
using HarvestAI.WebScraping;
using System;
using System.IO;

var outputDirectory = Path.Combine(Environment.CurrentDirectory, "harvest-output");
Directory.CreateDirectory(outputDirectory);

var service = new WebScrapingService(maxConcurrency: 3, outputDirectory: outputDirectory);
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

    Console.WriteLine($"Scraped {chunks.Count} chunks from {url}");
    Console.WriteLine($"Output is ready in: {outputDirectory}");
}
finally
{
    await service.DisposeSessionAsync(browserSession);
}
```

### Scrape A Single Page

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

### Full Smoke-Test Style Export

If you want a markdown-writing example like the external smoke test, use the same service/session flow and write the returned chunks to disk. The important part is the call shape, not the exact markdown layout:

```csharp
var (htmlContent, chunks) = await service.SinglePageScrapingAsync(userSession, metadata: true);
```

### Scrape A Specific URL

```csharp
await scrapingService.SinglePageScrapingAsync("https://example.com", metadata: true);
```

### Get Internal Links

```csharp
var internalLinks = await scrapingService.GetInternalLinksList(userSession);
```

### Scrape by Element Attribute

```csharp
var (html, chunks) = await scrapingService.ScrapeByValueAsync(
    userSession, 
    attributeValue: "article-content"
);
```

### Scrape Selected Pages

```csharp
var urls = new List<string> 
{ 
    "https://example.com/page1",
    "https://example.com/page2",
    "https://example.com/page3"
};

var results = await scrapingService.ScrapeSelectedPagesAsync(userSession, urls);
```

## Output Format

All scraping operations return content structured in `FileContent` objects:

```csharp
public class FileContent
{
    public List<Chunk> Sections { get; set; }        // Content chunks
    public string MimeType { get; set; }              // "text/markdown"
    public Dictionary<string, string> Metadata { get; set; }  // URL, UserId, etc.
}

public class Chunk
{
    public string Content { get; set; }               // Actual content
    public int ChunkNumber { get; set; }              // Sequence number
    public Dictionary<string, object> Metadata { get; set; }  // Additional metadata
}
```

## Login-Required Websites

For websites requiring authentication:

```csharp
// Launch browser in non-headless mode for manual login
var session = await scrapingService.LoadWebsite(needLogin: true);

var userSession = new UserSession 
{ 
    UserId = "user123",
    NeedLogin = true,    // Important!
    BrowserSession = session
};

// Browser window will open - perform manual login
// After login is detected, scraping will proceed automatically
var content = await scrapingService.SinglePageScrapingAsync(userSession);
```

## Configuration

### Constructor Parameters

```csharp
new WebScrapingService(
    maxConcurrency: 5,           // Max concurrent page downloads (default: 5)
    outputDirectory: null,       // Where to save intermediate files (optional)
    loggerFactory: null,         // Custom logger (optional)
    httpClient: null            // Custom HTTP client (optional)
);
```

### Chunk Size Control

Control how content is chunked by modifying the max tokens per chunk:

```csharp
// In the HTML to Markdown converter (default: 512 tokens per chunk)
converter.Convert(htmlContent, maxTokensPerChunk: 1024);
```

## Performance Tips

1. **Adjust Concurrency**: For large sites, increase `maxConcurrency` (up to 20), but monitor resource usage

2. **Use Sessions**: Reuse browser sessions when scraping multiple pages from the same domain

3. **Check Validity**: Validate sessions before long-running operations:
   
   ```csharp
   bool isInvalid = await scrapingService.IsSessionInvalid(session);
   ```

## Error Handling

```csharp
try
{
    var content = await scrapingService.ScrapeWebsiteAsync(userSession, login: false);
}
catch (ArgumentNullException ex)
{
    // Null browser session
    Console.WriteLine($"Invalid session: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Scraping failed: {ex.Message}");
}
```

## Limitations & Known Issues

- **JavaScript rendering**: Full JavaScript execution is performed (unlike simple HTTP clients)
- **Rate limiting**: Some sites may block aggressive concurrent requests; adjust `maxConcurrency` accordingly
- **Large websites**: For sites with 1000+ pages, expect significant memory and time usage
- **Network timeouts**: Default timeout is 90 seconds per page; adjust via browser configuration if needed

## Output Files

When `outputDirectory` is specified, content is saved to disk in Markdown format:

```
outputDirectory/
├── userId1/
│   ├── page_slug_1.md
│   ├── page_slug_2.md
│   └── ...
├── userId2/
│   └── ...
```

## Building the Package

To build a NuGet package locally:

```bash
dotnet pack -c Release
```

The `.nupkg` file will be created in `bin/Release/`.

To publish to NuGet.org:

```bash
dotnet nuget push bin/Release/HarvestAI.0.1.0-pre.nupkg --api-key <your-api-key> --source https://api.nuget.org/v3/index.json
```

## Dependencies

- **Microsoft.Playwright** (1.45.0+) - Browser automation
- **HtmlAgilityPack** (1.11.61+) - HTML parsing and manipulation
- **Microsoft.Extensions.Logging.Abstractions** (8.0.1+) - Logging interface
- **Tiktoken** (1.0.0+) - Token counting for LLM models

## License

MIT License - See LICENSE file for details

## Contributing

This package contains production-tested code from enterprise scraping projects. Contributions and feedback are welcome!

## Support

For issues, questions, or feature requests, please visit the [GitHub repository](https://github.com/yourusername/HarvestAI).

## Changelog

### Version 0.1.0-pre (Initial Pre-Release)

- Initial release
- Web scraping with Playwright
- Recursive crawling support
- HTML to Markdown conversion
- Smart chunking for LLM preparation
- Session management with login support
- Concurrent page processing

---

**Note**: This is a pre-release (0.1.0-pre) package. The API may change based on early adopter feedback. Use with caution in production environments.


