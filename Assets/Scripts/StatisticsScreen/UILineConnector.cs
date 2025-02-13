using System;
using System.Collections.Generic;
using MainScreen.EmojiSelector;
using UnityEngine;
using UnityEngine.UI;

namespace StatisticsScreen
{
    [Serializable]
    public class EmojiLineSettings
    {
        public EmojiType Type;
        public Color LineColor;
        public float LineWidth = 2f;
    }

    public class UILineConnector : MonoBehaviour
    {
        [SerializeField] private Image _linePrefab;
        [SerializeField] private RectTransform _linesContainer;
        [SerializeField] private EmojiLineSettings[] _lineSettings;
        [SerializeField] private float _lineShortening = 20f;

        private readonly Dictionary<EmojiType, Color> _typeColors = new();
        private readonly List<Image> _activeLines = new();

        private void Awake()
        {
            foreach (var setting in _lineSettings)
            {
                _typeColors[setting.Type] = setting.LineColor;
            }
        }

        public void ClearLines()
        {
            foreach (var line in _activeLines)
            {
                Destroy(line.gameObject);
            }

            _activeLines.Clear();
        }

        public void DrawLine(Vector2 startPoint, Vector2 endPoint, EmojiType startType, EmojiType endType)
        {
            var line = Instantiate(_linePrefab, _linesContainer);
            _activeLines.Add(line);

            if (_typeColors.TryGetValue(startType, out Color color))
            {
                line.color = color;
            }

            Vector2 direction = (endPoint - startPoint).normalized;
            float fullDistance = Vector2.Distance(startPoint, endPoint);

            Vector2 adjustedStart = startPoint + direction * _lineShortening;
            Vector2 adjustedEnd = endPoint - direction * _lineShortening;
            float adjustedDistance = Vector2.Distance(adjustedStart, adjustedEnd);
            
            if (adjustedDistance <= 0)
            {
                adjustedDistance = 1f;
                adjustedEnd = adjustedStart + direction * adjustedDistance;
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Vector2 midPoint = (adjustedStart + adjustedEnd) / 2;

            RectTransform rect = line.rectTransform;
            rect.position = midPoint;
            rect.rotation = Quaternion.Euler(0, 0, angle);
            rect.sizeDelta = new Vector2(adjustedDistance, _lineSettings[0].LineWidth);
        }
    }
}