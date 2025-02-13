using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MusicScreen
{
    public class MusicPlane : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _durationText;
        [SerializeField] private Button _openButton;

        public event Action<MusicData> Clicked;
        
        public MusicData MusicData { get; private set; }

        private void OnEnable()
        {
            _openButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(OnButtonClicked);
        }

        public void SetData(MusicData data)
        {
            MusicData = data ?? throw new ArgumentNullException(nameof(data));

            _iconImage.sprite = MusicData.Icon;
            _titleText.text = MusicData.Name;
            _durationText.text = MusicData.Duration.ToString("00:00");
        }

        private void OnButtonClicked()
        {
            Clicked?.Invoke(MusicData);
        }
    }
}
