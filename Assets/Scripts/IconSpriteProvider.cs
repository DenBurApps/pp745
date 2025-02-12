using System;
using System.Collections;
using System.Collections.Generic;
using HabbitData;
using MainScreen.EmojiSelector;
using UnityEngine;

public class IconSpriteProvider : MonoBehaviour
{
    [SerializeField] private IconData[] _iconDatas;

    public Sprite GetSpriteByType(IconType type)
    {
        foreach (IconData emojiData in _iconDatas)
        {
            if (emojiData.Type == type)
                return emojiData.Sprite;
        }

        return null;
    }
}

[Serializable]
public class IconData
{
    public Sprite Sprite;
    public IconType Type;
}