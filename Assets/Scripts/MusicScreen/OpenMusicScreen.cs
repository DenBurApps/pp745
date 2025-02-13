using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MusicScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class OpenMusicScreen : MonoBehaviour
    {
        [SerializeField] private Sprite _muteOnSprite;
        [SerializeField] private Sprite _muteOffSprite;
        [SerializeField] private Sprite _playSprite;
        [SerializeField] private Sprite _pauseSprite;
        
        [SerializeField] private Image _bgImage;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Slider _durationSlider;
        [SerializeField] private TMP_Text _currentFillProgressText;
        [SerializeField] private TMP_Text _maxDurationText;
        [SerializeField] private Button _muteButton;
        [SerializeField] private Button _pausePlayButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Button _backButton;

        [SerializeField] private AudioSource _audioSource;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private Image _pausePlayButtonImage;
        private Image _muteButtonImage;
        private bool _isPlaying;
        private bool _isMuted;

        public event Action BackClicked;
        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            _pausePlayButtonImage = _pausePlayButton.GetComponent<Image>();
            _muteButtonImage = _muteButton.GetComponent<Image>();
        }

        private void Start()
        {
            DisableScreen();
            InitializeButtons();
        }

        private void Update()
        {
            if (_isPlaying)
            {
                UpdateProgress();
            }
        }

        private void OnBackClicked()
        {
            BackClicked?.Invoke();
            _screenVisabilityHandler.DisableScreen();
        }
        
        private void InitializeButtons()
        {
            _pausePlayButton.onClick.AddListener(OnPlayPauseButtonClick);
            _muteButton.onClick.AddListener(OnMuteButtonClick);
            _stopButton.onClick.AddListener(OnStopButtonClick);
            _backButton.onClick.AddListener(OnBackClicked);
            
            _pausePlayButtonImage.sprite = _playSprite;
            _muteButtonImage.sprite = _muteOffSprite;
        }

        private void UpdateProgress()
        {
            float currentTime = _audioSource.time;
            float duration = _audioSource.clip.length;
            
            _durationSlider.value = currentTime / duration;
            _currentFillProgressText.text = TimeSpan.FromSeconds(currentTime).ToString(@"mm\:ss");
        }

        private void OnPlayPauseButtonClick()
        {
            if (_isPlaying)
            {
                PauseMusic();
            }
            else
            {
                PlayMusic();
            }
        }

        private void PlayMusic()
        {
            _audioSource.Play();
            _isPlaying = true;
            _pausePlayButtonImage.sprite = _pauseSprite;
        }

        private void PauseMusic()
        {
            _audioSource.Pause();
            _isPlaying = false;
            _pausePlayButtonImage.sprite = _playSprite;
        }

        private void OnMuteButtonClick()
        {
            _isMuted = !_isMuted;
            AudioListener.volume = _isMuted ? 0f : 1f;
            _muteButtonImage.sprite = _isMuted ? _muteOnSprite : _muteOffSprite;
        }

        private void OnStopButtonClick()
        {
            _audioSource.Stop();
            _isPlaying = false;
            _pausePlayButtonImage.sprite = _playSprite;
            _currentFillProgressText.text = "00:00";
            _durationSlider.value = 0f;
        }

        public void EnableScreen(MusicData data)
        {
            _screenVisabilityHandler.EnableScreen();

            _bgImage.sprite = data.Background;
            _iconImage.sprite = data.Icon;
            _titleText.text = data.Name;
            _maxDurationText.text = TimeSpan.FromSeconds(data.Duration).ToString(@"mm\:ss");
            _currentFillProgressText.text = "00:00";
            _durationSlider.value = 0f;

            _audioSource.clip = data.Music;
            _isPlaying = false;
            _pausePlayButtonImage.sprite = _playSprite;
        }

        public void DisableScreen()
        {
            OnStopButtonClick();
            _screenVisabilityHandler.DisableScreen();
        }

        private void OnDisable()
        {
            _pausePlayButton.onClick.RemoveListener(OnPlayPauseButtonClick);
            _muteButton.onClick.RemoveListener(OnMuteButtonClick);
            _stopButton.onClick.RemoveListener(OnStopButtonClick);
            _backButton.onClick.RemoveListener(OnBackClicked);
        }
    }
}