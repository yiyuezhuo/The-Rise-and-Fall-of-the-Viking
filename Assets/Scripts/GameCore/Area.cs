using System.Collections.Generic;

namespace GameCore
{
    public partial class AreaReference
    {
        public string objectId;
        public Area GetArea()
        {
            return CoreManager.Instance.entityManager.Get<Area>(objectId);
        }
    }

    public class Area : IObjectIdLabeled
    {
        public string objectId { get; set; }

        public string name;
        public bool vikingZoneCreated => isVikingHomeland || vikingResources > 0;
        public bool isVikingHomeland;
        public int vikingResources;
        public float vikingOccupyingPercent;
        public int hostResources;
        public float vikingChristianization;
        public float christianizationCoef = 1f;
        public int baseMaxResources = 50;
        public bool isColony;
        public AreaReference lord = new();
        public List<AreaReference> neighborReferences = new();

        public IEnumerable<IObjectIdLabeled> GetSubObjects()
        {
            yield break;
        }
    }
}