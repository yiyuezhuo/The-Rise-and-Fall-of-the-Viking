using System.Collections.Generic;

namespace GameCore
{
    public class Card : IObjectIdLabeled
    {
        public string objectId { get; set; }
        public string cardClassId;

        public IEnumerable<IObjectIdLabeled> GetSubObjects()
        {
            yield break;
        }
    }
}