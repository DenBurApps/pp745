using System;
using MainScreen.EmojiSelector;

public static class EmojiUpdateManager
{
    public static event Action<DateTime, EmojiType> OnEmojiUpdated;

    public static void NotifyEmojiUpdated(DateTime date, EmojiType type)
    {
        OnEmojiUpdated?.Invoke(date, type);
    }
}