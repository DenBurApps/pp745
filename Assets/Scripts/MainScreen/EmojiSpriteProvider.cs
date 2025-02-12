using System;
using MainScreen.EmojiSelector;
using UnityEngine;

namespace MainScreen
{
    public class EmojiSpriteProvider : MonoBehaviour
    {
        [SerializeField] private EmojiData[] _emojiDatas;

        public Sprite GetSpriteByType(EmojiType type)
        {
            foreach (EmojiData emojiData in _emojiDatas)
            {
                if (emojiData.Type == type)
                    return emojiData.Sprite;
            }

            return null;
        }
    }

    [Serializable]
    public class EmojiData
    {
        public Sprite Sprite;
        public EmojiType Type;
    }
}
