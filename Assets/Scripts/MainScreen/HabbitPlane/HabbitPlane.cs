using System;
using System.Collections.Generic;
using HabbitData;
using MainScreen.SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen.HabbitPlane
{
    public class HabbitPlane : MonoBehaviour
    {
        [SerializeField] private Image _progressFilledImage;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _addProgressButton;
        [SerializeField] private Sprite _progressButtonEnableSprite;
        [SerializeField] private Sprite _fullProgressButtonSprite;
         
        private IconSpriteProvider _iconSpriteProvider;
        private DateTime _lastResetDate;
        private HabbitDataSaver _dataSaver;
        private List<HabbitDataSaver.DailyProgress> _progressHistory;

        public event Action<HabbitPlane> PlaneClicked;
        
        public HabbitData.HabbitData HabbitData { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime LastResetDate => _lastResetDate;

        private void Awake()
        {
            _progressHistory = new List<HabbitDataSaver.DailyProgress>();
        }

        private void OnEnable()
        {
            _openButton.onClick.AddListener(() => PlaneClicked?.Invoke(this));
            _addProgressButton.onClick.AddListener(IncreaseProgress);
        }

        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(() => PlaneClicked?.Invoke(this));
            _addProgressButton.onClick.RemoveListener(IncreaseProgress);
        }

        
        public void SetSpriteProvider(IconSpriteProvider iconSpriteProvider)
        {
            _iconSpriteProvider = iconSpriteProvider;
        }
        
        public void SetDataSaver(HabbitDataSaver dataSaver)
        {
            _dataSaver = dataSaver ?? throw new ArgumentNullException(nameof(dataSaver));
        }
        
        public void Enable(HabbitData.HabbitData data)
        {
            HabbitData = data ?? throw new ArgumentNullException(nameof(data));
            
            gameObject.SetActive(true);
            IsActive = true;

            _nameText.text = data.Name;
            _iconImage.sprite = _iconSpriteProvider.GetSpriteByType(data.IconType);
            
            LoadSavedProgress();
            UpdateProgressDisplay();
        }

        public void UpdateValues(HabbitData.HabbitData data)
        {
            HabbitData = data ?? throw new ArgumentNullException(nameof(data));

            _nameText.text = data.Name;
            _iconImage.sprite = _iconSpriteProvider.GetSpriteByType(data.IconType);
            
            LoadSavedProgress();
            CheckAndResetProgress();
            UpdateProgressDisplay();
        }

        public void LoadSavedProgress()
        {
            if (HabbitData == null || _dataSaver == null) return;

            var (lastResetDate, progressHistory) = _dataSaver.LoadHabbitProgress(HabbitData.Id); // используем Id
            if (lastResetDate == DateTime.MinValue) // если данных нет
            {
                _lastResetDate = DateTime.Now.Date;
                _progressHistory = new List<HabbitDataSaver.DailyProgress>();
                return;
            }
    
            _lastResetDate = lastResetDate;
            _progressHistory = progressHistory ?? new List<HabbitDataSaver.DailyProgress>();
        }

        public void Disable()
        {
            if (HabbitData != null)
            {
                _dataSaver.DeleteHabbit(HabbitData);
            }
            
            HabbitData = null;
            IsActive = false;
            _progressHistory.Clear();
            gameObject.SetActive(false);
        }

        private void SaveCurrentProgress()
        {
            if (!IsActive || HabbitData == null || _dataSaver == null) return;

            _dataSaver.SaveHabbitProgress(HabbitData, _lastResetDate, _progressHistory);
        }

        public void IncreaseProgress()
        {
            if (HabbitData.Progress < HabbitData.Number)
            {
                HabbitData.Progress++;
                UpdateProgressDisplay();
                SaveProgressToHistory();
                SaveCurrentProgress();
            }
        }

        private void UpdateProgressDisplay()
        {
            _progressText.text = $"{HabbitData.Progress}/{HabbitData.Number}";

            float fillAmount = (float)HabbitData.Progress / HabbitData.Number;
            _progressFilledImage.fillAmount = fillAmount;

            if (HabbitData.Progress >= HabbitData.Number)
            {
                _addProgressButton.enabled = false;
                _addProgressButton.image.sprite = _fullProgressButtonSprite;
            }
            else
            {
                _addProgressButton.enabled = true;
                _addProgressButton.image.sprite = _progressButtonEnableSprite;
            }
        }

        private void CheckAndResetProgress()
        {
            DateTime currentDate = DateTime.Now.Date;

            if (_lastResetDate == DateTime.MinValue || currentDate > _lastResetDate)
            {
                if (HabbitData.Progress > 0)
                {
                    SaveProgressToHistory();
                    SaveCurrentProgress();
                }

                if (ShouldResetToday(currentDate))
                {
                    HabbitData.Progress = 0;
                    _lastResetDate = currentDate;
                    SaveCurrentProgress();
                }
            }
        }

        private bool ShouldResetToday(DateTime currentDate)
        {
            return HabbitData.FrequencyType switch
            {
                FrequencyType.EveryDay => true,
                FrequencyType.Once2Days => (currentDate - _lastResetDate).Days >= 2,
                FrequencyType.OnceWeek => (currentDate - _lastResetDate).Days >= 7,
                _ => false
            };
        }

        private void SaveProgressToHistory()
        {
            var dailyProgress = new HabbitDataSaver.DailyProgress
            {
                Date = DateTime.Now.Date,
                Progress = HabbitData.Progress,
                TargetNumber = HabbitData.Number
            };

            _progressHistory.Add(dailyProgress);
        }

        public List<HabbitDataSaver.DailyProgress> GetProgressHistory()
        {
            return new List<HabbitDataSaver.DailyProgress>(_progressHistory ?? new List<HabbitDataSaver.DailyProgress>());
        }
    }
}