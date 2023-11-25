using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Wilgysef.StdoutHook.Profiles;

/// <summary>
/// Profile state.
/// </summary>
/// <remarks>
/// Not thread safe, shared by the stdout and stderr stream reader handlers.
/// </remarks>
public class ProfileState : IDisposable
{
    private readonly ConcurrentDictionary<string, ConcurrentStream> _fileStreams = new();
    private readonly ColorState _colorState = new();

    private bool _trackColorState;

    /// <summary>
    /// Running process.
    /// </summary>
    public Process Process { get; private set; } = null!;

    /// <summary>
    /// Number of stdout lines passed.
    /// </summary>
    public long StdoutLineCount { get; set; }

    /// <summary>
    /// Number of stderr lines passed.
    /// </summary>
    public long StderrLineCount { get; set; }

    /// <summary>
    /// Number of lines passed.
    /// </summary>
    public long LineCount => StdoutLineCount + StderrLineCount;

    /// <summary>
    /// Stream factory.
    /// </summary>
    internal Func<string, Stream> StreamFactory { get; set; } =
        absolutePath => new FileStream(absolutePath, FileMode.Append, FileAccess.Write, FileShare.Read);

    /// <summary>
    /// Sets the running process.
    /// </summary>
    /// <param name="process">Process.</param>
    public void SetProcess(Process process)
    {
        Process = process;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        Process?.Dispose();

        foreach (var lockedStream in _fileStreams.Values)
        {
            lockedStream?.Dispose();
        }

        _fileStreams.Clear();
    }

    /// <summary>
    /// Gets or creates a file stream.
    /// </summary>
    /// <param name="absolutePath">File path.</param>
    /// <returns>File stream.</returns>
    internal ConcurrentStream GetOrCreateFileStream(string absolutePath)
    {
        ConcurrentStream? stream;
        Stream? createdStream = null;

        try
        {
            stream = _fileStreams.GetOrAdd(absolutePath, CreateStream);

            if (createdStream != null && !stream.IsStream(createdStream))
            {
                // a stream was created but the key already exists
                createdStream.Dispose();
                createdStream = null;
            }
        }
        catch
        {
            if (!_fileStreams.TryGetValue(absolutePath, out stream))
            {
                // wait a small amount in case the stream was created after another stream was created but before it was added to the dictionary
                Thread.Sleep(50);

                if (!_fileStreams.TryGetValue(absolutePath, out stream))
                {
                    throw;
                }
            }

            // exception was thrown but the stream exists, continue
            // GlobalLogger.Warn($"an exception was thrown when opening the file stream, although the file stream exists: {ex.Message}: {absolutePath}");
        }

        return stream;

        ConcurrentStream CreateStream(string key)
        {
            createdStream = StreamFactory(key);
            return new ConcurrentStream(createdStream);
        }
    }

    /// <summary>
    /// Enables tracking color state.
    /// </summary>
    internal void TrackColorState()
    {
        _trackColorState = true;
    }

    /// <summary>
    /// Applies color state.
    /// </summary>
    /// <param name="colors">Color list.</param>
    internal void ApplyColorState(ColorList colors)
    {
        if (!_trackColorState)
        {
            return;
        }

        lock (_colorState)
        {
            _colorState.UpdateState(colors, int.MaxValue);
        }
    }

    /// <summary>
    /// Gets the color state.
    /// </summary>
    /// <param name="dataState">Data state.</param>
    /// <param name="position">Start position offset.</param>
    /// <returns>Color state.</returns>
    internal ColorState GetColorState(DataState dataState, int position)
    {
        var copy = _colorState.Copy();

        copy.UpdateState(dataState.ExtractedColors, position);
        return copy;
    }
}
