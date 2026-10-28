using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.IO;
using TodoApp.Repositories;
using TodoApp.Services;
using TodoApp.ViewModels;
using TodoApp.Views;

namespace TodoApp
{
    public partial class App : Application
    {
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = Path.Combine(desktopPath, "tasks.json");

                var repository = new JsonTaskRepository(filePath);
                var service = new TodoService(repository);
                var vm = new MainViewModel(service);

                desktop.MainWindow = new MainWindow { DataContext = vm };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}