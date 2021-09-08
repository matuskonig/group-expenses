using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Frontend.Enums;
using Frontend.Helpers;

namespace Frontend.Services
{
    /// <summary>
    /// Service used to show messages (e.g. error messages from API calls)
    /// </summary>
    public class AlertMessageService
    {
        private static readonly TimeSpan MessageDuration = TimeSpan.FromSeconds(5);

        private readonly List<Alert> _messages = new();

        /// <summary>
        /// List of messages
        /// Modifying the list is internal implementation and should not be exposed directly
        /// </summary>
        public IReadOnlyList<Alert> Messages => _messages;

        public event Action OnChange;

        /// <summary>
        /// Show the message for defined amount of time
        /// </summary>
        /// <param name="message">Message to be shown</param>
        /// <param name="state">state of the message, adjusting the UI of the message</param>
        private async Task ShowMessage(string message, AlertState state)
        {
            var alertMessage = new Alert { Message = message, State = state };

            AddMessage(alertMessage);
            await Task.Delay(MessageDuration);
            RemoveMessage(alertMessage);
        }

        /// <summary>
        /// Shows the message as error message
        /// </summary>
        /// <param name="message">message to show</param>
        public async Task ShowErrorMessage(string message)
        {
            await ShowMessage(message, AlertState.Danger);
        }

        /// <summary>
        /// Adds the message into the list and notify subscribers about the change
        /// </summary>
        /// <param name="alert"></param>
        private void AddMessage(Alert alert)
        {
            _messages.Add(alert);
            OnChange?.Invoke();
        }

        /// <summary>
        /// Removes the message from the list and notify subscribers
        /// </summary>
        /// <param name="alert"></param>
        public void RemoveMessage(Alert alert)
        {
            _messages.Remove(alert);
            OnChange?.Invoke();
        }
    }
}