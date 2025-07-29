using Unity.Properties;
using UnityEngine.UIElements;
using UnityEngine;

namespace GameCore
{
    public partial class AreaReference
    {
        [CreateProperty]
        public string name
        {
            get => GetArea()?.name ?? "[Not Specified or Invalid]";
        }
    }

    public partial class CardClass
    {
        [CreateProperty]
        public StyleBackground styleBackground => UnityWebRequestImageReader.Instance.FetchStyleBackground(
            Application.streamingAssetsPath + "/" + imagePath
        );
    }

    public partial class Card
    {
        [CreateProperty]
        public CardClass cardClassProp => cardClass;
    }

    public partial class GameState
    {
        [CreateProperty]
        public bool isInPlayingCardPhase => phase == GamePhase.PlayingCard;

        [CreateProperty]
        public bool isInDoingActionPhase => phase == GamePhase.DoingAction;
    }
}