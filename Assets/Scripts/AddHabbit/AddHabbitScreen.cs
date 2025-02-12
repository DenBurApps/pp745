using HabbitData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using MainScreen.HabbitPlane;

namespace AddHabbit
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class AddHabbitScreen : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private TMP_Text _numberOfUsesText;
        [SerializeField] private Button _decreaseNumberButton;
        [SerializeField] private Button _increaseNumberButton;
        [SerializeField] private FrequencyButton[] _frequencyButtons;
        [SerializeField] private IconButton[] _iconButtons;
        [SerializeField] private TMP_InputField _noteInput;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private IconSpriteProvider _iconSpriteProvider;

        private IconButton _currentIconButton;
        private FrequencyButton _currentFrequency;
        private int _usesNumber = 1;
        private ScreenVisabilityHandler _screenVisabilityHandler;
        private HabbitPlane _currentEditingHabbit;

        public event Action<HabbitData.HabbitData> OnHabbitCreated;
        public event Action<HabbitPlane> OnHabbitDeleted;
        public event Action OnHabbitUpdated;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            _decreaseNumberButton.onClick.AddListener(DecreaseNumber);
            _increaseNumberButton.onClick.AddListener(IncreaseNumber);
            _saveButton.onClick.AddListener(SaveHabbit);
            _deleteButton.onClick.AddListener(OnDeleteButtonClicked);

            foreach (var button in _frequencyButtons)
                button.Selected += FrequencyButtonClicked;

            foreach (var button in _iconButtons)
            {
                button.Selected += OnIconButtonClicked;
                button.SetSprite(_iconSpriteProvider.GetSpriteByType(button.Type));
            }

            _nameInput.onValueChanged.AddListener(_ => ValidateInput());
            _noteInput.onValueChanged.AddListener(_ => ValidateInput());
        }

        private void Start()
        {
            _screenVisabilityHandler.DisableScreen();
            ResetScreen();
        }

        public void EnableScreen()
        {
            _screenVisabilityHandler.EnableScreen();
        }
        
        public void EnableScreen(HabbitPlane habbitToEdit = null)
        {
            _screenVisabilityHandler.EnableScreen();
            
            if (habbitToEdit != null)
            {
                _currentEditingHabbit = habbitToEdit;
                LoadHabbitData(habbitToEdit.HabbitData);
            }
        }
        
        private void LoadHabbitData(HabbitData.HabbitData habbitData)
        {
            _nameInput.text = habbitData.Name;
            _noteInput.text = habbitData.Note;
            _usesNumber = habbitData.Number;
            _numberOfUsesText.text = _usesNumber.ToString();

            var frequencyButton = _frequencyButtons.FirstOrDefault(b => b.Type == habbitData.FrequencyType);
            if (frequencyButton != null)
            {
                FrequencyButtonClicked(frequencyButton);
                frequencyButton.SetSelected();
            }

            var iconButton = _iconButtons.FirstOrDefault(b => b.Type == habbitData.IconType);
            if (iconButton != null)
            {
                OnIconButtonClicked(iconButton);
                iconButton.SetSelected();
            }

            ValidateInput();
        }

        private void OnDisable()
        {
            if (_frequencyButtons != null)
            {
                foreach (var button in _frequencyButtons)
                    if (button != null)
                        button.Selected -= FrequencyButtonClicked;
            }

            if (_iconButtons != null)
            {
                foreach (var button in _iconButtons)
                    if (button != null)
                        button.Selected -= OnIconButtonClicked;
            }
            
            _decreaseNumberButton.onClick.RemoveListener(DecreaseNumber);
            _increaseNumberButton.onClick.RemoveListener(IncreaseNumber);
            _saveButton.onClick.RemoveListener(SaveHabbit);
            _deleteButton.onClick.RemoveListener(OnDeleteButtonClicked);
            _nameInput.onValueChanged.RemoveListener(_ => ValidateInput());
            _noteInput.onValueChanged.RemoveListener(_ => ValidateInput());
        }

        private void SaveHabbit()
        {
            var habbitData = new HabbitData.HabbitData(
                _nameInput.text,
                _usesNumber,
                _currentFrequency.Type,
                _currentIconButton.Type,
                _noteInput.text
            );

            if (_currentEditingHabbit != null)
            {
               _currentEditingHabbit.UpdateValues(habbitData);
               OnHabbitUpdated?.Invoke();
            }
            else
            {
                OnHabbitCreated?.Invoke(habbitData);
            }

            _screenVisabilityHandler.DisableScreen();
            ResetScreen();
        }

        private void ResetScreen()
        {
            _currentEditingHabbit = null;
            _nameInput.text = string.Empty;
            _noteInput.text = string.Empty;
            _usesNumber = 1;
            _numberOfUsesText.text = _usesNumber.ToString();

            if (_currentFrequency != null)
            {
                _currentFrequency.Reset();
                _currentFrequency = null;
            }

            if (_currentIconButton != null)
            {
                _currentIconButton.Reset();
                _currentIconButton = null;
            }

            ValidateInput();
        }

        private void IncreaseNumber()
        {
            _usesNumber++;
            _numberOfUsesText.text = _usesNumber.ToString();
        }

        private void DecreaseNumber()
        {
            _usesNumber = Mathf.Clamp(_usesNumber - 1, 1, int.MaxValue);
            _numberOfUsesText.text = _usesNumber.ToString();
        }

        private void FrequencyButtonClicked(FrequencyButton button)
        {
            if (_currentFrequency != null)
                _currentFrequency.Reset();

            _currentFrequency = button;
            ValidateInput();
        }

        private void OnIconButtonClicked(IconButton iconButton)
        {
            if (_currentIconButton != null)
                _currentIconButton.Reset();

            _currentIconButton = iconButton;
            ValidateInput();
        }

        private void OnDeleteButtonClicked()
        {
            if (_currentEditingHabbit != null)
            {
                OnHabbitDeleted?.Invoke(_currentEditingHabbit);
            }
            
            ResetScreen();
            _screenVisabilityHandler.DisableScreen();
        }
        
        private void ValidateInput()
        {
            _saveButton.interactable = !string.IsNullOrEmpty(_nameInput.text) && _currentIconButton != null &&
                                       _currentFrequency != null &&
                                       !string.IsNullOrEmpty(_noteInput.text);
        }
    }
}