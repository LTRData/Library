using System.Runtime.InteropServices;
using Xunit;

namespace LTRData.Extensions.Tests;

public class NotOnWindowsFactAttribute : FactAttribute
{
    public NotOnWindowsFactAttribute()
    {
#if NETSTANDARD || NETCOREAPP
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Skip = null;
        }
        else
        {
            Skip = "This test runs on non-Windows platforms only";
        }
#else
        Skip = "This test runs on non-Windows platforms only";
#endif
    }
}
