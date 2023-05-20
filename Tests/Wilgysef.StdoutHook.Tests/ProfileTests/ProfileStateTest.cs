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
        var factoryResults = new List<Stream>();
        var factoryLock = new object();
        using var factoryResetEvent = new ManualResetEventSlim(false);
        using var setupResetEvent = new AutoResetEvent(false);

        using var state = new ProfileState()
        {
            StreamFactory = _ =>
            {
                setupResetEvent.Set();
                factoryResetEvent.Wait();

                var stream = new MemoryStream();

                lock (factoryLock)
                {
                    factoryResults.Add(stream);
                }
                return stream;
            },
        };

        var streamTask1 = Task.Run(() => state.GetOrCreateFileStream("test"));
        setupResetEvent.WaitOne();

        var streamTask2 = Task.Run(() => state.GetOrCreateFileStream("test"));
        setupResetEvent.WaitOne();

        factoryResetEvent.Set();

        var stream1 = await streamTask1;
        var stream2 = await streamTask2;

        stream1.IsStream(factoryResults[0]).ShouldBeTrue();
        stream2.IsStream(factoryResults[0]).ShouldBeTrue();

        factoryResults.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetOrCreateFileStream_DuplicateStream_FactoryException()
    {
        var factoryResults = new List<Stream>();
        var factoryLock = new object();
        var throwFactory = false;
        using var factoryResetEvent = new AutoResetEvent(false);
        using var setupResetEvent = new AutoResetEvent(false);

        using var state = new ProfileState()
        {
            StreamFactory = _ =>
            {
                setupResetEvent.Set();
                factoryResetEvent.WaitOne();

                if (throwFactory)
                {
                    throw new IOException();
                }

                setupResetEvent.Set();

                var stream = new MemoryStream();

                lock (factoryLock)
                {
                    factoryResults.Add(stream);
                }
                return stream;
            },
        };

        var streamTask1 = Task.Run(() => state.GetOrCreateFileStream("test"));
        setupResetEvent.WaitOne();

        var streamTask2 = Task.Run(() => state.GetOrCreateFileStream("test"));
        setupResetEvent.WaitOne();

        factoryResetEvent.Set();
        setupResetEvent.WaitOne();

        throwFactory = true;
        factoryResetEvent.Set();

        var stream1 = await streamTask1;
        var stream2 = await streamTask2;

        stream1.IsStream(factoryResults[0]).ShouldBeTrue();
        stream2.IsStream(factoryResults[0]).ShouldBeTrue();
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
}
