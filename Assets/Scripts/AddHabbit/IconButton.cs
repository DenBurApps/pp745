using System;
using HabbitData;
using UnityEngine;
using UnityEngine.UI;

namespace AddHabbit
{
    public class IconButton : MonoBehaviour
    {
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private IconType _type;
        [SerializeField] private Button _button;

        public event Action<IconButton> Selected;

        public IconType Type => _type;

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
            _button.image.color = _defaultColor;
        }

        public void SetSelected()
        {
            _button.image.color = _selectedColor;
        }
        
        public void SetSprite(Sprite sprite)
        {
            _button.image.sprite = sprite;
        }
        
        private void OnClicked()
        {
            _button.image.color = _selectedColor;
            Selected?.Invoke(this);
        }
    }
}
