using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

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
        KievanRus,
        GreatHeathenArmy,
        Danelaw,
        NorthSeaEmpire,
        SweynForkbeard,
        Danegeld,
        VarangianGuard,
        RaidOnParis,
        ErikTheRed,
        SettlementOfIceland,
        RaidOnLisbon
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

            // Moved to scripting layer? Smells not good for engine/framework code.
            if (eventCode == CardClassEventCode.VarangianGuard)
            {
                var norway = GameState.current.FindAreaByName("Norway");
                var constantinople = GameState.current.FindAreaByName("Constantinople");
                norway.vikingResources += 6;
                constantinople.hostResources += 3;
            }

            if (eventCode == CardClassEventCode.Danegeld)
            {
                var denmark = GameState.current.FindAreaByName("Denmark");
                foreach (var area in GameState.current.areas)
                {
                    if (area.hostResources >= 1 && area.hostResources > area.vikingResources && area.hostResources < denmark.vikingResources)
                    {
                        area.hostResources -= 1;
                        denmark.vikingResources += 1;
                    }
                }
            }
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
                cardDescription = "No Event Effect",
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
            {"Great Heathen Army", new()
            {
                name = "Great Heathen Army",
                imagePath = "Cards/Great Heathen Army.png",
                cardDescription = "Great Heathen Army",
                actionPoints = 2,
                eventCode = CardClassEventCode.GreatHeathenArmy
            }},
            {"Danelaw", new()
            {
                name = "Danelaw",
                imagePath = "Cards/Danelaw.png",
                cardDescription = "Danelaw",
                actionPoints = 2,
                eventCode = CardClassEventCode.Danelaw
            }},
            {"North Sea Empire", new()
            {
                name = "North Sea Empire",
                imagePath = "Cards/North Sea Empire.png",
                cardDescription = "North Sea Empire",
                actionPoints = 2,
                eventCode = CardClassEventCode.NorthSeaEmpire
            }},
            {"Sweyn Forkbeard", new()
            {
                name = "Sweyn Forkbeard",
                imagePath = "Cards/Sweyn Forkbeard.jpg",
                cardDescription = "Sweyn Forkbeard",
                actionPoints = 1,
                eventCode = CardClassEventCode.SweynForkbeard
            }},
            { "Danegeld", new()
            {
                name = "Danegeld",
                imagePath = "Cards/Danegeld.png",
                cardDescription = @"Danegeld, or Danish gold, was the tribute paid by regions threatened by Vikings to avoid being plundered.
Every area which host resource is larger than viking resource and lower than Denmark transfer 1 resource to Denmark.",
                actionPoints = 2,
                eventCode = CardClassEventCode.Danegeld
            }},
            //
            { "Varangian Guard", new()
            {
                name = "Varangian Guard",
                imagePath = "Cards/Varangian Guard.jpg",
                cardDescription = @"The Varangian Guard was an elite guard unit composed of Northmen in the Byzantine Empire. It provide the emperor with a powerful tool, while it brought wealth back the Nordic regions.
Norway + 6 resources, Constantinople + 3 resources",
                actionPoints = 2,
                eventCode = CardClassEventCode.VarangianGuard
            }},
            { "Raid on Paris", new()
            {
                name = "Raid on Paris",
                imagePath = "Cards/Raid on Paris.png",
                cardDescription = "Raid on Paris",
                actionPoints = 2,
                eventCode = CardClassEventCode.RaidOnParis
            }},
            { "Erik the Red", new()
            {
                name = "Erik the Red",
                imagePath = "Cards/Erik the Red.png",
                cardDescription = "Erik the Red",
                actionPoints = 2,
                eventCode = CardClassEventCode.ErikTheRed
            }},
            { "Settlement of Iceland", new()
            {
                name = "Settlement of Iceland",
                imagePath = "Cards/Settlement of Iceland.png",
                cardDescription = "Settlement of Iceland",
                actionPoints = 2,
                eventCode = CardClassEventCode.SettlementOfIceland
            }},
            { "Raid on Lisbon", new()
            {
                name = "Raid on Lisbon",
                imagePath = "Cards/Raid on Lisbon.png",
                cardDescription = "Raid on Lisbon",
                actionPoints = 2,
                eventCode = CardClassEventCode.RaidOnLisbon
            }},
        };

        public static CardClass GetByCardClassId(string cardClassId) => classMap[cardClassId];
    }
}