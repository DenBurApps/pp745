using MainScreen;
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

   private void OnEnable()
   {
      _mainScreenButton.onClick.AddListener(OpenMainScreen);
      _statisticsButton.onClick.AddListener(OpenStatisticsScreen);
   }

   private void OnDisable()
   {
      _mainScreenButton.onClick.RemoveListener(OpenMainScreen);
      _statisticsButton.onClick.RemoveListener(OpenStatisticsScreen);
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

   private void DisableAllScreens()
   {
      _statisticsScreenController.DisableScreen();
      _mainScreenController.DisableScreen();
   }
}
