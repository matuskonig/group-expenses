using Frontend.Enums;

namespace Frontend.Helpers
{
    public record Alert
    {
        public string Message { get; init; }
        public AlertState State { get; init; }
    }
}