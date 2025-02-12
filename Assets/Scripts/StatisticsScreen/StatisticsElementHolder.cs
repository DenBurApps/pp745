using System.Collections.Generic;
using MainScreen.EmojiSelector;
using UnityEngine;

namespace StatisticsScreen
{
    public class StatisticsElementHolder : MonoBehaviour
    {
        [SerializeField] private StatisticsElement[] _elements; // 5 элементов, по одному для каждого EmojiType
        [SerializeField] private int _dayIndex; // Индекс дня недели (0 = воскресенье, 6 = суббота)

        private Dictionary<EmojiType, StatisticsElement> _typeToElement;
        private StatisticsElement _activeElement;

        public int DayIndex => _dayIndex;

        private void Awake()
        {
            ValidateElements();
            InitializeElementsDictionary();
            DisableAllElements();
        }

        private void ValidateElements()
        {
            if (_elements.Length != 5) // Проверяем, что элементов ровно 5 (по количеству типов эмодзи)
            {
                Debug.LogError(
                    $"StatisticsElementHolder для дня {_dayIndex} должен содержать ровно 5 элементов (по одному для каждого типа эмодзи)");
            }
        }

        private void InitializeElementsDictionary()
        {
            _typeToElement = new Dictionary<EmojiType, StatisticsElement>();
            foreach (var element in _elements)
            {
                _typeToElement[element.Type] = element;
            }
        }

        public void EnableTypeElement(EmojiType type)
        {
            // Сначала отключаем текущий активный элемент
            if (_activeElement != null)
            {
                _activeElement.gameObject.SetActive(false);
            }

            // Активируем новый элемент, если он существует для данного типа
            if (_typeToElement.TryGetValue(type, out var element))
            {
                element.gameObject.SetActive(true);
                _activeElement = element;
            }
        }

        public void DisableAllElements()
        {
            foreach (var element in _elements)
            {
                element.gameObject.SetActive(false);
            }

            _activeElement = null;
        }

        public Vector3? GetConnectorPositionForType(EmojiType type)
        {
            if (_typeToElement.TryGetValue(type, out var element) && element.gameObject.activeSelf)
            {
                return element.ConnectorPosition;
            }

            return null;
        }
    }
}