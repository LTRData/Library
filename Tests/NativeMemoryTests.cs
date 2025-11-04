using LTRData.Extensions.Buffers;
using LTRData.Extensions.Native.Memory;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LTRData.Extensions.Tests;

public class NativeMemoryTests
{
    [Fact]
    public void CreateAndDispose()
    {
        var memBlock = new NativeMemory<byte>(200, 300);

        var span = memBlock.Span;

        Assert.Equal(300, span.Length);

        var memManager = memBlock.GetMemoryManager();

        var mem = memManager.Memory;

        Assert.Equal(300, mem.Length);

        ((IDisposable)memManager).Dispose();

        Assert.Throws<ObjectDisposedException>(() => _ = memManager.Memory);
    }

    [Fact]
    public unsafe void Slicing()
    {
        var buffer = new byte[300];

        fixed (byte* ptr = buffer)
        {
            var memBlock = new NativeMemory<byte>((nint)ptr, buffer.Length);

            memBlock.Span[50] = 42;

            var subBlock = memBlock.Slice(50, 150);

            Assert.Equal(42, subBlock.Span[0]);

            Assert.Equal(150, subBlock.Span.Length);

            Assert.Throws<ArgumentOutOfRangeException>(() => _ = memBlock.Slice(-1));

            Assert.Throws<ArgumentOutOfRangeException>(() => _ = memBlock.Slice(50, 1000));

            Assert.Throws<ArgumentOutOfRangeException>(() => _ = memBlock.Slice(50, -1));
        }
    }

    [Fact]
    public unsafe void SafeBufferMemoryManager()
    {
        var mapping = MemoryMappedFile.CreateNew(null,
                                                 400,
                                                 MemoryMappedFileAccess.ReadWrite,
                                                 MemoryMappedFileOptions.None,
                                                 HandleInheritability.None);

        var accessor = mapping.CreateViewAccessor(0, 400, MemoryMappedFileAccess.ReadWrite);

        var memoryManager = accessor.SafeMemoryMappedViewHandle.GetMemoryManager(ownsBuffer: false);

        var memBlock = memoryManager.Memory;

        try
        {
            memBlock.Span[200] = 21;

            Assert.Equal(21, accessor.ReadByte(200));
        }
        finally
        {
            accessor.Dispose();
            mapping.Dispose();
        }

        Assert.Throws<ObjectDisposedException>(() => _ = memBlock.Span[200]);
    }
}
