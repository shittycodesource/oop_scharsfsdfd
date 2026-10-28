using System;
using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly TodoService _service;

        public MainViewModel(TodoService service)
        {
            if (service is null) 
            {
                throw new ArgumentNullException(nameof(service));
            }
            
            _service = service;
        }

        public ObservableCollection<TodoItemViewModel> Tasks { get; } = new();
        public int TasksCount => Tasks.Count;

        private bool _isUpdatingFromCode = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UpdateCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
        private TodoItemViewModel? _selectedTask;

        partial void OnSelectedTaskChanged(TodoItemViewModel? value)
        {
            if (_isUpdatingFromCode) 
            {
                return;
            }

            if (value is null)
            {
                return;
            }

            Title = value.Title;
            Description = value.Description;
            Priority = value.Priority;
            Status = value.Status;
            DueToDate = value.DueToDate;
        }

        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private string? _description;
        [ObservableProperty] private TaskPriority _priority = TaskPriority.Medium;
        [ObservableProperty] private TaskStatus _status = TaskStatus.NotStarted;
        [ObservableProperty] private DateTimeOffset? _dueToDate = DateTimeOffset.Now.Date.AddDays(1);

        [ObservableProperty] private string? _errorMessage;
        [ObservableProperty] private string? _infoMessage;

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
        public bool HasInfo => !string.IsNullOrEmpty(InfoMessage);

        partial void OnErrorMessageChanged(string? value) 
        { 
            OnPropertyChanged(nameof(HasError)); 
        }
        
        partial void OnInfoMessageChanged(string? value) 
        { 
            OnPropertyChanged(nameof(HasInfo)); 
        }

        private void SetError(string message) 
        { 
            InfoMessage = null; 
            ErrorMessage = message; 
        }
        
        private void SetInfo(string message) 
        { 
            ErrorMessage = null; 
            InfoMessage = message; 
        }
        
        private void ClearMessages() 
        { 
            ErrorMessage = null; 
            InfoMessage = null; 
        }

        public Array Priorities => Enum.GetValues(typeof(TaskPriority));
        public Array Statuses => Enum.GetValues(typeof(TaskStatus));

        [RelayCommand]
        private void Add()
        {
            ClearMessages();
            try
            {
                var model = BuildModelFromForm(DateOnly.FromDateTime(DateTime.Today));
                _service.Add(model);
                
                _isUpdatingFromCode = true;
                Tasks.Add(new TodoItemViewModel(model));
                ClearForm();
                _isUpdatingFromCode = false;

                SetInfo("Задача добавлена.");
            }
            catch (Exception ex) 
            { 
                SetError(ex.Message); 
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditSelected))]
        private void Update()
        {
            ClearMessages();
            
            if (SelectedTask is null) 
            {
                return;
            }
            
            try
            {
                var original = SelectedTask.GetTodoItem();
                var updated = BuildModelFromForm(original.CreatedAtDate);
                updated.Id = original.Id; 

                _service.Update(original, updated);
                SelectedTask.LoadFrom(updated);
                RefreshSelectedRow();
                
                SetInfo("Изменения сохранены");
            }
            catch (Exception ex) 
            { 
                SetError(ex.Message); 
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditSelected))]
        private void Delete()
        {
            ClearMessages();
            
            if (SelectedTask is null) 
            {
                return;
            }
            
            try
            {
                var original = SelectedTask.GetTodoItem();
                _service.Delete(original);
                
                _isUpdatingFromCode = true;
                Tasks.Remove(SelectedTask);
                ClearForm();
                _isUpdatingFromCode = false;
                
                SetInfo("Задача удалена");
            }
            catch (Exception ex) 
            { 
                SetError(ex.Message); 
            }
        }

        [RelayCommand]
        private void Save()
        {
            ClearMessages();
            try
            {
                _service.SaveToRepository();
                SetInfo("Задачи сохранены в файл");
            }
            catch (Exception ex) 
            { 
                SetError($"Не удалось сохранить {ex.Message}"); 
            }
        }

        [RelayCommand]
        private void Load()
        {
            ClearMessages();
            try
            {
                _service.LoadFromRepository();
                
                _isUpdatingFromCode = true;
                Tasks.Clear();
                
                foreach (var model in _service.Tasks)
                {
                    Tasks.Add(new TodoItemViewModel(model));
                }
                
                ClearForm();
                _isUpdatingFromCode = false;
                
                OnPropertyChanged(nameof(TasksCount));
                SetInfo($"Загружено задач: {Tasks.Count}.");
            }
            catch (Exception ex) 
            { 
                SetError($"Не удалось загрузить: {ex.Message}"); 
            }
        }

        private bool CanEditSelected() 
        {
            if (SelectedTask is not null) 
            {
                return true;                
            }

            return false;
        }

        private TodoItem BuildModelFromForm(DateOnly createdAt)
        {
            var item = new TodoItem
            {
                Title = Title,
                Description = Description,
                Priority = Priority,
                Status = Status
            };
            
            item.CreatedAtDate = createdAt;
            
            if (DueToDate.HasValue)
            {
                item.DueToDate = DateOnly.FromDateTime(DueToDate.Value.DateTime);
            }
            else 
            {
                item.DueToDate = createdAt.AddDays(1);
            }

            return item;
        }

        private void RefreshSelectedRow()
        {
            if (SelectedTask is null)
            {
                return;
            }

            var index = Tasks.IndexOf(SelectedTask);
            
            if (index < 0) 
            {
                return;
            }
            
            _isUpdatingFromCode = true;
            
            var updated = new TodoItemViewModel(SelectedTask.GetTodoItem());
            Tasks[index] = updated;
            SelectedTask = updated;

            _isUpdatingFromCode = false;
        }

        private void ClearForm()
        {
            _isUpdatingFromCode = true;
            SelectedTask = null;
            _isUpdatingFromCode = false;

            Title = string.Empty;
            Description = null;
            Priority = TaskPriority.Medium;
            Status = TaskStatus.NotStarted;
            DueToDate = DateTimeOffset.Now.Date.AddDays(1);
        }
    }
}