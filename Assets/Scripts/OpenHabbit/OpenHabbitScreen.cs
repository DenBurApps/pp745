using System;
using AddHabbit;
using MainScreen.HabbitPlane;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenHabbit
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class OpenHabbitScreen : MonoBehaviour
    {
        [SerializeField] private OpenHabbitCalendar _calendar;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _currentDateText;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private ProgressImage[] _progressImages;
        [SerializeField] private TMP_Text _noteText;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _editButton;
        [SerializeField] private IconSpriteProvider _iconSpriteProvider;
        [SerializeField] private AddHabbitScreen _editHabbitScreen;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        
        public event Action BackClicked;

        public HabbitPlane CurrentPlane { get; private set; }

        public void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void Start()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackClicked);
            _editButton.onClick.AddListener(() => OnEditClicked(CurrentPlane));

            foreach (var progressImage in _progressImages)
            {
                progressImage.ProgressAdded += OnProgressAdded;
            }
        }

        private void OnDisable()
        {
            foreach (var progressImage in _progressImages)
            {
                progressImage.ProgressAdded -= OnProgressAdded;
            }
        }

        public void OpenScreen(HabbitPlane habbitPlane)
        {
            CurrentPlane = habbitPlane ?? throw new ArgumentNullException(nameof(habbitPlane));
            _screenVisabilityHandler.EnableScreen();

            UpdateScreenInfo();
            _calendar.Initialize(CurrentPlane);
            InitializeProgressImages();
        }

        private void UpdateScreenInfo()
        {
            _titleText.text = CurrentPlane.HabbitData.Name;
            _currentDateText.text = DateTime.Now.ToString("dd MMMM yyyy");
            _progressText.text = $"{CurrentPlane.HabbitData.Progress} of {CurrentPlane.HabbitData.Number}";
            _noteText.text = CurrentPlane.HabbitData.Note;
        }

        private void InitializeProgressImages()
        {
            foreach (var progressImage in _progressImages)
            {
                progressImage.gameObject.SetActive(false);
            }

            int totalNeeded = CurrentPlane.HabbitData.Number;
            int currentProgress = CurrentPlane.HabbitData.Progress;

            for (int i = 0; i < Mathf.Min(totalNeeded, _progressImages.Length); i++)
            {
                _progressImages[i].gameObject.SetActive(true);

                if (i < currentProgress)
                {
                    _progressImages[i].Initialize(_iconSpriteProvider.GetSpriteByType(CurrentPlane.HabbitData.IconType));
                }
                else
                {
                    _progressImages[i].enabled = true;
                }
            }

            if (totalNeeded > _progressImages.Length)
            {
                Debug.LogWarning(
                    $"Not enough progress image elements. Need {totalNeeded}, have {_progressImages.Length}");
            }
        }

        private void OnProgressAdded(ProgressImage image)
        {
            image.Initialize(_iconSpriteProvider.GetSpriteByType(CurrentPlane.HabbitData.IconType));
            
            if (CurrentPlane.HabbitData.Progress < CurrentPlane.HabbitData.Number)
            {
                CurrentPlane.IncreaseProgress();
                UpdateScreenInfo();
                _calendar.UpdateCurrentDay();
            }
        }

        private void OnEditClicked(HabbitPlane plane)
        {
            _editHabbitScreen.EnableScreen(plane);
            _screenVisabilityHandler.DisableScreen();
        }

        private void OnBackClicked()
        {
            BackClicked?.Invoke();
            _screenVisabilityHandler.DisableScreen();
        }
    }
}