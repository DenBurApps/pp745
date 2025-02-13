using System;
using TMPro;
using UnityEngine;

namespace MusicScreen
{
    [CreateAssetMenu(fileName = "MusicData", menuName = "Music/MusicData")]
    public class MusicData : ScriptableObject
    {
        public Sprite Icon;
        public string Name;
        public float Duration;
        public AudioClip Music;
        public Sprite Background;

        private void OnValidate()
        {
            if (Music != null)
            {
                Duration = Music.length;
            }
        }
    }
}