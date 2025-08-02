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

            // var forceResetCards = false;
            var forceResetCards = true;

            if (forceResetCards)
            {
                state.cards = new()
                {
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

                    new Card(){cardClassId="Raid on Lindisfarne"},
                    new Card(){cardClassId="Raid on Luni"},
                    new Card(){cardClassId="Sack of Thessalonica"},
                    new Card(){cardClassId="Duchy of Normandy"},
                    new Card(){cardClassId="Norman conquest of Southern Italy"},

                    new Card(){cardClassId="Longship"},
                    new Card(){cardClassId="Longship"},
                    new Card(){cardClassId="Longship"},
                    new Card(){cardClassId="Longship"},
                    new Card(){cardClassId="Longship"},
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