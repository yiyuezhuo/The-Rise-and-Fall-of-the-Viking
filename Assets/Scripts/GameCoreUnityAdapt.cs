using Unity.Properties;
using UnityEngine.UIElements;
using UnityEngine;
using System.Xml.Serialization;

using GameCore;

// public class ReversedUserLogs : ReversedList<string>
// {
//     public ReversedUserLogs()
//     {
//         originalListProvider = () => GameState.current.userLogs;
//     }

//     static ReversedUserLogs _instance = new();
//     public static ReversedUserLogs Instance => _instance;
// }

namespace GameCore
{
    public partial class AreaReference
    {
        [CreateProperty]
        public string name
        {
            get => GetArea()?.name ?? " ";
        }
    }

    public partial class Area
    {
        [CreateProperty]
        public int vikingResourcesProp
        {
            get => vikingResources;
            set => vikingResources = value;
        }

        [CreateProperty]
        public float vikingOccupyingPercentProp
        {
            get => vikingOccupyingPercent;
            set => vikingOccupyingPercent = value;
        }

        [CreateProperty]
        public int hostResourcesProp
        {
            get => hostResources;
            set => hostResources = value;
        }

        [CreateProperty]
        public float vikingChristianizationProp
        {
            get => vikingChristianization;
            set => vikingChristianization = value;
        }

        [CreateProperty]
        public bool lordSetAvailable => GameState.current.lordSetPoint > 0;
    }

    public partial class ResourceAssignParameter
    {
        [CreateProperty]
        public bool editable => GameManager.Instance.editMode; // Temp workaround
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

        [CreateProperty]
        public bool isPlayingCardForActionAvaliable => IsPlayingCardForActionPointAvaliable(GameManager.Instance.selectedCard);

        [CreateProperty]
        public bool isPlayingCardForEventEffectAvailable => IsPlayingCardForEventEffectAvailable(GameManager.Instance.selectedCard);

        [CreateProperty]
        public bool isTradeAvailable => IsTradeAvailable(GameManager.Instance.selectedArea);

        [CreateProperty]
        public bool isCounterInfluenceAvailable => IsCounterInfluenceAvailable(GameManager.Instance.selectedArea);

        [CreateProperty]
        public bool isColonizationAvailable => IsColonizationAvailable(GameManager.Instance.selectedArea);

        [CreateProperty]
        public bool isRaidAvailable => IsRaidAvailable(GameManager.Instance.selectedArea);

        [CreateProperty]
        public bool isConquerAvailable => IsConquerAvailable(GameManager.Instance.selectedArea);

        [CreateProperty]
        public bool isTransferAvailable => IsTransferAvailable(GameManager.Instance.selectedArea);



        // [CreateProperty]
        // public ReversedUserLogs reversedUserLogs => ReversedUserLogs.Instance;
    }
}