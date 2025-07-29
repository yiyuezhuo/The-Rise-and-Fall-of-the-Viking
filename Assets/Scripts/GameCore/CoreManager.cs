using System.Collections.Generic;
using System.Linq;

namespace GameCore
{
    // The instance of CoreManager can be referenced by UITK dataSrouce.
    public class CoreManager
    {
        public GameState state = new();
        static CoreManager _instance = new();
        public static CoreManager Instance
        {
            get => _instance;
        }

        public EntityManager entityManager = new();

        public void LoadFromXml(string xml)
        {
            state = XmlUtils.FromXML<GameState>(xml);

            var forceResetCards = false;

            if (forceResetCards)
            {
                state.cards = new()
                {
                    new Card(){cardClassId="Action"},
                    new Card(){cardClassId="Action"},
                    new Card(){cardClassId="Action"},
                    new Card(){cardClassId="Action"},
                    new Card(){cardClassId="Action"},
                    new Card(){cardClassId="Action"},
                    new Card(){cardClassId="Action"},
                };

                GameState.current.ResetAndRegisterAll();

                state.deckCardReferences = state.cards.Select(card => new CardReference() { objectId = card.objectId }).ToList();

                var handCardNum = 5;
                state.handCardReferences = state.deckCardReferences.Take(handCardNum).ToList();
                state.deckCardReferences = state.deckCardReferences.Skip(handCardNum).ToList();
            }

            GameState.current.ResetAndRegisterAll();
        }
    }
}