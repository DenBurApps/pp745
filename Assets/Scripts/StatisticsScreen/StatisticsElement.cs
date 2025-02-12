using MainScreen.EmojiSelector;
using UnityEngine;

namespace StatisticsScreen
{
    public class StatisticsElement : MonoBehaviour
    {
        [SerializeField] private EmojiType _emojiType;
        [SerializeField] private Transform _connectorPoint;

        public EmojiType Type => _emojiType;
        public Vector3 ConnectorPosition => _connectorPoint.position;
    }
}
