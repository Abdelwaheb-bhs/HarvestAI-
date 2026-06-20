using System;

namespace HarvestAI.Chunkers;

/// <summary>
/// Defines configuration options for markdown chunk generation.
/// </summary>
public class MarkDownChunkerOptions
{
    private int _maxTokensPerChunk = 1024;
    private int _overlap = 0;

    /// <summary>
    /// Maximum number of tokens per chunk (must be > 0)
    /// </summary>
    public int MaxTokensPerChunk
    {
        get => this._maxTokensPerChunk;
        set => this._maxTokensPerChunk = value > 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(this.MaxTokensPerChunk), "Value must be greater than zero.");
    }

    /// <summary>
    /// Number of tokens to copy and repeat from a chunk into the next (must be >= 0)
    /// </summary>
    public int Overlap
    {
        get => this._overlap;
        set => this._overlap = value >= 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(this.Overlap), "Value must be zero or greater.");
    }

    /// <summary>
    /// Optional header to add before each chunk.
    /// </summary>
    public string? ChunkHeader { get; set; } = null;

    /// <summary>
    /// When true, chunks are split primarily on h1, h2, h3 headers.
    /// If a section exceeds MaxTokensPerChunk, it will be split into multiple chunks,
    /// with each continuation chunk prefixed with the section's header hierarchy for context.
    /// Default is true.
    /// </summary>
    public bool PreserveHeaderContext { get; set; } = true;
}