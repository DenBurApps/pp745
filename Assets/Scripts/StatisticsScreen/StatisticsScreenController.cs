using System;
using System.Collections.Generic;
using System.Linq;
using MainScreen.EmojiSelector;
using StatisticsScreen.StatisticsScreen;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace StatisticsScreen
{
    public class StatisticsScreenController : MonoBehaviour
    {
        private const string EmojiPrefsKey = "CalendarEmoji_";

        [SerializeField] private StatisticsElementHolder[] _statisticsElementHolders;
        [SerializeField] private DailyPlane[] _dailyPlanes;
        [SerializeField] private StatisticsTypeHolder[] _statisticsTypeHolders;
        [SerializeField] private DailyEmojiSelector _dailyEmojiSelector;
        [SerializeField] private LineRenderer _lineRendererPrefab;
        [SerializeField] private Transform _lineContainer;
        [SerializeField] private Button _previousWeekButton;
        [SerializeField] private Button _nextWeekButton;
        [SerializeField] private TMP_Text _weekText;
        [SerializeField] private UILineConnector _lineConnector;

        private readonly List<LineRenderer> _activeLines = new();
        private readonly HashSet<EmojiType> _selectedTypes = new();
        private DailyPlane _currentSelectedPlane;
        private DateTime[] _weekDates;
        private int _currentWeekOffset;
        private ScreenVisabilityHandler _screenVisabilityHandler;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            _dailyEmojiSelector.EmojiSelected += OnEmojiSelected;
            _previousWeekButton.onClick.AddListener(OnPreviousWeekClicked);
            _nextWeekButton.onClick.AddListener(OnNextWeekClicked);
            EmojiUpdateManager.OnEmojiUpdated += OnEmojiUpdatedFromCalendar;
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            _dailyEmojiSelector.EmojiSelected -= OnEmojiSelected;
            _previousWeekButton.onClick.RemoveListener(OnPreviousWeekClicked);
            _nextWeekButton.onClick.RemoveListener(OnNextWeekClicked);
            EmojiUpdateManager.OnEmojiUpdated -= OnEmojiUpdatedFromCalendar;
            UnsubscribeFromEvents();
        }

        private void Start()
        {
            DisableScreen();
        }

        public void EnableScreen()
        {
            _screenVisabilityHandler.EnableScreen();
            _currentWeekOffset = 0;
            _selectedTypes.Clear();

            InitializeWeekDates();
            LoadSavedEmojis();
            UpdateWeekNavigationButtons();

            foreach (var holder in _statisticsTypeHolders)
            {
                holder.SetEnabled();
                _selectedTypes.Add(holder.Type);
            }

            UpdateDailyPlanesVisibility();
            UpdateStatisticsElements();
            UpdateConnectingLines();
        }

        public void DisableScreen()
        {
            _screenVisabilityHandler.DisableScreen();
        }
        
        private void OnEmojiUpdatedFromCalendar(DateTime date, EmojiType type)
        {
            for (int i = 0; i < _weekDates.Length; i++)
            {
                if (_weekDates[i].Date == date.Date)
                {
                    _dailyPlanes[i].Initialize(_weekDates[i], type);
                    UpdateDailyPlanesVisibility();
                    UpdateStatisticsElements();
                    UpdateConnectingLines();
                    break;
                }
            }
        }

        private void InitializeWeekDates()
        {
            _weekDates = new DateTime[7];
            DateTime currentDate = DateTime.Now;

            int daysUntilSunday = ((int)currentDate.DayOfWeek - (int)DayOfWeek.Sunday + 7) % 7;
            DateTime currentSunday = currentDate.AddDays(-daysUntilSunday);
            DateTime targetSunday = currentSunday.AddDays(_currentWeekOffset * -7);

            for (int i = 0; i < 7; i++)
            {
                _weekDates[i] = targetSunday.AddDays(i);
            }

            UpdateWeekText();
        }

        private void LoadSavedEmojis()
        {
            for (int i = 0; i < _weekDates.Length; i++)
            {
                string key = GetPrefsKey(_weekDates[i]);
                EmojiType type = PlayerPrefs.HasKey(key)
                    ? (EmojiType)PlayerPrefs.GetInt(key)
                    : EmojiType.None;

                _dailyPlanes[i].Initialize(_weekDates[i], type);
            }
        }

        private void SubscribeToEvents()
        {
            foreach (var holder in _statisticsTypeHolders)
            {
                holder.Clicked += OnTypeHolderClicked;
            }

            foreach (var plane in _dailyPlanes)
            {
                plane.Clicked += OnDailyPlaneClicked;
            }
        }

        private void UnsubscribeFromEvents()
        {
            foreach (var holder in _statisticsTypeHolders)
            {
                holder.Clicked -= OnTypeHolderClicked;
            }

            foreach (var plane in _dailyPlanes)
            {
                plane.Clicked -= OnDailyPlaneClicked;
            }
        }

        private void OnTypeHolderClicked(StatisticsTypeHolder holder)
        {
            if (holder.IsSelected)
            {
                _selectedTypes.Add(holder.Type);
            }
            else
            {
                _selectedTypes.Remove(holder.Type);
            }

            UpdateDailyPlanesVisibility();
            UpdateStatisticsElements();
            UpdateConnectingLines();
        }

        private void UpdateDailyPlanesVisibility()
        {
            bool hasAnyEmojis = _dailyPlanes.Any(plane => plane.EmojiType != EmojiType.None);
            bool showAllPlanes = !hasAnyEmojis || _selectedTypes.Count == 0;

            foreach (var plane in _dailyPlanes)
            {
                bool shouldShow;

                if (showAllPlanes)
                {
                    shouldShow = true;
                }
                else
                {
                    shouldShow = plane.EmojiType != EmojiType.None &&
                                 _selectedTypes.Contains(plane.EmojiType);
                }

                plane.gameObject.SetActive(shouldShow);
            }
        }

        private void UpdateStatisticsElements()
        {
            bool hasAnyEmojis = _dailyPlanes.Any(plane => plane.EmojiType != EmojiType.None);

            if (!hasAnyEmojis)
            {
                foreach (var holder in _statisticsElementHolders)
                {
                    holder.DisableAllElements();
                }

                return;
            }

            for (int i = 0; i < _statisticsElementHolders.Length; i++)
            {
                var holder = _statisticsElementHolders[i];
                var plane = _dailyPlanes[i];

                if (_selectedTypes.Count == 0)
                {
                    if (plane.EmojiType != EmojiType.None)
                    {
                        holder.EnableTypeElement(plane.EmojiType);
                    }
                    else
                    {
                        holder.DisableAllElements();
                    }
                }
                else
                {
                    if (plane.EmojiType != EmojiType.None &&
                        _selectedTypes.Contains(plane.EmojiType))
                    {
                        holder.EnableTypeElement(plane.EmojiType);
                    }
                    else
                    {
                        holder.DisableAllElements();
                    }
                }
            }
        }

        private void UpdateConnectingLines()
        {
            _lineConnector.ClearLines();
        
            bool hasAnyEmojis = _dailyPlanes.Any(plane => plane.EmojiType != EmojiType.None);
            if (!hasAnyEmojis || _selectedTypes.Count == 0) return;

            for (int i = 0; i < _statisticsElementHolders.Length - 1; i++)
            {
                var currentHolder = _statisticsElementHolders[i];
                var nextHolder = _statisticsElementHolders[i + 1];
            
                var currentElement = currentHolder.GetActiveElement();
                var nextElement = nextHolder.GetActiveElement();
            
                if (currentElement != null && nextElement != null && 
                    _selectedTypes.Contains(currentElement.Type) && 
                    _selectedTypes.Contains(nextElement.Type))
                {
                    Vector2 startPoint = currentElement.ConnectorPosition.position;
                    Vector2 endPoint = nextElement.ConnectorPosition.position;
                
                    _lineConnector.DrawLine(startPoint, endPoint, currentElement.Type, nextElement.Type);
                }
            }
        }

        private void OnDailyPlaneClicked(DailyPlane plane)
        {
            _currentSelectedPlane = plane;
            _dailyEmojiSelector.gameObject.SetActive(true);
        }

        private void OnEmojiSelected(EmojiType type)
        {
            if (_currentSelectedPlane == null) return;

            int planeIndex = Array.IndexOf(_dailyPlanes, _currentSelectedPlane);
            if (planeIndex != -1)
            {
                _currentSelectedPlane.Initialize(_weekDates[planeIndex], type);
                SaveEmojiType(_weekDates[planeIndex], type);
                
                EmojiUpdateManager.NotifyEmojiUpdated(_weekDates[planeIndex], type);

                UpdateDailyPlanesVisibility();
                UpdateStatisticsElements();
                UpdateConnectingLines();
            }

            _currentSelectedPlane = null;
        }
        private void SaveEmojiType(DateTime date, EmojiType type)
        {
            string key = GetPrefsKey(date);
            PlayerPrefs.SetInt(key, (int)type);
            PlayerPrefs.Save();
        }

        private string GetPrefsKey(DateTime date)
        {
            return $"{EmojiPrefsKey}{date:yyyy_MM_dd}";
        }

        private void OnPreviousWeekClicked()
        {
            _currentWeekOffset++;
            UpdateScreen();
        }

        private void OnNextWeekClicked()
        {
            if (_currentWeekOffset > 0)
            {
                _currentWeekOffset--;
                UpdateScreen();
            }
        }

        private void UpdateScreen()
        {
            InitializeWeekDates();
            LoadSavedEmojis();
            UpdateWeekNavigationButtons();
            UpdateDailyPlanesVisibility();
            UpdateStatisticsElements();
            UpdateConnectingLines();
        }

        private void UpdateWeekNavigationButtons()
        {
            _nextWeekButton.interactable = _currentWeekOffset > 0;
            _previousWeekButton.interactable = true;
        }

        private void UpdateWeekText()
        {
            string weekText = _currentWeekOffset switch
            {
                0 => "current week",
                1 => "1 week ago",
                _ => $"{_currentWeekOffset} weeks ago"
            };

            _weekText.text = weekText;
        }
    }
}