using System.Collections.Concurrent;
using System.Diagnostics;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Tests.ProfileTests;

public class ProfileStateTest
{
    [Fact]
    public void SetProcess()
    {
        using var state = new ProfileState();
        var currentProcess = Process.GetCurrentProcess();

        state.SetProcess(currentProcess);
        state.Process.ShouldBe(currentProcess);
    }

    [Fact]
    public void GetOrCreateFileStream()
    {
        var factoryResults = new List<Stream>();

        using var state = new ProfileState()
        {
            StreamFactory = _ =>
            {
                var stream = new MemoryStream();
                factoryResults.Add(stream);
                return stream;
            },
        };

        var result = state.GetOrCreateFileStream("test");
        result.IsStream(factoryResults[0]).ShouldBeTrue();

        result = state.GetOrCreateFileStream("test");
        result.IsStream(factoryResults[0]).ShouldBeTrue();

        result = state.GetOrCreateFileStream("asdf");
        result.IsStream(factoryResults[1]).ShouldBeTrue();

        factoryResults.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetOrCreateFileStream_DuplicateStream()
    {
        await ShouldDuplicateStreamsBeSame(3, _ => new MemoryStream(), 0);
    }

    [Fact]
    public async Task GetOrCreateFileStream_DuplicateStream_FactoryException()
    {
        await ShouldDuplicateStreamsBeSame(
            3,
            number => number == 1 ? throw new IOException() : new MemoryStream(),
            0);
    }

    [Fact]
    public void GetOrCreateFileStream_FactoryException()
    {
        var factoryResults = new List<Stream>();
        var throwFactory = false;

        using var state = new ProfileState()
        {
            StreamFactory = _ =>
            {
                if (throwFactory)
                {
                    throw new IOException();
                }

                var stream = new MemoryStream();
                factoryResults.Add(stream);
                return stream;
            },
        };

        state.GetOrCreateFileStream("test").IsStream(factoryResults[0]).ShouldBeTrue();

        throwFactory = true;
        Should.Throw<Exception>(() => state.GetOrCreateFileStream("asdf"));
    }

    private static async Task ShouldDuplicateStreamsBeSame(int streamCount, Func<int, Stream> streamFactory, int expectedExceptions)
    {
        var factoryResults = new ConcurrentDictionary<int, Stream>();
        var factoryLock = new object();

        using var factoryResetEvent = new ManualResetEventSlim(false);
        var taskCounter = 0;
        var tasksReady = 0;

        using var state = new ProfileState()
        {
            StreamFactory = _ =>
            {
                var taskNumber = 0;
                lock (factoryLock)
                {
                    taskNumber = ++taskCounter;
                    tasksReady++;
                }

                factoryResetEvent.Wait();

                var stream = streamFactory(taskNumber);
                factoryResults[taskNumber] = stream;
                return stream;
            },
        };

        var tasks = new Task<ConcurrentStream>[streamCount];
        for (var i = 0; i < streamCount; i++)
        {
            tasks[i] = Task.Run(() => state.GetOrCreateFileStream("test"));
        }

        while (tasksReady < streamCount)
        {
            await Task.Delay(10);
        }

        factoryResetEvent.Set();

        var streams = new List<ConcurrentStream>(streamCount);
        var exceptions = 0;

        for (var i = 0; i < tasks.Length; i++)
        {
            try
            {
                streams.Add(await tasks[i]);
            }
            catch
            {
                exceptions++;
            }
        }

        Stream? expectedStream = null;
        foreach (var stream in factoryResults.Values)
        {
            if (streams[0].IsStream(stream))
            {
                expectedStream = stream;
                break;
            }
        }

        for (var i = 0; i < streams.Count; i++)
        {
            streams[i].IsStream(expectedStream!).ShouldBeTrue();
        }

        exceptions.ShouldBe(expectedExceptions);
    }
}
