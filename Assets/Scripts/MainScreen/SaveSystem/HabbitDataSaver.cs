using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace MainScreen.SaveSystem
{
    public class HabbitDataSaver
    {
        private const string SaveFileName = "habits_data.json";
        private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);
        private List<HabbitSaveData> _localHabits;

        [Serializable]
        private class SaveDataClass
        {
            public List<HabbitSaveData> Habits { get; set; } = new();
        }

        [Serializable]
        private class HabbitSaveData
        {
            public HabbitData.HabbitData HabbitData { get; set; }
            public DateTime LastResetDate { get; set; }
            public List<DailyProgress> ProgressHistory { get; set; } = new();
        }

        [Serializable]
        public class DailyProgress
        {
            public DateTime Date { get; set; }
            public int Progress { get; set; }
            public int TargetNumber { get; set; }
        }

        public HabbitDataSaver()
        {
            Debug.Log($"Initializing HabbitDataSaver");
            Debug.Log($"Application.persistentDataPath: {Application.persistentDataPath}");
            Debug.Log($"Full save path: {SavePath}");
    
            // Проверим права на запись
            try
            {
                string directory = Path.GetDirectoryName(SavePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Debug.Log($"Created directory: {directory}");
                }

                // Создадим тестовый файл
                File.WriteAllText(SavePath + ".test", "test");
                File.Delete(SavePath + ".test");
                Debug.Log("Write test successful");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error checking write permissions: {e.Message}");
            }

            LoadData();
        }

        public void SaveHabbitProgress(HabbitData.HabbitData habbitData, DateTime lastResetDate,
            List<DailyProgress> progressHistory)
        {
            var existingHabit = _localHabits.Find(h => h.HabbitData.Id == habbitData.Id);

            if (existingHabit != null)
            {
                existingHabit.HabbitData = habbitData;
                existingHabit.LastResetDate = lastResetDate;
                existingHabit.ProgressHistory = progressHistory;
            }
            else
            {
                _localHabits.Add(new HabbitSaveData
                {
                    HabbitData = habbitData,
                    LastResetDate = lastResetDate,
                    ProgressHistory = progressHistory
                });
            }

            SaveData();
        }

        public (DateTime lastResetDate, List<DailyProgress> progressHistory) LoadHabbitProgress(string habbitId)
        {
            var habitData = _localHabits.Find(h => h.HabbitData.Id == habbitId);

            if (habitData != null)
            {
                return (habitData.LastResetDate, habitData.ProgressHistory);
            }

            return (DateTime.MinValue, new List<DailyProgress>());
        }

        public List<HabbitData.HabbitData> LoadAllHabbits()
        {
            return _localHabits.ConvertAll(h => h.HabbitData);
        }

        public void DeleteHabbit(HabbitData.HabbitData habbitToDelete)
        {
            Debug.Log($"=== Starting Delete Operation ===");
            Debug.Log($"Trying to delete habit with ID: {habbitToDelete.Id}");
            Debug.Log($"Habit to delete: Name={habbitToDelete.Name}, ID={habbitToDelete.Id}");
            Debug.Log($"Total habits before deletion: {_localHabits.Count}");

            Debug.Log("Current habits before deletion:");
            foreach (var habit in _localHabits)
            {
                Debug.Log($"- Habit: Name={habit.HabbitData.Name}, ID={habit.HabbitData.Id}");
            }

            int removedCount = _localHabits.RemoveAll(h => h.HabbitData.Id == habbitToDelete.Id);

            Debug.Log($"Removed {removedCount} habits");
            Debug.Log($"Total habits after deletion: {_localHabits.Count}");

            // Выведем все привычки после удаления
            Debug.Log("Current habits after deletion:");
            foreach (var habit in _localHabits)
            {
                Debug.Log($"- Habit: Name={habit.HabbitData.Name}, ID={habit.HabbitData.Id}");
            }

            SaveData();

            try
            {
                string json = File.ReadAllText(SavePath);
                Debug.Log($"Saved JSON content: {json}");

                // Попробуем сразу загрузить и проверить
                var savedData = JsonConvert.DeserializeObject<SaveDataClass>(json);
                Debug.Log($"Habits in saved file: {savedData.Habits.Count}");
                foreach (var habit in savedData.Habits)
                {
                    Debug.Log($"- Saved habit: Name={habit.HabbitData.Name}, ID={habit.HabbitData.Id}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error checking saved data: {e.Message}");
            }

            Debug.Log("=== Delete Operation Completed ===");
        }

        private void LoadData()
        {
            Debug.Log("=== Loading Data ===");
            try
            {
                if (File.Exists(SavePath))
                {
                    string json = File.ReadAllText(SavePath);
                    Debug.Log($"Loaded JSON content: {json}");

                    var saveData = JsonConvert.DeserializeObject<SaveDataClass>(json);
                    _localHabits = saveData.Habits;

                    Debug.Log($"Loaded {_localHabits.Count} habits:");
                    foreach (var habit in _localHabits)
                    {
                        Debug.Log($"- Loaded habit: Name={habit.HabbitData.Name}, ID={habit.HabbitData.Id}");
                    }
                }
                else
                {
                    Debug.Log("Save file does not exist, creating new empty list");
                    _localHabits = new List<HabbitSaveData>();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading habit data: {e.Message}");
                Debug.LogError($"Stack trace: {e.StackTrace}");
                _localHabits = new List<HabbitSaveData>();
            }

            Debug.Log("=== Loading Data Completed ===");
        }

        private void SaveData()
        {
            Debug.Log("=== Saving Data ===");
            try
            {
                var saveData = new SaveDataClass { Habits = _localHabits };
                string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                Debug.Log($"Trying to save JSON: {json}");
                Debug.Log($"Saving to path: {SavePath}");

                // Убедимся, что директория существует
                string directory = Path.GetDirectoryName(SavePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Попробуем открыть файл для записи чтобы проверить права
                using (FileStream fs = File.Open(SavePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(json);
                        writer.Flush();
                    }
                }

                // Проверим, что файл действительно обновился
                string savedContent = File.ReadAllText(SavePath);
                Debug.Log($"Verification - Content saved to file: {savedContent}");

                if (savedContent != json)
                {
                    Debug.LogError("Save verification failed - content mismatch!");
                }

                Debug.Log("Save completed successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving habit data: {e.Message}");
                Debug.LogError($"Stack trace: {e.StackTrace}");

                // Дополнительная информация о файле
                try
                {
                    if (File.Exists(SavePath))
                    {
                        var fileInfo = new FileInfo(SavePath);
                        Debug.Log($"File exists. Size: {fileInfo.Length}, Last write: {fileInfo.LastWriteTime}");
                        Debug.Log($"File attributes: {fileInfo.Attributes}");
                    }
                    else
                    {
                        Debug.Log("File does not exist");
                    }
                }
                catch (Exception fileEx)
                {
                    Debug.LogError($"Error checking file: {fileEx.Message}");
                }
            }

            Debug.Log("=== Saving Data Completed ===");
        }
    }
}