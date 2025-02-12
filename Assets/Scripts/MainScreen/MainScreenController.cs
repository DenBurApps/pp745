using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AddHabbit;
using MainScreen.SaveSystem;
using OpenHabbit;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class MainScreenController : MonoBehaviour
    {
        [SerializeField] private Button _addHabbitButton;
        [SerializeField] private List<HabbitPlane.HabbitPlane> _habbitPlanes;
        [SerializeField] private GameObject _emptyDataPlane;
        [SerializeField] private AddHabbitScreen _addHabbitScreen;
        [SerializeField] private AddHabbitScreen _editHabbitScreen;
        [SerializeField] private IconSpriteProvider _iconSpriteProvider;
        [SerializeField] private OpenHabbitScreen _openHabbitScreen;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private HabbitDataSaver _dataSaver;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            _dataSaver = new HabbitDataSaver();
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }

        private void Start()
        {
            DisableAllPlanes();
            LoadSavedHabbits();
            _screenVisabilityHandler.EnableScreen();
        }

        private void OnEnable()
        {
            _addHabbitButton.onClick.AddListener(AddHabbitClicked);
            _openHabbitScreen.BackClicked += _screenVisabilityHandler.EnableScreen;
            _addHabbitScreen.OnHabbitCreated += EnableHabbitPlane;
            _editHabbitScreen.OnHabbitUpdated += _screenVisabilityHandler.EnableScreen;
            _editHabbitScreen.OnHabbitDeleted += DeleteHabbit;

            foreach (var plane in _habbitPlanes)
            {
                plane.PlaneClicked += OnHabbitPlaneClicked;
            }
        }

        private void OnDisable()
        {
            if (_habbitPlanes != null)
            {
                foreach (var plane in _habbitPlanes)
                {
                    if (plane != null)
                    {
                        plane.PlaneClicked -= OnHabbitPlaneClicked;
                    }
                }
            }

            _openHabbitScreen.BackClicked -= _screenVisabilityHandler.EnableScreen;
            
            _addHabbitScreen.OnHabbitCreated -= EnableHabbitPlane;

            _addHabbitButton.onClick.RemoveListener(AddHabbitClicked);
            
            _editHabbitScreen.OnHabbitUpdated -= _screenVisabilityHandler.EnableScreen;
            _editHabbitScreen.OnHabbitDeleted -= DeleteHabbit;
        }
        
        public void EnableScreen()
        {
            _screenVisabilityHandler.EnableScreen();
        }

        public void DisableScreen()
        {
            _screenVisabilityHandler.DisableScreen();
        }
        
        private void SaveAllHabbits()
        {
            foreach (var plane in _habbitPlanes.Where(p => p.IsActive))
            {
                _dataSaver.SaveHabbitProgress(plane.HabbitData, plane.LastResetDate, plane.GetProgressHistory());
            }
        }

        private void LoadSavedHabbits()
        {
            var savedHabbits = _dataSaver.LoadAllHabbits();

            foreach (var habbitData in savedHabbits)
            {
                EnableHabbitPlane(habbitData);
            }
        }

        private void DisableAllPlanes()
        {
            foreach (var habbitPlane in _habbitPlanes)
            {
                habbitPlane.SetSpriteProvider(_iconSpriteProvider);
                habbitPlane.SetDataSaver(_dataSaver);
                habbitPlane.Disable();
            }

            ToggleEmptyData();
        }

        private void ToggleEmptyData()
        {
            _emptyDataPlane.SetActive(!_habbitPlanes.Any(p => p.IsActive));
        }

        private void EnableHabbitPlane(HabbitData.HabbitData data)
        {
            _screenVisabilityHandler.EnableScreen();

            var plane = _habbitPlanes.FirstOrDefault(plane => !plane.IsActive && plane.HabbitData == null);

            if (plane != null)
            {
                plane.Enable(data);
                SaveAllHabbits();
            }
            else
            {
                Debug.LogWarning("No available habit planes!");
            }

            ToggleEmptyData();
        }

        private void OnHabbitPlaneClicked(HabbitPlane.HabbitPlane plane)
        {
            _openHabbitScreen.OpenScreen(plane);
            _screenVisabilityHandler.DisableScreen();
        }

        private void AddHabbitClicked()
        {
            _addHabbitScreen.EnableScreen();
            _screenVisabilityHandler.DisableScreen();
        }

        public void DeleteHabbit(HabbitPlane.HabbitPlane plane)
        {
            _screenVisabilityHandler.EnableScreen();

            if (plane != null && plane.IsActive)
            {
                _dataSaver.DeleteHabbit(plane.HabbitData.Name);
                plane.Disable();
                SaveAllHabbits();
                ToggleEmptyData();
            }
        }
    }
}