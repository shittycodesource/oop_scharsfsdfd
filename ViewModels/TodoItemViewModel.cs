using System;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Models;

namespace TodoApp.ViewModels
{
    public partial class TodoItemViewModel : ObservableObject
    {
        public Guid Id { get; set; } 

        public TodoItemViewModel()
        {
            _createdAtDate = DateTimeOffset.Now.Date;
            _dueToDate = DateTimeOffset.Now.Date.AddDays(1);
        }

        public TodoItemViewModel(TodoItem item)
        {
            if (item is null) 
            {
                throw new ArgumentNullException(nameof(item));
            }

            Id = item.Id;
            Title = item.Title;
            Description = item.Description;
            Priority = item.Priority;
            Status = item.Status;
            
            CreatedAtDate = new DateTimeOffset(item.CreatedAtDate.ToDateTime(TimeOnly.MinValue));
            DueToDate = new DateTimeOffset(item.DueToDate.ToDateTime(TimeOnly.MinValue));
        }

        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private string? _description;
        [ObservableProperty] private TaskPriority _priority = TaskPriority.Medium;
        [ObservableProperty] private TaskStatus _status = TaskStatus.NotStarted;
        
        [ObservableProperty] private DateTimeOffset? _createdAtDate;
        [ObservableProperty] private DateTimeOffset? _dueToDate;

        public TodoItem GetTodoItem()
        {
            var item = new TodoItem
            {
                Id = this.Id,
                Title = Title,
                Description = Description,
                Priority = Priority,
                Status = Status
            };

            if (CreatedAtDate.HasValue)
            {
                item.CreatedAtDate = DateOnly.FromDateTime(CreatedAtDate.Value.DateTime);
            }
            else 
            {
                item.CreatedAtDate = DateOnly.FromDateTime(DateTime.Today);
            }

            if (DueToDate.HasValue)
            {
                item.DueToDate = DateOnly.FromDateTime(DueToDate.Value.DateTime);
            }
            else 
            {
                item.DueToDate = item.CreatedAtDate.AddDays(1);
            }

            return item;
        }

        public void LoadFrom(TodoItem item)
        {
            if (item is null) 
            {
                throw new ArgumentNullException(nameof(item));
            }

            Id = item.Id;
            Title = item.Title;
            Description = item.Description;
            Priority = item.Priority;
            Status = item.Status;
            
            CreatedAtDate = new DateTimeOffset(item.CreatedAtDate.ToDateTime(TimeOnly.MinValue));
            DueToDate = new DateTimeOffset(item.DueToDate.ToDateTime(TimeOnly.MinValue));
        }
    }
}