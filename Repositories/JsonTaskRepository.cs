using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TodoApp.Interfaces;
using TodoApp.Models;

namespace TodoApp.Repositories
{
    public class JsonTaskRepository : ITaskRepository
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private readonly string _filePath;

        public JsonTaskRepository(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Путь к файлу указан", nameof(filePath));
            }

            _filePath = filePath;
        }

        public string FilePath => _filePath;

        public IEnumerable<TodoItem> Load()
        {
            if (!File.Exists(_filePath))
            {
                return Array.Empty<TodoItem>();
            }

            try
            {
                var json = File.ReadAllText(_filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return Array.Empty<TodoItem>();
                }

                return JsonSerializer.Deserialize<List<TodoItem>>(json, Options) ?? new List<TodoItem>();
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("Что-то не так...", ex);
            }
        }

        public void Save(IEnumerable<TodoItem> tasks)
        {
            if (tasks is null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            var json = JsonSerializer.Serialize(tasks, Options);

            var fullPath = Path.GetFullPath(_filePath);
            var dir = Path.GetDirectoryName(fullPath);

            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
                

            File.WriteAllText(fullPath, json);
        }
    }
}