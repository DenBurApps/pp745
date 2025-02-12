using System;
using MainScreen.EmojiSelector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen.Calendar
{
    public class CalendarElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dateNumberText;
        [SerializeField] private TMP_Text _dayNameText;
        [SerializeField] private Image _planeImage;
        [SerializeField] private Color _emojiSelectedColor;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Image _emojiImage;
        [SerializeField] private Button _addEmojiButton;
        [SerializeField] private EmojiSpriteProvider _emojiSpriteProvider;

        public event Action<CalendarElement> ElementClicked;

        private EmojiType _currentType;

        private void OnEnable()
        {
            _addEmojiButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _addEmojiButton.onClick.RemoveListener(OnButtonClicked);
        }

        public void Initialize(DateTime dateTime)
        {
            _dateNumberText.text = dateTime.Day.ToString();
            _dayNameText.text = dateTime.ToString("ddd");

            if (_currentType != EmojiType.None)
            {
                HandleTypeSelected();
                return;
            }

            HandleTypeNotSelected();
        }

        public void AssignEmojiType(EmojiType type)
        {
            if (type != EmojiType.None)
            {
                _currentType = type;
                HandleTypeSelected();
                return;
            }
            
            HandleTypeNotSelected();
        }

        private void HandleTypeSelected()
        {
            _planeImage.color = _emojiSelectedColor;
            _addEmojiButton.gameObject.SetActive(false);
            _emojiImage.gameObject.SetActive(true);
            _emojiImage.sprite = _emojiSpriteProvider.GetSpriteByType(_currentType);
        }

        private void HandleTypeNotSelected()
        {
            _planeImage.color = _defaultColor;
            _addEmojiButton.gameObject.SetActive(true);
            _emojiImage.gameObject.SetActive(false);
        }

        private void OnButtonClicked()
        {
            ElementClicked?.Invoke(this);
        }
    }
}