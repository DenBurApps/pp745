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

        private SaveDataClass _currentSaveDataClass;

        public HabbitDataSaver()
        {
            LoadData();
        }

        public void SaveHabbitProgress(HabbitData.HabbitData habbitData, DateTime lastResetDate, List<DailyProgress> progressHistory)
        {
            LoadData();

            var existingHabitData = _currentSaveDataClass.Habits.Find(h => h.HabbitData.Name == habbitData.Name);
        
            if (existingHabitData != null)
            {
                existingHabitData.HabbitData = habbitData;
                existingHabitData.LastResetDate = lastResetDate;
                existingHabitData.ProgressHistory = progressHistory;
            }
            else
            {
                _currentSaveDataClass.Habits.Add(new HabbitSaveData
                {
                    HabbitData = habbitData,
                    LastResetDate = lastResetDate,
                    ProgressHistory = progressHistory
                });
            }

            SaveData();
        }

        public (DateTime lastResetDate, List<DailyProgress> progressHistory) LoadHabbitProgress(string habbitName)
        {
            LoadData();

            var habitData = _currentSaveDataClass.Habits.Find(h => h.HabbitData.Name == habbitName);
        
            if (habitData != null)
            {
                return (habitData.LastResetDate, habitData.ProgressHistory);
            }

            return (DateTime.MinValue, new List<DailyProgress>());
        }

        public List<HabbitData.HabbitData> LoadAllHabbits()
        {
            LoadData();
            return _currentSaveDataClass.Habits.ConvertAll(h => h.HabbitData);
        }

        public void DeleteHabbit(string habbitName)
        {
            LoadData();
            _currentSaveDataClass.Habits.RemoveAll(h => h.HabbitData.Name == habbitName);
            SaveData();
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    string json = File.ReadAllText(SavePath);
                    _currentSaveDataClass = JsonConvert.DeserializeObject<SaveDataClass>(json);
                }
                _currentSaveDataClass ??= new SaveDataClass();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading habit data: {e.Message}");
                _currentSaveDataClass = new SaveDataClass();
            }
        }

        private void SaveData()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_currentSaveDataClass, Formatting.Indented);
                File.WriteAllText(SavePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving habit data: {e.Message}");
            }
        }
    }
}