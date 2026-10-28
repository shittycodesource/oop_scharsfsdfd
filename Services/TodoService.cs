using System;
using System.Collections.ObjectModel;
using System.Linq;
using TodoApp.Models;
using TodoApp.Interfaces;

namespace TodoApp.Services
{
    public class TodoService
    {
        private readonly ITaskRepository _repository;
        private readonly ObservableCollection<TodoItem> _tasks = new();

        public ReadOnlyObservableCollection<TodoItem> Tasks { get; }

        public TodoService(ITaskRepository repository)
        {
            if (repository is null) 
            {
                throw new ArgumentNullException(nameof(repository));
            }
            
            _repository = repository;
            Tasks = new ReadOnlyObservableCollection<TodoItem>(_tasks);
        }

        public void Add(TodoItem item)
        {
            if (item is null) 
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (_tasks.Any(t => ReferenceEquals(t, item)))
            {
                throw new InvalidOperationException("Задача уже есть.");
            }

            _tasks.Add(item);
        }

        public void Update(TodoItem original, TodoItem updated)
        {
            if (original is null) 
            {
                throw new ArgumentNullException(nameof(original));
            }

            if (updated is null) 
            {
                throw new ArgumentNullException(nameof(updated));
            }

            var taskToUpdate = _tasks.FirstOrDefault(t => t.Id == original.Id);
            
            if (taskToUpdate == null)
            {
                throw new InvalidOperationException("Не нашел в списке");
            }

            taskToUpdate.Title = updated.Title;
            taskToUpdate.Description = updated.Description;
            taskToUpdate.Priority = updated.Priority;
            taskToUpdate.Status = updated.Status;
            taskToUpdate.DueToDate = updated.DueToDate;
        }

        public void Delete(TodoItem item)
        {
            if (item is null) 
            {
                throw new ArgumentNullException(nameof(item));
            }

            var taskToRemove = _tasks.FirstOrDefault(t => t.Id == item.Id);
            
            if (taskToRemove == null)
            {
                throw new InvalidOperationException("Не нашел в списке");
            }

            _tasks.Remove(taskToRemove);
        }

        public void ChangeStatus(TodoItem item, TaskStatus status)
        {
            if (item is null) 
            {
                throw new ArgumentNullException(nameof(item));
            }

            var task = _tasks.FirstOrDefault(t => t.Id == item.Id);
            
            if (task == null)
            {
                throw new InvalidOperationException("Задачи нет в списке???");
            }

            task.Status = status;
        }

        public void SaveToRepository() => _repository.Save(_tasks);
        
        public void LoadFromRepository()
        {
            _tasks.Clear();
            
            foreach (var task in _repository.Load())
            {
                _tasks.Add(task);
            }
        }
    }
}