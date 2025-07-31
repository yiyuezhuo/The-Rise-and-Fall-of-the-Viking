using System.Collections.Generic;
using System.Diagnostics;

namespace GameCore
{
    public enum CardClassEventCode
    {
        None,
        RaidOnLindisfarne,
        RaidOnLuni,
        SackOfThessalonica,
        DuchyOfNormandy,
        NormanConquestOfSouthernItaly,
        KievanRus
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
            }},
            {"Raid on Lindisfarne", new()
            {
                name = "Raid on Lindisfarne",
                imagePath = "Cards/Raid on Lindisfarne.png",
                cardDescription = "Raid on Lindisfarne",
                actionPoints = 2,
                eventCode = CardClassEventCode.RaidOnLindisfarne
            }},
            {"Raid on Luni", new()
            {
                name = "Action",
                imagePath = "Cards/Raid on Luni.png",
                cardDescription = "Raid on Luni",
                actionPoints = 2,
                eventCode = CardClassEventCode.RaidOnLuni
            }},
            {"Sack of Thessalonica", new()
            {
                name = "Sack of Thessalonica",
                imagePath = "Cards/Sack of Thessalonica.png",
                cardDescription = "Sack of Thessalonica",
                actionPoints = 2,
                eventCode = CardClassEventCode.SackOfThessalonica
            }},
            {"Duchy of Normandy", new()
            {
                name = "Duchy of Normandy",
                imagePath = "Cards/Duchy of Normandy.png",
                cardDescription = "Duchy of Normandy",
                actionPoints = 2,
                eventCode = CardClassEventCode.DuchyOfNormandy
            }},
            {"Norman conquest of Southern Italy", new()
            {
                name = "Norman conquest of Southern Italy",
                imagePath = "Cards/Norman conquest of Southern Italy.png",
                cardDescription = "Norman conquest of Southern Italy",
                actionPoints = 2,
                eventCode = CardClassEventCode.NormanConquestOfSouthernItaly
            }},
            {"Kievan Rus", new()
            {
                name = "Kievan Rus",
                imagePath = "Cards/Kievan Rus.png",
                cardDescription = "Kievan Rus",
                actionPoints = 2,
                eventCode = CardClassEventCode.KievanRus
            }},
        };

        public static CardClass GetByCardClassId(string cardClassId) => classMap[cardClassId];
    }
}