using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace TodoApp.Models
{
    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        Completed
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }

    public class TodoItem : INotifyPropertyChanged
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        private string _title = string.Empty;
        private string? _description;
        private TaskPriority _priority = TaskPriority.Medium;
        private TaskStatus _status = TaskStatus.NotStarted;
        private DateOnly _createdAtDate = DateOnly.FromDateTime(DateTime.Today);
        private DateOnly _dueToDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Название задачи не может быть пустым.", nameof(Title));
                }

                _title = value.Trim();
                OnPropertyChanged();
            }
        }

        public string? Description
        {
            get => _description;
            set 
            { 
                _description = value; 
                OnPropertyChanged(); 
            }
        }

        public TaskPriority Priority
        {
            get => _priority;
            set
            {
                if (!Enum.IsDefined(typeof(TaskPriority), value))
                {
                    throw new ArgumentOutOfRangeException(nameof(Priority), "Недопустимый приоритет.");
                }

                _priority = value;
                OnPropertyChanged();
            }
        }

        public TaskStatus Status
        {
            get => _status;
            set
            {
                if (!Enum.IsDefined(typeof(TaskStatus), value))
                {
                    throw new ArgumentOutOfRangeException(nameof(Status), "Недопустимый статус.");
                }
                
                _status = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsDone));
            }
        }

        public DateOnly CreatedAtDate
        {
            get => _createdAtDate;
            set
            {
                if (value == default)
                {
                    throw new ArgumentException("Дата создания не задана.", nameof(CreatedAtDate));
                }
                
                if (value > _dueToDate)
                {
                    throw new ArgumentException("Дата создания не может быть позже срока выполнения.", nameof(CreatedAtDate));
                }
                
                _createdAtDate = value;
                OnPropertyChanged();
            }
        }

        public DateOnly DueToDate
        {
            get => _dueToDate;
            set
            {
                if (value == default)
                {
                    throw new ArgumentException("Срок выполнения не задан.", nameof(DueToDate));
                }
                
                if (value < _createdAtDate)
                {
                    throw new ArgumentException("Срок выполнения не может быть раньше даты создания.", nameof(DueToDate));
                }

                _dueToDate = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool IsDone => Status == TaskStatus.Completed;

        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}