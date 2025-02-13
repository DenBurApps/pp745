using System;
using System.Collections.Generic;
using MainScreen.EmojiSelector;
using UnityEngine;

namespace StatisticsScreen
{
    public class StatisticsElementHolder : MonoBehaviour
    {
        [SerializeField] private StatisticsElement[] _elements;
        [SerializeField] private int _dayIndex;

        private Dictionary<EmojiType, StatisticsElement> _typeToElement;
        private StatisticsElement _activeElement;

        public int DayIndex => _dayIndex;

        private void Awake()
        {
            InitializeElementsDictionary();
            DisableAllElements();
        }
        
        public StatisticsElement GetActiveElement()
        {
            var activeElement = Array.Find(_elements, element => element.gameObject.activeSelf);
    
            return activeElement;
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
            if (_activeElement != null)
            {
                _activeElement.gameObject.SetActive(false);
            }

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
    }
}