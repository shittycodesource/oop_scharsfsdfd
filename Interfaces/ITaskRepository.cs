using System.Collections.Generic;
using TodoApp.Models;

namespace TodoApp.Interfaces
{
    public interface ITaskRepository
    {
        IEnumerable<TodoItem> Load();

        void Save(IEnumerable<TodoItem> tasks);
    }
}