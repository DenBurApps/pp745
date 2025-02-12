using System;
using MainScreen;
using MainScreen.EmojiSelector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StatisticsScreen
{
    public class DailyPlane : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _dateText;
        [SerializeField] private EmojiSpriteProvider _emojiSpriteProvider;
        [SerializeField] private Button _addEmojiButton;
        [SerializeField] private Transform _connectorPoint;

        public event Action<DailyPlane> Clicked;
        
        public EmojiType EmojiType { get; private set; }
        public Vector3 ConnectorPosition => _connectorPoint.position;
        public DateTime Date { get; private set; }

        private void OnEnable()
        {
            _addEmojiButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _addEmojiButton.onClick.RemoveListener(OnButtonClicked);
        }

        public void Initialize(DateTime date, EmojiType type)
        {
            Date = date;
            EmojiType = type;
            _dateText.text = date.ToString("dd");

            if (type != EmojiType.None)
            {
                _iconImage.sprite = _emojiSpriteProvider.GetSpriteByType(type);
                _iconImage.gameObject.SetActive(true);
                _addEmojiButton.gameObject.SetActive(false);
                return;
            }
            
            _iconImage.gameObject.SetActive(false);
            _addEmojiButton.gameObject.SetActive(true);
        }

        public void UpdateEmojiType(EmojiType type)
        {
            EmojiType = type;
            
            if (type != EmojiType.None)
            {
                _iconImage.sprite = _emojiSpriteProvider.GetSpriteByType(type);
                _iconImage.gameObject.SetActive(true);
                _addEmojiButton.gameObject.SetActive(false);
            }
        }

        private void OnButtonClicked()
        {
            Clicked?.Invoke(this);
        }
    }
}