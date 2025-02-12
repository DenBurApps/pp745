using System;
using MainScreen.EmojiSelector;
using UnityEngine;
using UnityEngine.UI;

namespace StatisticsScreen
{
    using System;
    using MainScreen.EmojiSelector;
    using UnityEngine;
    using UnityEngine.UI;

    namespace StatisticsScreen
    {
        public class StatisticsTypeHolder : MonoBehaviour
        {
            [SerializeField] private EmojiType _type;
            [SerializeField] private Button _button;
            [SerializeField] private Color _selectedColor;
            [SerializeField] private Color _notSelectedColor;
            [SerializeField] private bool _startSelected;

            private bool _isSelected;

            public event Action<StatisticsTypeHolder> Clicked;

            public EmojiType Type => _type;
            public bool IsSelected => _isSelected;

            private void Start()
            {
                // Initialize with default state
                if (_startSelected)
                {
                    SetEnabled();
                }
                else
                {
                    ResetHolder();
                }
            }

            private void OnEnable()
            {
                _button.onClick.AddListener(OnButtonClicked);
            }

            private void OnDisable()
            {
                _button.onClick.RemoveListener(OnButtonClicked);
            }

            public void ResetHolder()
            {
                _button.image.color = _notSelectedColor;
                _isSelected = false;
            }

            public void SetEnabled()
            {
                _button.image.color = _selectedColor;
                _isSelected = true;
            }

            private void OnButtonClicked()
            {
                Toggle();
                Clicked?.Invoke(this);
            }

            public void Toggle()
            {
                if (_isSelected)
                    ResetHolder();
                else
                    SetEnabled();
            }
        }
    }
}