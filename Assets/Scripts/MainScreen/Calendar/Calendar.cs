using System;
using MainScreen.EmojiSelector;
using TMPro;
using UnityEngine;

namespace MainScreen.Calendar
{
    public class Calendar : MonoBehaviour
    {
        private const string EmojiPrefsKey = "CalendarEmoji_";
        
        [SerializeField] private CalendarElement[] _elements; //Total of 7 elements
        [SerializeField] private TMP_Text _periodText;
        [SerializeField] private DailyEmojiSelector _dailyEmojiSelector;

        private CalendarElement _currentSelectedElement;
        private DateTime[] _dates;

        private void Start()
        {
            InitializeDates();
            InitializeElements();
            SetCurrentPeriod();
            LoadSavedEmojis();
        }

        private void OnEnable()
        {
            _dailyEmojiSelector.EmojiSelected += AssignType;
            EmojiUpdateManager.OnEmojiUpdated += OnEmojiUpdatedFromStatistics;

            foreach (var calendarElement in _elements)
            {
                calendarElement.ElementClicked += OnElementClicked;
            }
        }

        private void OnDisable()
        {
            _dailyEmojiSelector.EmojiSelected -= AssignType;
            EmojiUpdateManager.OnEmojiUpdated -= OnEmojiUpdatedFromStatistics;

            foreach (var calendarElement in _elements)
            {
                calendarElement.ElementClicked -= OnElementClicked;
            }
        }
        
        private void OnEmojiUpdatedFromStatistics(DateTime date, EmojiType type)
        {
            for (int i = 0; i < _dates.Length; i++)
            {
                if (_dates[i].Date == date.Date)
                {
                    _elements[i].AssignEmojiType(type);
                    break;
                }
            }
        }

        private void InitializeDates()
        {
            _dates = new DateTime[7];
            DateTime currentDate = DateTime.Now;
    
            int diff = (int)currentDate.DayOfWeek - (int)DayOfWeek.Sunday;
            if (diff < 0)
                diff += 7;
        
            DateTime sunday = currentDate.AddDays(-diff);
    
            for (int i = 0; i < 7; i++)
            {
                _dates[i] = sunday.AddDays(i);
            }
        }

        private void InitializeElements()
        {
            for (int i = 0; i < _elements.Length; i++)
            {
                _elements[i].Initialize(_dates[i]);
            }
        }

        private void SetCurrentPeriod()
        {
            int startMonth = _dates[0].Month;
            int endMonth = _dates[6].Month;

            if (startMonth == endMonth)
            {
                _periodText.text = _dates[0].ToString("MMMM");
            }
            else
            {
                string startMonthName = _dates[0].ToString("MMMM");
                string endMonthName = _dates[6].ToString("MMMM");
                _periodText.text = $"{startMonthName}-{endMonthName}";
            }
        }

        private void LoadSavedEmojis()
        {
            for (int i = 0; i < _dates.Length; i++)
            {
                string key = GetPrefsKey(_dates[i]);
                if (PlayerPrefs.HasKey(key))
                {
                    EmojiType savedType = (EmojiType)PlayerPrefs.GetInt(key);
                    _elements[i].AssignEmojiType(savedType);
                }
                else
                {
                    _elements[i].AssignEmojiType(EmojiType.None);
                }
            }
        }

        private void OnElementClicked(CalendarElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            _currentSelectedElement = element;
            _dailyEmojiSelector.gameObject.SetActive(true);
        }

        private void AssignType(EmojiType type)
        {
            if (_currentSelectedElement != null)
            {
                _currentSelectedElement.AssignEmojiType(type);
                SaveEmojiType(_currentSelectedElement, type);
                
                int elementIndex = Array.IndexOf(_elements, _currentSelectedElement);
                if (elementIndex != -1)
                {
                    EmojiUpdateManager.NotifyEmojiUpdated(_dates[elementIndex], type);
                }
            }
        }

        private void SaveEmojiType(CalendarElement element, EmojiType type)
        {
            int elementIndex = Array.IndexOf(_elements, element);
            if (elementIndex != -1)
            {
                string key = GetPrefsKey(_dates[elementIndex]);
                PlayerPrefs.SetInt(key, (int)type);
                PlayerPrefs.Save();
            }
        }

        private string GetPrefsKey(DateTime date)
        {
            return $"{EmojiPrefsKey}{date:yyyy_MM_dd}";
        }
    }
}