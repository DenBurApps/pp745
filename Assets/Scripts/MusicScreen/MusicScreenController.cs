using System;
using UnityEngine;

namespace MusicScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class MusicScreenController : MonoBehaviour
    {
        [SerializeField] private MusicData[] _musicDatas;
        [SerializeField] private MusicPlane[] _musicPlanes;
        [SerializeField] private OpenMusicScreen _openMusicScreen;

        private ScreenVisabilityHandler _screenVisabilityHandler;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            foreach (var musicPlane in _musicPlanes)
            {
                musicPlane.Clicked += OpenMusicScreen;
            }

            _openMusicScreen.BackClicked += EnableScreen;
        }

        private void OnDisable()
        {
            foreach (var musicPlane in _musicPlanes)
            {
                musicPlane.Clicked -= OpenMusicScreen;
            }
            
            _openMusicScreen.BackClicked -= EnableScreen;
        }

        private void Start()
        {
            DisableScreen();
            InitializeMusicPlanes();
        }

        private void InitializeMusicPlanes()
        {
            for (int i = 0; i < _musicPlanes.Length; i++)
            {
                _musicPlanes[i].SetData(_musicDatas[i]);
            }
        }

        public void EnableScreen()
        {
            _screenVisabilityHandler.EnableScreen();
        }

        public void DisableScreen()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void OpenMusicScreen(MusicData data)
        {
            _openMusicScreen.EnableScreen(data);
            DisableScreen();
        }
    }
}
