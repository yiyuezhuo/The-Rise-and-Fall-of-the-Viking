using System.Collections.Generic;

namespace GameCore
{
    public class Area : IObjectIdLabeled
    {
        public string objectId { get; set; }

        public string name;
        public bool vikingZoneCreated;
        public bool isVikingHomeland;
        public int vikingResources;
        public float vikingOccupyingPercent;
        public int hostResources;
        public float vikingChristianization;
        public float christianizationCoef = 1f;
        public int baseMaxResources = 50;
        public bool isColony;
        public string lord;

        public IEnumerable<IObjectIdLabeled> GetSubObjects()
        {
            yield break;
        }
    }
}