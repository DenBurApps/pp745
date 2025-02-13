using MainScreen;
using MusicScreen;
using StatisticsScreen;
using UnityEngine;
using UnityEngine.UI;

public class LowPlane : MonoBehaviour
{
   [SerializeField] private Button _mainScreenButton;
   [SerializeField] private Button _statisticsButton;
   [SerializeField] private Button _musicButton;
   [SerializeField] private Button _settingsButton;

   [SerializeField] private MainScreenController _mainScreenController;
   [SerializeField] private StatisticsScreenController _statisticsScreenController;
   [SerializeField] private MusicScreenController _musicScreenController;
   [SerializeField] private Settings _settings;

   private void OnEnable()
   {
      _mainScreenButton.onClick.AddListener(OpenMainScreen);
      _statisticsButton.onClick.AddListener(OpenStatisticsScreen);
      _musicButton.onClick.AddListener(OpenMusicScreen);
      _settingsButton.onClick.AddListener(OpenSettings);
   }

   private void OnDisable()
   {
      _mainScreenButton.onClick.RemoveListener(OpenMainScreen);
      _statisticsButton.onClick.RemoveListener(OpenStatisticsScreen);
      _musicButton.onClick.RemoveListener(OpenMusicScreen);
      _settingsButton.onClick.RemoveListener(OpenSettings);
   }

   private void OpenMainScreen()
   {
      DisableAllScreens();
      _mainScreenController.EnableScreen();
   }

   private void OpenStatisticsScreen()
   {
      DisableAllScreens();
      _statisticsScreenController.EnableScreen();
   }

   private void OpenMusicScreen()
   {
      DisableAllScreens();
      _musicScreenController.EnableScreen();
   }

   private void OpenSettings()
   {
      DisableAllScreens();
      _settings.ShowSettings();
   }
   
   private void DisableAllScreens()
   {
      _statisticsScreenController.DisableScreen();
      _mainScreenController.DisableScreen();
      _musicScreenController.DisableScreen();
      _settings.CloseSettings();
   }
}
