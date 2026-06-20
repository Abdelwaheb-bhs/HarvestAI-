# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0-pre] - 2024-02-24

### Added
- Initial pre-release version
- Web scraping with Microsoft Playwright browser automation
- Recursive website crawling with internal link detection
- Advanced URL normalization and validation
- HTML to Markdown conversion with intelligent formatting
- Smart content chunking optimized for LLM token limits
- Session management supporting both headless and non-headless modes
- Login support for authentication-required websites
- Concurrent page processing with configurable throttling
- Content cleanup and normalization for LLM preparation
- Comprehensive logging support via Microsoft.Extensions.Logging
- Per-user scraping context isolation
- File-based output for intermediate results

### Removed
- Removed vector database dependencies (Qdrant integration)
- Removed embedding service integration
- Removed document repository and collection management
- Removed RAGProject-specific dependencies
- Removed category and collectionName parameters from API

### Changed
- Simplified constructor removing database-related dependencies
- Updated namespace from Nexia to HarvestAI
- Cleaner, focused API surface for web scraping only
- Production-tested code base converted to standalone package

### Notes
- This is a pre-release version (0.1.0-pre)
- API may change based on feedback
- Recommended for testing and evaluation
- Not recommended for production use yet

---

## Future Planned Features

- [ ] Async stream processing for large websites
- [ ] Custom CSS/XPath selectors for content extraction
- [ ] Proxy rotation support
- [ ] Rate limiting and backoff strategies
- [ ] Cache layer for repeated URL crawls
- [ ] Performance metrics and analytics
- [ ] Webhook support for progress notifications
- [ ] Batch job management
- [ ] Cloud storage integration options
