using System;
using System.Collections.Generic;

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

    public class GameState
    {
        public int beginYear = 793;
        public int endYear = 1066;
        public int currentYear = 793;
        public int turnLengthYear = 10;

        public int availableCardPlay = 0;
        public int cardLimit = 4;

        public GamePhase phase;

        public int availableActionPoints;
        public int victroyPoint;

        public List<Area> areas = new();
        public List<Card> cards = new();

        public List<CardReference> deckCardReferences = new();
        public List<CardReference> handCardReferences = new();
        public List<CardReference> discardCardReferences = new();

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
            // Reinvestment, counter attack war, christianization and etc.
            foreach (var area in areas)
            {
                area.hostResources = Math.Clamp(area.hostResources + RollForResourceDelta(area.hostResources, 0.05f), 0, area.baseMaxResources);
                area.vikingResources = Math.Clamp(area.vikingResources + RollForResourceDelta(area.vikingResources, 0.05f), 0, area.baseMaxResources);

                if (area.vikingZoneCreated)
                {
                    var christianizationDelta = (float)area.vikingResources / area.baseMaxResources * 0.1f;
                    area.vikingChristianization = Math.Clamp(area.vikingChristianization + christianizationDelta, 0, 1);
                }
            }
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