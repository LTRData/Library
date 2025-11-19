using LTRData.Data;
using Xunit;

namespace LTRData.Extensions.Tests;

public class Calculations
{
    [Fact]
    public void Luhn()
    {
        var personnr = "780517561";
        var luhn = StringData.AppendLuhn(personnr, PG: false);
        Assert.Equal("7805175614", luhn);

        var result = StringData.ValidSwedishPersonalIdNumber(luhn);
        Assert.Equal(luhn, result);
    }

}
