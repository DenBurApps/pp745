using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenHabbit
{
    public class OpenHabbitCalendarElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dateNumberText;
        [SerializeField] private TMP_Text _dayNameText;
        [SerializeField] private Image _planeImage;
        [SerializeField] private Image _filledImage;
        [SerializeField] private Image _icon;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _pastDayColor;

        private DateTime _currentDate;

        public DateTime CurrentDate => _currentDate;

        public void Initialize(DateTime dateTime, Sprite sprite, float progress)
        {
            _currentDate = dateTime;
            
            _dateNumberText.text = dateTime.Day.ToString();
            _dayNameText.text = dateTime.ToString("ddd");

            _icon.sprite = sprite;
            UpdateProgress(progress);
        }

        public void UpdateProgress(float progress)
        {
            _filledImage.fillAmount = progress;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (_currentDate.Date == DateTime.Now.Date)
            {
                _planeImage.color = _selectedColor;
            }
            else if (_currentDate.Date < DateTime.Now.Date)
            {
                _planeImage.color = _pastDayColor;
            }
            else
            {
                _planeImage.color = _defaultColor;
            }

            _filledImage.gameObject.SetActive(_currentDate.Date <= DateTime.Now.Date);
        }

        public void SetSelected(bool isSelected)
        {
            if (isSelected)
            {
                _planeImage.color = _selectedColor;
            }
            else
            {
                UpdateVisuals();
            }
        }
    }
}