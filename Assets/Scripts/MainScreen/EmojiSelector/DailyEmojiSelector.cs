using System;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen.EmojiSelector
{
    public class DailyEmojiSelector : MonoBehaviour
    {
        [SerializeField] private DailyEmojiSelectionElement[] _elements;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private EmojiSpriteProvider _emojiSpriteProvider;
        
        private DailyEmojiSelectionElement _currentElement;

        public event Action<EmojiType> EmojiSelected; 
        
        private void OnEnable()
        {
            ResetAllElements();
            _confirmButton.onClick.AddListener(OnConfirmClicked);
            
            foreach (var dailyEmojiSelectionElement in _elements)
            {
                dailyEmojiSelectionElement.Clicked += OnElementClicked;
            }
        }

        private void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(OnConfirmClicked);
            
            foreach (var dailyEmojiSelectionElement in _elements)
            {
                dailyEmojiSelectionElement.Clicked -= OnElementClicked;
            }
        }

        private void Start()
        {
            foreach (var dailyEmojiSelectionElement in _elements)
            {
                dailyEmojiSelectionElement.SetSpriteProvider(_emojiSpriteProvider);
            }
        }

        private void OnElementClicked(DailyEmojiSelectionElement element)
        {
            if (_currentElement != null)
                _currentElement.Reset();

            _currentElement = element;
        }

        private void ResetAllElements()
        {
            foreach (var element in _elements)
            {
                element.Reset();
            }
        }

        private void OnConfirmClicked()
        {
            if (_currentElement == null)
            {
                EmojiSelected?.Invoke(EmojiType.None);
                gameObject.SetActive(false);
                return;
            }
            
            EmojiSelected?.Invoke(_currentElement.Type);
            gameObject.SetActive(false);
        }
    }
}