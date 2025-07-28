using System.Collections.Generic;

namespace GameCore
{
    public enum CardClassEventCode
    {
        None,
    }

    public class CardClass
    {
        public string name;
        public string imagePath;
        public string cardDescription;
        public string eventDescription => eventCodeDescription[eventCode];
        public int actionPoints;
        public CardClassEventCode eventCode;

        public virtual void DoEvent()
        {

        }

        public static Dictionary<CardClassEventCode, string> eventCodeDescription = new()
        {
            { CardClassEventCode.None, "None" },
        };

        public static Dictionary<string, CardClass> classMap = new()
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
    }
}