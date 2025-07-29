using System.Collections.Generic;

namespace GameCore
{
    public partial class Card : IObjectIdLabeled
    {
        public string objectId { get; set; }
        public string cardClassId;
        public CardClass cardClass => CardClass.GetByCardClassId(cardClassId);

        public IEnumerable<IObjectIdLabeled> GetSubObjects()
        {
            yield break;
        }
    }
}