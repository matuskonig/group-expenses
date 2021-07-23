using System;
using System.Collections.Generic;

namespace Frontend.Services
{
    public record Todo
    {
        public string Name { get; set; }
        public bool Done { get; set; }
    }

    public interface ITodoService
    {
        public List<Todo> Todos { get; set; }
        public event Action OnChangeHandlers;
        public void NotifyChange();
    }

    public class TodoService : ITodoService
    {
        private List<Todo> todos = new();

        public List<Todo> Todos
        {
            get => todos;
            set
            {
                todos = value;
                NotifyChange();
            }
        }

        public event Action OnChangeHandlers;
        public void NotifyChange() => OnChangeHandlers?.Invoke();
    }
}