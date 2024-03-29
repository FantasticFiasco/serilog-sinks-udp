using System.Globalization;

namespace Serilog.Support;

public class FormatProvider : CultureInfo
{
    public FormatProvider()
        : base("en-US")
    {
    }
}