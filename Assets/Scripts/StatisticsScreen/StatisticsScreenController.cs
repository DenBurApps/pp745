using System;
using System.Collections.Generic;
using System.Linq;
using MainScreen.EmojiSelector;
using StatisticsScreen.StatisticsScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        
        private readonly List<LineRenderer> _activeLines = new();
        private readonly HashSet<EmojiType> _selectedTypes = new();
        private DailyPlane _currentSelectedPlane;
        private DateTime[] _weekDates;
        private int _currentWeekOffset;
        private ScreenVisabilityHandler _screenVisabilityHandler;
        private StatisticsTypeHolder _currentHolder;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            _dailyEmojiSelector.EmojiSelected += OnEmojiSelected;
            _previousWeekButton.onClick.AddListener(OnPreviousWeekClicked);
            _nextWeekButton.onClick.AddListener(OnNextWeekClicked);
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            _dailyEmojiSelector.EmojiSelected -= OnEmojiSelected;
            _previousWeekButton.onClick.RemoveListener(OnPreviousWeekClicked);
            _nextWeekButton.onClick.RemoveListener(OnNextWeekClicked);
            UnsubscribeFromEvents();
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

        private void EnableAllTypeHolders()
        {
            _selectedTypes.Clear();
            foreach (var holder in _statisticsTypeHolders)
            {
                holder.SetEnabled();
                _selectedTypes.Add(holder.Type);
            }
            
            UpdateDailyPlanesVisibility();
            UpdateStatisticsElements();
            UpdateConnectingLines();
        }
        
        private void InitializeWeekDates()
        {
            _weekDates = new DateTime[7];
            DateTime currentDate = DateTime.Now;
            
            // Adjust to start from Sunday of the selected week
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
                if (!_selectedTypes.Contains(holder.Type))
                {
                    _selectedTypes.Add(holder.Type);
                }
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
            if (_selectedTypes.Count == 0)
            {
                // Если нет выбранных типов, показываем все DailyPlanes с эмодзи
                foreach (var plane in _dailyPlanes)
                {
                    plane.gameObject.SetActive(plane.EmojiType != EmojiType.None);
                }
            }
            else
            {
                // Показываем только DailyPlanes с выбранными типами
                foreach (var plane in _dailyPlanes)
                {
                    plane.gameObject.SetActive(plane.EmojiType != EmojiType.None && 
                                               _selectedTypes.Contains(plane.EmojiType));
                }
            }
        }
        
        private void UpdateStatisticsElements()
        {
            if (_selectedTypes.Count == 0)
            {
                // Если нет выбранных типов, показываем все использованные эмодзи
                for (int i = 0; i < _statisticsElementHolders.Length; i++)
                {
                    var holder = _statisticsElementHolders[i];
                    var plane = _dailyPlanes[i];
            
                    if (plane.EmojiType != EmojiType.None)
                    {
                        holder.EnableTypeElement(plane.EmojiType);
                    }
                    else
                    {
                        holder.DisableAllElements();
                    }
                }
            }
            else
            {
                // Показываем только элементы с выбранными типами
                for (int i = 0; i < _statisticsElementHolders.Length; i++)
                {
                    var holder = _statisticsElementHolders[i];
                    var plane = _dailyPlanes[i];
            
                    if (plane.EmojiType != EmojiType.None && _selectedTypes.Contains(plane.EmojiType))
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

        private void UpdateConnectingLines()
        {
            // Clear existing lines
            foreach (var line in _activeLines)
            {
                Destroy(line.gameObject);
            }
            _activeLines.Clear();

            if (_selectedTypes.Count == 0) return;

            // Create new lines for each selected type
            foreach (var type in _selectedTypes)
            {
                var points = new List<(int index, Vector3 position)>();
        
                // Собираем точки соединения вместе с их индексами
                for (int i = 0; i < _statisticsElementHolders.Length; i++)
                {
                    var connectorPosition = _statisticsElementHolders[i].GetConnectorPositionForType(type);
                    if (connectorPosition.HasValue)
                    {
                        // Сохраняем индекс вместе с позицией
                        Vector3 localPosition = _lineContainer.InverseTransformPoint(connectorPosition.Value);
                        points.Add((i, localPosition));
                    }
                }

                if (points.Count >= 2)
                {
                    // Сортируем точки по индексу, чтобы соединить их последовательно
                    points.Sort((a, b) => a.index.CompareTo(b.index));
            
                    var lineObj = Instantiate(_lineRendererPrefab, _lineContainer);
                    var lineRenderer = lineObj.GetComponent<LineRenderer>();
            
                    // Настройка LineRenderer
                    lineRenderer.useWorldSpace = false;
                    lineRenderer.sortingOrder = 1;
            
                    // Устанавливаем позиции в правильном порядке
                    lineRenderer.positionCount = points.Count;
                    lineRenderer.SetPositions(points.Select(p => p.position).ToArray());
            
                    _activeLines.Add(lineRenderer);
                }
            }
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

// Additional script to handle Line Renderer configuration