using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wilgysef.StdoutHook.Cli.Tests;

public class CommandLocatorTest
{
    [Fact]
    public void LocateCommand()
    {
        var locator = new CommandLocator(FileExists);
        var pathsToCheck = new[]
        {
            Path.Combine("abc", "def"),
            Path.Combine("aaaa", "bbb", "cc"),
        };
        var extensions = new[]
        {
            ".exe",
            ".sh",
            ".py",
        };

        var results = locator.LocateCommand("test", pathsToCheck, extensions);
        results.Count.ShouldBe(2);
        results[0].ShouldBe(Path.Combine("abc", "def", "test.exe"));
        results[1].ShouldBe(Path.Combine("abc", "def", "test.py"));

        static bool FileExists(string path)
        {
            return path.StartsWith("abc")
                && (path.EndsWith(".exe") || path.EndsWith(".py"));
        }
    }

    [Fact]
    public void LocateCommand_NoExtension()
    {
        var locator = new CommandLocator(FileExists);
        var pathsToCheck = new[]
        {
            Path.Combine("abc", "def"),
            Path.Combine("aaaa", "bbb", "cc"),
        };

        var results = locator.LocateCommand("test", pathsToCheck, null);
        results.Count.ShouldBe(1);
        results[0].ShouldBe(Path.Combine("abc", "def", "test"));

        static bool FileExists(string path)
        {
            return path.StartsWith("abc");
        }
    }
}
