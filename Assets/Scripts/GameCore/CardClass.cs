using System.Collections.Generic;
using System.Diagnostics;

namespace GameCore
{
    public enum CardClassEventCode
    {
        None,
    }

    public partial class CardClass
    {
        public string name;
        public string imagePath;
        public string cardDescription;
        public string eventDescription => eventCodeDescription[eventCode];
        public int actionPoints;
        public CardClassEventCode eventCode;

        public virtual void ApplyEventEffect()
        {
            ServiceLocator.Get<ILoggerService>().Log($"{name} applied event effect");
        }

        public static Dictionary<CardClassEventCode, string> eventCodeDescription = new()
        {
            { CardClassEventCode.None, "None" },
        };

        static Dictionary<string, CardClass> classMap = new()
        {
            {"Action", new()
            {
                name = "Action",
                imagePath = "Cards/Long Boat Placeholder.png",
                cardDescription = "Normal Action",
                actionPoints = 1,
                eventCode = CardClassEventCode.None
            }}
        };

        public static CardClass GetByCardClassId(string cardClassId) => classMap[cardClassId];
    }
}