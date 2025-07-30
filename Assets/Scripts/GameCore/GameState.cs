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

        public List<Area> areas = new();
        public List<Card> cards = new();

        public List<CardReference> deckCardReferences = new();
        public List<CardReference> handCardReferences = new();
        public List<CardReference> discardCardReferences = new();

        public List<string> userLogs = new();

        // Conquer
        // Raid
        // Trade
        // Colonization
        // Counter Influence

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

        public void TurnEndHousekeeping()
        {
            // Reinvestment, counter attack war, christianization, VP acc and etc.
            foreach (var area in areas)
            {
                // Reinvestment
                area.hostResources = Math.Clamp(area.hostResources + RollForResourceDelta(area.hostResources, 0.05f), 0, area.baseMaxResources);
                area.vikingResources = Math.Clamp(area.vikingResources + RollForResourceDelta(area.vikingResources, 0.05f), 0, area.baseMaxResources);

                // TODO: Counter Attack War

                // Christianization Update
                if (area.vikingZoneCreated)
                {
                    var christianizationDelta = (float)area.vikingResources / area.baseMaxResources * 0.1f;
                    area.vikingChristianization = Math.Clamp(area.vikingChristianization + christianizationDelta, 0, 1);
                }

                // VP Acc
                if (area.vikingZoneCreated)
                {
                    victoryPoint += (int)Math.Round(5 * (1 - area.vikingChristianization));
                }
            }

            AddUserLog($"{currentYear} Turn End");
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

        public void DoCounterInfluence(Area area)
        {
            if (availableActionPoints == 0)
            {
                ServiceLocator.Get<IUserMessageService>().ShowMessage("No action point left");
                return;
            }

            availableActionPoints -= 1;

            var newValue = Math.Clamp(area.vikingChristianization - RandomUtils.D100F() * 0.01f, 0, 1);
            AddUserLog($"Counter Influence: {area.vikingChristianization} -> {newValue}");
            area.vikingChristianization = newValue;
        }

        public void DoTrade(FromToAreaReferenceParameter p)
        {
            if (availableActionPoints == 0)
            {
                ServiceLocator.Get<IUserMessageService>().ShowMessage("No action point left");
                return;
            }

            var fromArea = p.from.GetArea();
            var toArea = p.to.GetArea();

            if (toArea.hostResources == 0)
            {
                ServiceLocator.Get<IUserMessageService>().ShowMessage("Target host area should have at least 1 resource to do trade");
                // TODO: SUpport trade between viking area? (E.X. Colonization?)
                return;
            }

            availableActionPoints -= 1;

            var add = Math.Max(1, Math.Min(fromArea.vikingResources, toArea.hostResources) / 10);

            fromArea.vikingResources = Math.Clamp(fromArea.vikingResources + add, 0, fromArea.baseMaxResources);
            toArea.hostResources = Math.Clamp(toArea.hostResources + add, 0, toArea.baseMaxResources);

            AddUserLog($"Trade: {fromArea.name} +{add}, {toArea.name} +{add}");
        }

        public void DoConquer(ResourceAssignParameter p)
        {
            // TODO
        }

        public void DoRaid(ResourceAssignParameter p)
        {
            // TODO
        }

        void Propmt(string message) => ServiceLocator.Get<IUserMessageService>().ShowMessage(message);

        public void DoColonization(FromToAreaReferenceParameter p)
        {
            if (availableActionPoints == 0)
            {
                Propmt("No action point left");
                return;
            }

            var fromArea = p.from.GetArea();
            var toArea = p.to.GetArea();

            var transfer = 2;

            if (fromArea.vikingResources < transfer)
            {
                Propmt("Source area should have at least 2 resource to do Colonization");
                // TODO: SUpport trade between viking area? (E.X. Colonization?)
                return;
            }

            availableActionPoints -= 1;

            fromArea.vikingResources -= transfer;
            toArea.vikingResources += transfer;
            // toArea.vikingZoneCreated = true;

            AddUserLog($"Trade: {fromArea.name} -{transfer}, {toArea.name} +{transfer}");
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