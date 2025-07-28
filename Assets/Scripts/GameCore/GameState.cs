using System.Collections.Generic;
using Unity.VisualScripting;

namespace GameCore
{
    public enum GamePhase
    {
        PlayingCard,
        DoingAction,
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

        public GamePhase phase;

        public int availableActionPoints;
        public int victroyPoint;

        public List<Area> areas = new();
        public List<Card> cards = new();

        public List<CardReference> deckCardReferences = new();
        public List<CardReference> handCardReferences = new();
        public List<CardReference> discardCardReferences = new();

        public void NextPhase()
        {

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