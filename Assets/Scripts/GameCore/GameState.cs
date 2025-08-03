using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GameCore
{
    public enum GamePhase
    {
        PlayingCard,
        DiscardCard,
        DoingAction,
        GameEnd
    }

    public partial class CardReference
    {
        public string objectId;
        public Card GetCard()
        {
            return EntityManager.current.Get<Card>(objectId);
        }
    }


    public partial class GameState
    {
        public int beginYear = 793;
        public int endYear = 1066;
        public int currentYear = 793;
        public int turnLengthYear = 10;

        public int availableCardPlay = 0;
        public int cardLimit = 4;

        public GamePhase phase;

        public int availableActionPoints;
        public int victoryPoint;
        public int lordSetPoint;

        public bool isFreeTransfer;

        public List<Area> areas = new();
        public List<Card> cards = new();

        public List<CardReference> deckCardReferences = new();
        public List<CardReference> handCardReferences = new();
        public List<CardReference> discardCardReferences = new();

        public bool cardHasSetup = false;
        public bool reshuffled = false;

        public List<string> userLogs = new();

        public void RebuildPathfindng()
        {
            foreach (var area in areas)
            {
                area.RebuildPaths();
            }
        }

        public void CreateDefaultDecks()
        {
            cards = new()
            {
                new Card(){cardClassId="Berserker"},
                new Card(){cardClassId="Berserker"},
                new Card(){cardClassId="Normans"},
                new Card(){cardClassId="The Volga and Dnieper Trade Routes"},
                new Card(){cardClassId="England 1066 Campaign"},

                new Card(){cardClassId="Raid on Lindisfarne"},
                new Card(){cardClassId="Raid on Luna"},
                new Card(){cardClassId="Sack of Thessalonica"},
                new Card(){cardClassId="Duchy of Normandy"},
                new Card(){cardClassId="Norman conquest of Southern Italy"},

                new Card(){cardClassId="Kyivan Rus"},
                new Card(){cardClassId="Rurik"},
                new Card(){cardClassId="Shield Maiden"},
                new Card(){cardClassId="Valkyrie"},
                new Card(){cardClassId="Ragnarok"},

                new Card(){cardClassId="Great Heathen Army"},
                new Card(){cardClassId="Danelaw"},
                new Card(){cardClassId="North Sea Empire"},
                new Card(){cardClassId="Sweyn Forkbeard"},
                new Card(){cardClassId="Danegeld"},

                new Card(){cardClassId="Varangian Guard"},
                new Card(){cardClassId="Raid on Paris"},
                new Card(){cardClassId="Erik the Red"},
                new Card(){cardClassId="Settlement of Iceland"},
                new Card(){cardClassId="Raid on Lisbon"},

                new Card(){cardClassId="Longship"},
                new Card(){cardClassId="Longship"},
                new Card(){cardClassId="Longship"},
                new Card(){cardClassId="Longship"},
                new Card(){cardClassId="Longship"},
            };

            ResetAndRegisterAll();

            deckCardReferences = cards.Select(card => new CardReference() { objectId = card.objectId }).ToList();
        }

        public void EnsureSetup()
        {
            if (!cardHasSetup)
            {
                cardHasSetup = true;

                CreateDefaultDecks();

                if (!CoreManager.Instance.disableReshuffle && !reshuffled)
                {
                    reshuffled = true;
                    RandomUtils.Shuffle(deckCardReferences);
                }

                var firstDraw = cardLimit + 1;
                handCardReferences = deckCardReferences.Take(firstDraw).ToList();
                deckCardReferences = deckCardReferences.Skip(firstDraw).ToList();
            }
        }

        public Area FindAreaByName(string name)
        {
            return areas.FirstOrDefault(area => area.name == name);
        }

        public void AddUserLog(string message)
        {
            userLogs.Insert(0, message); // TODO: Temp Hack to simplify data binding
            // userLogs.Add(message);
        }

        public void MoveCard(List<CardReference> from, List<CardReference> to, string cardId)
        {
            var cardReference = from.FirstOrDefault(r => r.objectId == cardId);
            if (cardReference != null)
            {
                from.Remove(cardReference);
                to.Add(cardReference);
            }
            else
            {
                ServiceLocator.Get<ILoggerService>().LogWarning($"Card {cardId} not found in deck");
            }
        }

        public void MoveCardFromDeckToHand(string cardId) => MoveCard(deckCardReferences, handCardReferences, cardId);
        public void MoveCardFromHandToDiscard(string cardId) => MoveCard(handCardReferences, discardCardReferences, cardId);

        public void PlayCardForActionPoint(Card card)
        {
            if (availableCardPlay < 1)
            {
                ServiceLocator.Get<IUserMessageService>().ShowMessage("Available Cards Play should be > 0 to play a card");
                return;
            }

            availableCardPlay -= 1;
            MoveCardFromHandToDiscard(card.objectId);

            availableActionPoints += card.cardClass.actionPoints;
        }

        public void PlayCardForEventEffect(Card card)
        {
            if (availableCardPlay < 1)
            {
                ServiceLocator.Get<IUserMessageService>().ShowMessage("Available Cards Play should be > 0 to play a card");
                return;
            }

            availableCardPlay -= 1;
            MoveCardFromHandToDiscard(card.objectId);

            card.cardClass.ApplyEventEffect();
        }

        public bool IsNextPhaseValid(out string message)
        {
            message = "";

            if (phase == GamePhase.DiscardCard)
            {
                message = "Card exceeds limit";
                return handCardReferences.Count <= cardLimit;
            }

            return true;
        }

        public void NextPhase()
        {
            if (phase == GamePhase.GameEnd)
            {
                Prompt("Game is over.");
                return;
            }

            if (phase == GamePhase.PlayingCard)
            {
                availableCardPlay = 0;

                if (handCardReferences.Count <= cardLimit)
                    phase = GamePhase.DoingAction;
                else
                    phase = GamePhase.DiscardCard;
            }
            else if (phase == GamePhase.DiscardCard)
            {
                while (handCardReferences.Count > cardLimit)
                {
                    var randomCard = RandomUtils.Sample(handCardReferences);
                    handCardReferences.Remove(randomCard);
                    discardCardReferences.Add(randomCard);
                }

                phase = GamePhase.DoingAction;
            }
            else if (phase == GamePhase.DoingAction)
            {
                TurnEndHousekeeping();

                availableActionPoints = 0;
                currentYear += turnLengthYear;

                if (currentYear > endYear)
                {
                    phase = GamePhase.GameEnd;
                    return;
                }

                phase = GamePhase.PlayingCard;

                TurnBeginHouseKeeping();
            }
        }

        public void ProcessGrowth(Area area)
        {
            // Reinvestment
            area.hostResources += RollForResourceDelta(area.hostResources, 0.05f);
            area.vikingResources += RollForResourceDelta(area.vikingResources, 0.05f);

            // Fixed Growth
            if (area.isColony)
            {
                if (area.vikingZoneCreated)
                {
                    area.vikingResources += RandomUtils.RandomRound(0.5f);
                }
            }
            else if (area.isVikingHomeland)
            {
                area.vikingResources += 1;
            }
            else if (!area.vikingZoneCreated)
            {
                area.hostResources += 1;
            }
            else
            {
                area.hostResources += RandomUtils.RandomRound(1 - area.vikingOccupyingPercent);
                area.vikingResources += RandomUtils.RandomRound(area.vikingOccupyingPercent);
            }

            // "External" affiars: 10% => -50% Host Sources
            if (RandomUtils.NextFloat() < 0.1f)
            {
                area.hostResources = RandomUtils.RandomRound(area.hostResources / 2f);
            }
            
            // area.hostResources = Math.Clamp(area.hostResources, 0, area.baseMaxResources);
            // area.vikingResources = Math.Clamp(area.vikingResources, 0, area.baseMaxResources);
        }

        public void ProcessCounterAttackWar(Area area, ref List<string> summary)
        {
            if (area.vikingZoneCreated && area.hostResources * 2 >= area.vikingResources)
            {
                if (RandomUtils.D100F() <= 50 && RandomUtils.NextFloat() > area.vikingChristianization)
                {
                    var attackStrength = area.hostResources;
                    var defStrength = area.vikingResources;
                    var attackPower = RandomUtils.RandomRound(attackStrength * RandomUtils.NextFloat());
                    var defPower = RandomUtils.RandomRound(defStrength * RandomUtils.NextFloat());

                    var summaryHead = $"{area.name} Counter Attack War => Host: {attackStrength} -> {attackPower}, Viking: {defStrength} -> {defPower}";
                    string summaryBody;

                    var baseLoss = Math.Min(attackPower, defPower);
                    if (attackPower > defPower)
                    {
                        var vikingDelta = -baseLoss;
                        var hostDelta = -RandomUtils.RandomRound(baseLoss / 2f);

                        area.vikingResources += vikingDelta;
                        area.hostResources += hostDelta;

                        var occupyDelta = -RandomUtils.NextFloat() * Math.Min(area.vikingOccupyingPercent * 1.5f, 0.75f);
                        area.vikingOccupyingPercent += occupyDelta;

                        summaryBody = $" Host Victory => Host: ({hostDelta}), Viking: ({vikingDelta}), Occupy: {occupyDelta:P0}";
                    }
                    else
                    {
                        var vikingDelta = -RandomUtils.RandomRound(baseLoss / 2f);
                        var hostDelta = -baseLoss;

                        area.vikingResources += vikingDelta;
                        area.hostResources += hostDelta;

                        summaryBody = $" Host Defeat => Host: ({hostDelta}), Viking: ({vikingDelta})";
                    }

                    if (area.vikingOccupyingPercent == 0)
                    {
                        area.vikingChristianization = 0;
                        area.vikingResources = 0;
                        summary.Add("Viking Zone Destroyed");
                    }

                    summary.Add(summaryHead + summaryBody);
                }
            }
        }

        public void TurnEndHousekeeping()
        {
            lordSetPoint = 0;
            isFreeTransfer = false;

            List<string> turnEndSummary = new() { $"Turn Year {currentYear} Housekeeping:" };

            // Reinvestment, counter attack war, christianization, VP acc and etc.
            foreach (var area in areas)
            {
                ProcessGrowth(area);

                ProcessCounterAttackWar(area, ref turnEndSummary);

                // Christianization Update
                if (area.vikingZoneCreated)
                {
                    var christianizationDelta = (float)area.vikingResources / area.baseMaxResources * 0.1f;
                    area.vikingChristianization = Math.Clamp(area.vikingChristianization + christianizationDelta, 0, 1);
                }
            }

            var vpAddedSum = 0;

            foreach (var area in areas)
            {
                // VP Acc
                if (area.vikingZoneCreated)
                {
                    var vpAdded = 0;

                    if (area.isVikingHomeland)
                    {
                        vpAdded = RandomUtils.RandomRound(5 * (1 - area.vikingChristianization));
                    }
                    else if (area.isColony)
                    {
                        vpAdded = RandomUtils.RandomRound(3 * (1 - area.vikingChristianization));
                    }
                    else
                    {
                        vpAdded = RandomUtils.RandomRound(5 * (1 - area.vikingChristianization) * (0.5f + area.vikingOccupyingPercent * 0.5f));
                    }

                    vpAddedSum += vpAdded;
                    turnEndSummary.Add($"{area.name} + {vpAdded} VP");
                }
            }

            victoryPoint += vpAddedSum;

            turnEndSummary.Add($"Area VP +{vpAddedSum}");

            Prompt(string.Join("\n", turnEndSummary));
        }

        public int RollForResourceDelta(int resource, float prob)
        {
            var ret = 0;
            for (int i = 0; i < resource; i++)
            {
                if (RandomUtils.D100F() <= prob * 100)
                {
                    ret += 1;
                }
            }
            return ret;
        }

        public void TurnBeginHouseKeeping()
        {
            // Draw cards
            if (deckCardReferences.Count > 0)
            {
                var topCard = deckCardReferences[0];
                deckCardReferences.RemoveAt(0);
                handCardReferences.Add(topCard);
            }

            availableCardPlay = 1;
        }

        public bool CheckGenericActionCondition(ResolutionContext ctx = null)
        {
            if (availableActionPoints == 0 && !(ctx?.skipActionPoint ?? false))
            {
                Prompt("No action point left");
                return false;
            }

            if (phase != GamePhase.DoingAction && !(ctx?.skipPhaseCheck ?? false))
            {
                Prompt("Action can only do in the action phase");
                return false;
            }

            return true;
        }

        public void DoCounterInfluence(Area area)
        {
            if (!CheckGenericActionCondition())
                return;

            availableActionPoints -= 1;

            var newValue = Math.Clamp(area.vikingChristianization - RandomUtils.D100F() * 0.01f, 0, 1);
            Prompt($"Counter Influence: {area.vikingChristianization} -> {newValue}");
            area.vikingChristianization = newValue;
        }

        public void DoTrade(FromToAreaReferenceParameter p, ResolutionContext ctx = null)
        {
            if (!CheckGenericActionCondition())
                return;

            if (!CheckDistanceLimit(p, 3, ctx))
                return;

            availableActionPoints -= 1;

            var fromArea = p.from.GetArea();
            var toArea = p.to.GetArea();

            if (toArea.hostResources == 0)
            {
                ServiceLocator.Get<IUserMessageService>().ShowMessage("Target host area should have at least 1 resource to do trade");
                // TODO: SUpport trade between viking area? (E.X. Colonization?)
                return;
            }

            // var add = Math.Max(1, Math.Min(fromArea.vikingResources, toArea.hostResources) / 10);
            var add = RandomUtils.D6() + Math.Min(fromArea.vikingResources, toArea.hostResources) / 10;

            fromArea.vikingResources = Math.Clamp(fromArea.vikingResources + add, 0, fromArea.baseMaxResources);
            toArea.hostResources = Math.Clamp(toArea.hostResources + add, 0, toArea.baseMaxResources);

            Prompt($"Trade: {fromArea.name} +{add}, {toArea.name} +{add}");
        }

        public void DoTransfer(ResourceAssignParameter p)
        {
            if (!isFreeTransfer && !CheckGenericActionCondition())
                return;

            var fromArea = p.from.GetArea();
            var toArea = p.to.GetArea();

            if (!fromArea.IsRelatedTo(toArea))
            {
                Prompt("Can't transfer resource between areas which don't have any lord/vassal relationship.");
                return;
            }

            if (!toArea.vikingZoneCreated)
            {
                Prompt("Can't transfer to an area which viking zone is not created yet");
                return;
            }

            if (!isFreeTransfer)
                availableActionPoints -= 1;

            fromArea.vikingResources -= p.assignResource;
            toArea.vikingResources += p.assignResource;
        }

        public void DoConquest(ResourceAssignParameter p, ResolutionContext ctx = null)
        {
            if (!CheckGenericActionCondition(ctx))
                return;

            if (!CheckDistanceLimit(p, 1, ctx))
                return;

            var fromArea = p.from.GetArea();
            var toArea = p.to.GetArea();

            if (toArea.vikingZoneCreated && toArea != fromArea)
            {
                Prompt("If Viking zone had been created, conquest can only launched from the same zone.");
                return;
            }

            if (toArea.isVikingHomeland)
            {
                Prompt("Cannot conquer viking homeland");
                return;
            }

            if (fromArea.GetConquerAssignedResourceLimit() < p.assignResource)
            {
                Prompt("Invalid resource assigned to conquer");
                return;
            }

            if(!(ctx?.skipActionPoint ?? false))
                availableActionPoints -= 1;

            var defStrength = RandomUtils.RandomRound(toArea.hostResources * 0.5f);
            var attackStrength = RandomUtils.RandomRound(p.assignResource);
            var defPower = RandomUtils.RandomRound(defStrength * RandomUtils.NextFloat());
            var attackPower = RandomUtils.RandomRound(attackStrength * RandomUtils.NextFloat() * p.modifierCoef);

            var baseLoss = Math.Min(defStrength, attackStrength);

            if (attackPower >= defPower * 2)
            {
                // var vikingFromDelta = -RandomUtils.RandomRound(baseLoss / 2f);
                var vikingFromDelta = -p.assignResource;
                var vikingToDelta = Math.Max(RandomUtils.RandomRound(p.assignResource - baseLoss / 2), 1);
                var hostDelta = -baseLoss;

                fromArea.vikingResources += vikingFromDelta;
                toArea.vikingResources += vikingToDelta;
                toArea.hostResources += hostDelta;
                victoryPoint += -hostDelta;

                var occupyDelta = Math.Min(RandomUtils.NextFloat() * 0.25f, 1 - toArea.vikingOccupyingPercent);
                toArea.vikingOccupyingPercent += occupyDelta;

                toArea.lord.objectId = fromArea.objectId;

                Prompt($"Attacker Pwr ({attackStrength} => {attackPower})  vs Def Pwr ({defStrength} => {defPower}) => Critical Success => Attacker: ({vikingFromDelta}, {vikingToDelta}), Host: {hostDelta} Resources, VP: {-hostDelta}, Occupy: {occupyDelta:P0}");
            }
            else if (attackPower >= defPower)
            {
                // var vikingFromDelta = -baseLoss;
                var vikingFromDelta = -p.assignResource;
                var vikingToDelta = Math.Max(p.assignResource - baseLoss, 1);
                var hostDelta = -baseLoss;

                fromArea.vikingResources += vikingFromDelta;
                toArea.vikingResources += vikingToDelta;
                toArea.hostResources += hostDelta;
                victoryPoint += -hostDelta;

                var occupyDelta = Math.Min(RandomUtils.NextFloat() * 0.25f, 1 - toArea.vikingOccupyingPercent);
                toArea.vikingOccupyingPercent += occupyDelta;

                toArea.lord.objectId = fromArea.objectId;

                Prompt($"Attacker Pwr ({attackStrength} => {attackPower})  vs Def Pwr ({defStrength} => {defPower}) => Success => Attacker: ({vikingFromDelta}, {vikingToDelta}), Host: {hostDelta} Resources, VP: {-hostDelta}, Occupy: {occupyDelta:P0}");
            }
            else
            {
                var vikingDelta = -baseLoss;
                var hostDelta = -RandomUtils.RandomRound(baseLoss / 2f);

                fromArea.vikingResources += vikingDelta;
                toArea.hostResources += hostDelta;
                victoryPoint += -hostDelta;

                Prompt($"Attacker Pwr ({attackStrength} => {attackPower})  vs Def Pwr ({defStrength} => {defPower}) => Failure => Attacker: {vikingDelta}, Host: {hostDelta} Resources, VP: {-hostDelta}");
            }
        }

        public class ResolutionContext
        {
            public bool skipActionPoint;
            public bool skipPhaseCheck;
            public bool skipDistanceCheck;
        }

        // static ResolutionContext defaultCtx = new();

        public bool CheckDistanceLimit(FromToAreaReferenceParameter p, int limit, ResolutionContext ctx = null)
        {
            if (ctx?.skipDistanceCheck ?? false)
                return true;
            var dist = p.GetDistance();
            var passed = dist <= limit;
            if (!passed)
            {
                Prompt($"Distance limit exceeded: {dist} > {limit}");
            }
            return passed;
        }

        public void DoRaid(ResourceAssignParameter p, ResolutionContext ctx = null)
        {
            if (!CheckGenericActionCondition(ctx))
                return;

            if (!CheckDistanceLimit(p, 2, ctx))
                return;

            var fromArea = p.from.GetArea();
            var toArea = p.to.GetArea();

            if (fromArea.GetRaidAssignedResourceLimit() < p.assignResource)
            {
                Prompt("Invalid resource assigned to raid");
                return;
            }

            if (toArea.isVikingHomeland)
            {
                Prompt("Cannot raid viking homeland");
                return;
            }

            if (!(ctx?.skipActionPoint ?? false))
            {
                availableActionPoints -= 1;
            }

            var defStrength = RandomUtils.RandomRound(toArea.hostResources * 0.2f);
            var raidStrength = RandomUtils.RandomRound(p.assignResource);
            var defPower = RandomUtils.RandomRound(defStrength * RandomUtils.NextFloat());
            var raidPower = RandomUtils.RandomRound(raidStrength * RandomUtils.NextFloat() * p.modifierCoef);

            if (raidPower >= defPower * 2)
            {
                var transfer = Math.Min(RandomUtils.RandomRound(p.assignResource * 2 * p.modifierCoef), toArea.hostResources);
                var vikingDelta = transfer - p.assignResource;
                var hostDelta = -transfer;

                fromArea.vikingResources += vikingDelta;
                toArea.hostResources += hostDelta;
                victoryPoint += transfer;

                toArea.lord.objectId = fromArea.objectId;

                Prompt($"Raider Pwr ({raidStrength} => {raidPower})  vs Def Pwr ({defStrength} => {defPower}) => Critical Success => Raider: {vikingDelta}, Host: {hostDelta} Resources, VP: {transfer}");
            }
            else if (raidPower >= defPower)
            {
                var transfer = Math.Min(RandomUtils.RandomRound(p.assignResource * 1 * p.modifierCoef), toArea.hostResources);
                var vikingDelta = transfer - p.assignResource;
                var hostDelta = -transfer;

                fromArea.vikingResources += vikingDelta;
                toArea.hostResources += hostDelta;
                victoryPoint += transfer;

                toArea.lord.objectId = fromArea.objectId;

                Prompt($"Raider Pwr ({raidStrength} => {raidPower})  vs Def Pwr ({defStrength} => {defPower}) => Success => Raider: {vikingDelta}, Host: {hostDelta} Resources, VP: {transfer}");
            }
            else
            {
                fromArea.vikingResources -= p.assignResource;
                Prompt($"Raider Pwr ({raidStrength} => {raidPower})  vs Def Pwr ({defStrength} => {defPower}) => Failure => Raider -{p.assignResource} Resources");
            }
        }

        void Prompt(string message) => ServiceLocator.Get<IUserMessageService>().ShowMessage(message);

        public void DoColonization(FromToAreaReferenceParameter p, ResolutionContext ctx = null)
        {
            if (!CheckGenericActionCondition())
                return;

            if (!CheckDistanceLimit(p, 1, ctx))
                return;

            if (!p.to.GetArea().isColony)
            {
                Prompt("This area is not a colonizable (only 4 areas in the west-north are colonizable)");
                return;
            }

            var fromArea = p.from.GetArea();
            var toArea = p.to.GetArea();

            var transfer = 2;

            if (fromArea.vikingResources < transfer)
            {
                Prompt("Source area should have at least 2 resource to do Colonization");
                // TODO: SUpport trade between viking area? (E.X. Colonization?)
                return;
            }

            availableActionPoints -= 1;

            fromArea.vikingResources -= transfer;
            toArea.vikingResources += transfer;
            // toArea.vikingZoneCreated = true;

            if (toArea.lord.objectId == null || toArea.lord.objectId == "")
            {
                toArea.lord.objectId = fromArea.objectId;
            }

            Prompt($"Colonization: {fromArea.name} -{transfer}, {toArea.name} +{transfer}");
        }

        public bool IsPlayingCardForActionPointAvaliable(Card card) => phase == GamePhase.PlayingCard;
        public bool IsPlayingCardForEventEffectAvailable(Card card)
        {
            if (phase != GamePhase.PlayingCard)
                return false;
            if (card == null)
                return false;
            if (card.cardClass.eventCode == CardClassEventCode.None)
                return false;
            if (CardClass.eventActivatedConditionMap.TryGetValue(card.cardClass.eventCode, out var condition))
            {
                if (!condition())
                    return false;
            }
            return true;
        }

        public bool IsTradeAvailable(Area area) => phase == GamePhase.DoingAction;
        public bool IsCounterInfluenceAvailable(Area area) => phase == GamePhase.DoingAction;
        public bool IsColonizationAvailable(Area area) => phase == GamePhase.DoingAction;
        public bool IsRaidAvailable(Area area) => phase == GamePhase.DoingAction;
        public bool IsConquerAvailable(Area area) => phase == GamePhase.DoingAction;
        public bool IsTransferAvailable(Area area) => phase == GamePhase.DoingAction;

        public GameState DetachCardStates()
        {
            var detached = XmlUtils.FromXML<GameState>(XmlUtils.ToXML(this));
            detached.cards = new();
            detached.deckCardReferences = new();
            detached.handCardReferences = new();
            detached.discardCardReferences = new();
            detached.cardHasSetup = false;
            detached.reshuffled = false;
            return detached;
        }

        public string GetEndGameSummary()
        {
            var vikingZoneMaintained = areas.Where(x => x.vikingZoneCreated).Count();

            var lines = new List<string>()
            {
                $"Victory Points: {victoryPoint}",
                $"Viking Zone Maintained: {vikingZoneMaintained}"
            };
            return string.Join("\n", lines);
        }

        public static GameState current => CoreManager.Instance.state;

        public void ResetAndRegisterAll()
        {
            EntityManager.current.Reset();

            foreach (var area in areas)
                EntityManager.current.Register(area, null);

            foreach (var card in cards)
                EntityManager.current.Register(card, null);
        }
    }
}