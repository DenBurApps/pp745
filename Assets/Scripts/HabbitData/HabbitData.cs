using System;
using UnityEngine;

namespace HabbitData
{
    public enum FrequencyType
    {
        EveryDay,
        Once2Days,
        OnceWeek,
        None
    }

    public enum IconType
    {
        Pills,
        Glass,
        Man,
        Music,
        Card
    }
    
    [Serializable]
    public class HabbitData
    {
        public string Name;
        public int Number;
        public FrequencyType FrequencyType;
        public IconType IconType;
        public string Note;
        public int Progress;
        
        public string Id { get; private set; }

        public HabbitData(string name, int number, FrequencyType frequencyType, IconType iconType, string note)
        {
            Name = name;
            Number = number;
            FrequencyType = frequencyType;
            IconType = iconType;
            Note = note;
            Progress = 0;
            
            Id = Guid.NewGuid().ToString();
        }
    }
}
