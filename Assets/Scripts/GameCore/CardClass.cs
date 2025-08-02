using System;
using System.Collections.Generic;

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
        KyivanRus,
        GreatHeathenArmy,
        Danelaw,
        NorthSeaEmpire,
        SweynForkbeard,
        Danegeld,
        VarangianGuard,
        RaidOnParis,
        ErikTheRed,
        SettlementOfIceland,
        RaidOnLisbon,
        Ragnarok,
        Rurik,
        Valkyrie
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

            if (eventCode == CardClassEventCode.RaidOnParis)
            {
                InitiateRaid("France", 2f);
            }

            if (eventCode == CardClassEventCode.ErikTheRed)
            {
                var greenland = GameState.current.FindAreaByName("Greenland");
                greenland.vikingResources += 2;
            }

            if (eventCode == CardClassEventCode.SettlementOfIceland)
            {
                var iceland = GameState.current.FindAreaByName("Iceland");
                iceland.vikingResources += 2;
            }

            if (eventCode == CardClassEventCode.RaidOnLisbon)
            {
                InitiateRaid("Lisbon", 2f);
            }

            if (eventCode == CardClassEventCode.SweynForkbeard)
            {
                var england = GameState.current.FindAreaByName("England");
                var denmark = GameState.current.FindAreaByName("Denmark");
                var norway = GameState.current.FindAreaByName("Norway");

                england.lord.objectId = denmark.objectId;
                norway.lord.objectId = denmark.objectId;
                england.vikingResources += 5;
                england.hostResources -= 5;
                england.vikingOccupyingPercent += 0.5f;
                denmark.vikingResources += 3;
                norway.vikingResources -= 2;
                GameState.current.victoryPoint += 5;
            }

            if (eventCode == CardClassEventCode.NorthSeaEmpire)
            {
                GameState.current.isFreeTransfer = true;
                GameState.current.lordSetPoint += 1;
            }

            if (eventCode == CardClassEventCode.Danelaw)
            {
                var gameState = GameState.current;

                gameState.victoryPoint += 30;
                var england = gameState.FindAreaByName("England");
                england.vikingResources += 3;
                england.hostResources += 3;
                england.vikingChristianization += 0.1f;
                england.lord.objectId = null;
            }

            if (eventCode == CardClassEventCode.GreatHeathenArmy)
            {
                InitiateConquest("England", 2f);
            }

            if (eventCode == CardClassEventCode.KyivanRus)
            {
                var gameState = GameState.current;
                gameState.victoryPoint += 30;

            }
        }

        public void InitiateRaid(string areaName, float modifierCoef)
        {
            GameManager.Instance.PrepareSelectingAreaCallback(area =>
            {
                var france = GameState.current.FindAreaByName(areaName);
                var p = new ResourceAssignParameter()
                {
                    from = new AreaReference() { objectId = area.objectId },
                    to = new AreaReference() { objectId = france.objectId },
                    assignResource = area.GetRaidAssignedResourceLimit(),
                    assignResourceLimit = area.GetRaidAssignedResourceLimit(),
                    modifierCoef = modifierCoef
                };

                DialogRoot.Instance.PopupResourceAssignParameterDialog(p, "Raid", p =>
                {
                    GameState.current.DoRaid(p, new() { skipActionPoint = true, skipPhaseCheck = true });
                });
            });
        }

        public void InitiateConquest(string areaName, float modifierCoef)
        {
            GameManager.Instance.PrepareSelectingAreaCallback(area =>
            {
                var france = GameState.current.FindAreaByName(areaName);
                var p = new ResourceAssignParameter()
                {
                    from = new AreaReference() { objectId = area.objectId },
                    to = new AreaReference() { objectId = france.objectId },
                    assignResource = area.GetConquerAssignedResourceLimit(),
                    assignResourceLimit = area.GetConquerAssignedResourceLimit(),
                    modifierCoef = modifierCoef
                };

                DialogRoot.Instance.PopupResourceAssignParameterDialog(p, "Conquest", p =>
                {
                    GameState.current.DoConquest(p, new() { skipActionPoint = true, skipPhaseCheck = true });
                });
            });
        }

        public static Dictionary<CardClassEventCode, string> eventCodeDescription = new()
        {
            { CardClassEventCode.None, "None" },
        };

        static Dictionary<string, CardClass> classMap = new()
        {
            {"Longship", new()
            {
                name = "Longship",
                imagePath = "Cards/Long Boat Placeholder.png",
                cardDescription = @"Longship is long slender ship used by Norsemen for commerce, exploration and warfare during the Viking age.
No Event Effect",
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
            {"Kyivan Rus", new()
            {
                name = "Kyivan Rus",
                imagePath = "Cards/Kyivan Rus.png",
                cardDescription = @"The Kyivan Rus' (880–1240) was a state established by Varangian ruler Oleg the Wise, ruling over Kyiv, Novgorod, and the areas along the Dnieper River. The state continued to expand until Mongols invasion.
Can only be played if Novgorod and Kyiv have at least 50% control percentage. +30 VP, Kyiv become lord of Novgorod, Kyiv & Novgorod +3 resources",
                actionPoints = 2,
                eventCode = CardClassEventCode.KyivanRus
            }},
            {"Great Heathen Army", new()
            {
                name = "Great Heathen Army",
                imagePath = "Cards/Great Heathen Army.png",
                cardDescription = @"The Great Heathen Army (865–878) was a massive Viking force led by the Ragnar's sons—Halfdan Ragnarsson, Ivar the Boneless, and Ubba—in their invasion of England. Although their conquest was halted by Alfred the Great, the campaigns led to the creation of the Danelaw, a region under Viking control.
Select an area to initiate conquest to England (ignoring distance), the strength is calculated as x2.",
                actionPoints = 2,
                eventCode = CardClassEventCode.GreatHeathenArmy
            }},
            {"Danelaw", new()
            {
                name = "Danelaw",
                imagePath = "Cards/Danelaw.png",
                cardDescription = @"Danelaw was part of eastern and northern England which originated in the conquest of Danish Viking from 878. After reconquest, those area are subject to England again but be able to keep thier Danish law.
Can only be activated if England has >= 50% Viking Occupation. +30 VP, +10% Christianization, +3 Viking/Host resource in England, England is released from its lord (if any).",
                actionPoints = 1,
                eventCode = CardClassEventCode.Danelaw
            }},
            {"North Sea Empire", new()
            {
                name = "North Sea Empire",
                imagePath = "Cards/North Sea Empire.png",
                cardDescription = @"North Sea Empire, or Anglo-Scandinavian Empire, was the personal union of the kingdoms of Denmark, Norway and England (1013-1042). It's most powerful entity in western Europe after the Holy Roman Empire.
For this turn, 1 Lord set point is available and transfers are free.",
                actionPoints = 2,
                eventCode = CardClassEventCode.NorthSeaEmpire
            }},
            {"Sweyn Forkbeard", new()
            {
                name = "Sweyn Forkbeard",
                imagePath = "Cards/Sweyn Forkbeard.jpg",
                cardDescription = @"Sweyn Forkbeard (963–1014), King of Denmark, conquered Norway in 999–1000. After some small-scale campaigns in response to the St. Brice's Day Massacre (1002), his forces launched a full-scale invasion of England and conquered it in 1013–1014, though his son later carried out a reconquest.
Denmark become lord of Norway and England, England's host force -5, Viking force+5, Controlled percentage +50%, Denmark's resource+3, Norway's ressource -2, VP+5",
                actionPoints = 3,
                eventCode = CardClassEventCode.SweynForkbeard
            }},
            { "Danegeld", new()
            {
                name = "Danegeld",
                imagePath = "Cards/Danegeld.png",
                cardDescription = @"Danegeld, or Danish gold, was the tribute paid by regions threatened by Vikings to avoid being plundered.
Every area which host resource is larger than viking resource and lower than Denmark transfer 1 resource to Denmark.",
                actionPoints = 3,
                eventCode = CardClassEventCode.Danegeld
            }},
            //
            { "Varangian Guard", new()
            {
                name = "Varangian Guard",
                imagePath = "Cards/Varangian Guard.jpg",
                cardDescription = @"The Varangian Guard was an elite guard unit composed of Northmen in the Byzantine Empire. It provide the emperor with a powerful tool, while it brought wealth back the Nordic regions.
Norway + 6 resources, Constantinople + 3 resources",
                actionPoints = 1,
                eventCode = CardClassEventCode.VarangianGuard
            }},
            { "Raid on Paris", new()
            {
                name = "Raid on Paris",
                imagePath = "Cards/Raid on Paris.png",
                cardDescription = @"In 845, Ragnar led 120 Viking ships carrying thousands of warriors, plundered and occupied Paris, and withdrew after Charles the Bald paid 2,570 kg of gold and silver.
Select an area to initiate a raid (ignoring the distance), with the assigned strength calculated at a x2 modifier.",
                actionPoints = 2,
                eventCode = CardClassEventCode.RaidOnParis
            }},
            { "Erik the Red", new()
            {
                name = "Erik the Red",
                imagePath = "Cards/Erik the Red.png",
                cardDescription = @"Erik the Red (c. 950 - c. 1003) was a Norse explorer who explored Greenland and eventually established a settlement.
Greenland +2 Resources.",
                actionPoints = 1,
                eventCode = CardClassEventCode.ErikTheRed
            }},
            { "Settlement of Iceland", new()
            {
                name = "Settlement of Iceland",
                imagePath = "Cards/Settlement of Iceland.png",
                cardDescription = @"Beginning around 870, numerous Norse settlers migrated to Iceland across the North Atlantic. By 930, the island had been 'fully settled'
Iceland + 2 Resources",
                actionPoints = 2,
                eventCode = CardClassEventCode.SettlementOfIceland
            }},
            { "Raid on Lisbon", new()
            {
                name = "Raid on Lisbon",
                imagePath = "Cards/Raid on Lisbon.png",
                cardDescription = @"Beginning from August 844, a raid party reached Galicia and then raid Lisbon and Seville.
Select an area to initiate a raid to Lisbon (ignoring distance), with the assigned strength calculated at a x2 modifier.",
                actionPoints = 1,
                eventCode = CardClassEventCode.RaidOnLisbon
            }},
            // ...
            { "Shield Maiden", new()
            {
                name = "Shield Maiden",
                imagePath = "Cards/Shield Maiden.jpg",
                cardDescription = @"Shield-maiden refer to female warriors in Viking culture.
No Event Effect",
                actionPoints = 1,
                eventCode = CardClassEventCode.None
            }},
            { "Valkyrie", new()
            {
                name = "Valkyrie",
                imagePath = "Cards/Valkyrie.jpg",
                cardDescription = @"Valkyrie",
                actionPoints = 1,
                eventCode = CardClassEventCode.Valkyrie
            }},
            { "Ragnarok", new()
            {
                name = "Ragnarok",
                imagePath = "Cards/Ragnarok.jpg",
                cardDescription = @"Ragnarok",
                actionPoints = 1,
                eventCode = CardClassEventCode.RaidOnLisbon
            }},
            { "Rurik", new()
            {
                name = "Rurik",
                imagePath = "Cards/Rurik.jpg",
                cardDescription = @"Rurik",
                actionPoints = 2,
                eventCode = CardClassEventCode.Rurik
            }},

        };

        public static CardClass GetByCardClassId(string cardClassId) => classMap[cardClassId];

        public static Dictionary<CardClassEventCode, Func<bool>> eventActivatedConditionMap = new()
        {
            {CardClassEventCode.Danelaw, () => {
                var england = GameState.current.FindAreaByName("England");
                return england.vikingOccupyingPercent >= 0.5f;
            }},
            {CardClassEventCode.KyivanRus, () => {
                var kivy = GameState.current.FindAreaByName("Kivy");
                var novgorod = GameState.current.FindAreaByName("Novgorod");
                return kivy.vikingOccupyingPercent >= 0.5f && novgorod.vikingOccupyingPercent >= 0.5f;
            }},

        };
    }
}