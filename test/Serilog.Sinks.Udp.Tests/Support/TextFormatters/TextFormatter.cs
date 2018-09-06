using Serilog.Formatting.Display;

namespace Serilog.Support.TextFormatters
{
    public class TextFormatter : MessageTemplateTextFormatter
    {
        public TextFormatter()
            : base("{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message} - {Exception}", null)
        {
        }
    }
}
