using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen.EmojiSelector
{
   public enum EmojiType
   {
      Inspired,
      Happy,
      Surprised,
      Sad,
      Angry,
      None
   }
   
   public class DailyEmojiSelectionElement : MonoBehaviour
   {
      [SerializeField] private EmojiType _type;
      [SerializeField] private Button _selectButton;
      [SerializeField] private Sprite _selectedButtonSprite;
      [SerializeField] private Sprite _defaultButtonSprite;
      [SerializeField] private TMP_Text _text;
      [SerializeField] private Image _emojiImage;

      private EmojiSpriteProvider _emojiSpriteProvider;
      private bool _isClicked;
      
      public event Action<DailyEmojiSelectionElement> Clicked;

      public EmojiType Type => _type;

      private void OnEnable()
      {
         _selectButton.onClick.AddListener(OnButtonClicked);
         _text.text = _type.ToString();
      }

      private void OnDisable()
      {
         _selectButton.onClick.RemoveListener(OnButtonClicked);
      }

      public void SetSpriteProvider(EmojiSpriteProvider spriteProvider)
      {
         _emojiSpriteProvider = spriteProvider;
         _emojiImage.sprite = _emojiSpriteProvider.GetSpriteByType(_type);
      }
      
      public void Reset()
      {
         _isClicked = false;
         SetSpriteState();
      }

      private void SetSpriteState()
      {
         _selectButton.image.sprite = _isClicked ? _selectedButtonSprite : _defaultButtonSprite;
      }

      private void OnButtonClicked()
      {
         Clicked?.Invoke(this);

         _isClicked = !_isClicked;
         SetSpriteState();
      }
   }
}
