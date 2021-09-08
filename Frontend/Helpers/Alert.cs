using Frontend.Enums;

namespace Frontend.Helpers
{
    /// <summary>
    /// Record representing message with some state
    /// State is directly affecting the UI
    /// </summary>
    public record Alert
    {
        public string Message { get; init; }
        public AlertState State { get; init; }
    }
}