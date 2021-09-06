using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Frontend.Enums;
using Frontend.Helpers;

namespace Frontend.Services
{
    public class AlertMessageService
    {
        private static readonly TimeSpan MessageDuration = TimeSpan.FromSeconds(5);
        public List<Alert> Messages { get; } = new();

        public event Action OnChange;

        public async Task ShowMessage(string message, AlertState state)
        {
            var alertMessage = new Alert { Message = message, State = state };
            Messages.Add(alertMessage);
            OnChange?.Invoke();
            await Task.Delay(MessageDuration);
            Messages.Remove(alertMessage);
            OnChange?.Invoke();
        }

        public async Task ShowErrorMessage(string message)
        {
            await ShowMessage(message, AlertState.Danger);
        }

        public void RemoveMessage(Alert alert)
        {
            Messages.Remove(alert);
            OnChange?.Invoke();
        }
    }
}