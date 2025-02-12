using System;
using HabbitData;
using UnityEngine;
using UnityEngine.UI;

namespace AddHabbit
{
    public class FrequencyButton : MonoBehaviour
    {
        [SerializeField] private FrequencyType _type;
        [SerializeField] private Button _button;
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _selectedSprite;
        
        public event Action<FrequencyButton> Selected;

        public FrequencyType Type => _type;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClicked);
        }

        public void Reset()
        {
            _button.image.sprite = _defaultSprite;
        }

        public void SetSelected()
        {
            _button.image.sprite = _selectedSprite;
        }
        
        private void OnClicked()
        {
            _button.image.sprite = _selectedSprite;
            Selected?.Invoke(this);
        }
    }
}
