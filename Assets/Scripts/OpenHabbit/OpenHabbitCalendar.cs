using System;
using System.Linq;
using MainScreen.HabbitPlane;
using TMPro;
using UnityEngine;

namespace OpenHabbit
{
    public class OpenHabbitCalendar : MonoBehaviour
    {
        [SerializeField] private OpenHabbitCalendarElement[] _elements;
        [SerializeField] private TMP_Text _periodText;
        [SerializeField] private IconSpriteProvider _iconSpriteProvider;
        
        public HabbitPlane CurrentHabbitPlane { get; private set; }

        private DateTime _currentWeekStart;

        public void Initialize(HabbitPlane habbitPlane)
        {
            CurrentHabbitPlane = habbitPlane;
            _currentWeekStart = GetStartOfWeek(DateTime.Now);
            UpdateCalendar();
        }
        
        public void UpdateCurrentDay()
        {
            if (CurrentHabbitPlane == null) return;

            var progressHistory = CurrentHabbitPlane.GetProgressHistory();
            var currentDayElement = _elements.FirstOrDefault(e => e.CurrentDate.Date == DateTime.Now.Date);

            if (currentDayElement != null)
            {
                float progress = 0f;
                var todayProgress = progressHistory?.FirstOrDefault(p => p.Date.Date == DateTime.Now.Date);
                if (todayProgress != null)
                {
                    progress = (float)todayProgress.Progress / todayProgress.TargetNumber;
                }

                currentDayElement.UpdateProgress(progress);
            }
        }

        private void UpdateCalendar()
        {
            if (CurrentHabbitPlane == null) return;

            var progressHistory = CurrentHabbitPlane.GetProgressHistory();
            var icon = _iconSpriteProvider.GetSpriteByType(CurrentHabbitPlane.HabbitData.IconType);
            
            _periodText.text = $"{_currentWeekStart:dd.MM} - {_currentWeekStart.AddDays(6):dd.MM}";

            for (int i = 0; i < _elements.Length; i++)
            {
                var currentDate = _currentWeekStart.AddDays(i);
                float progress = 0f;

                if (progressHistory != null)
                {
                    var dayProgress = progressHistory.FirstOrDefault(p => p.Date.Date == currentDate.Date);
                    if (dayProgress != null)
                    {
                        progress = (float)dayProgress.Progress / dayProgress.TargetNumber;
                    }
                }

                _elements[i].Initialize(currentDate, icon, progress);
            }
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }
    }
}