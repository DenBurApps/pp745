using System;
using UnityEngine;
using UnityEngine.UI;

namespace OpenHabbit
{
    public class ProgressImage : MonoBehaviour
    {
        [SerializeField] private Button _addProgressButton;
        [SerializeField] private Sprite _addProgressSprite;
        [SerializeField] private Sprite _progressFilledSprite;

        public event Action<ProgressImage> ProgressAdded;

        private void OnEnable()
        {
            _addProgressButton.enabled = true;
            _addProgressButton.image.sprite = _addProgressSprite;
            _addProgressButton.onClick.AddListener(OnProgressButtonClicked);
        }

        private void OnDisable()
        {
            _addProgressButton.onClick.RemoveListener(OnProgressButtonClicked);
        }

        private void OnProgressButtonClicked()
        {
            ProgressAdded?.Invoke(this);
        }

        public void Initialize(Sprite sprite)
        {
            _addProgressButton.image.sprite = sprite;
            _addProgressButton.enabled = false;
        }
    }
}
