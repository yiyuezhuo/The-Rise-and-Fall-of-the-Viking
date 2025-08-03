using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

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

    public partial class PathRecord
    {
        public string toAreaObjectId;
        public string prevAreaObjectId;
        public int cost;
    }

    public partial class Area : IObjectIdLabeled
    {
        public string objectId { get; set; }

        public string name;
        public bool vikingZoneCreated => isVikingHomeland || vikingResources > 0;
        public bool isVikingHomeland;

        int _vikingResources;
        public int vikingResources
        {
            get => _vikingResources;
            set
            {
                _vikingResources = Math.Clamp(value, 0, baseMaxResources);
            }
        }

        float _vikingOccupyingPercent;
        public float vikingOccupyingPercent
        {
            get => _vikingOccupyingPercent;
            set
            {
                _vikingOccupyingPercent = Math.Clamp(value, 0, 1);
            }
        }

        int _hostResources;
        public int hostResources
        {
            get => _hostResources;
            set
            {
                _hostResources = Math.Clamp(value, 0, baseMaxResources);
            }
        }

        float _vikingChristianization;
        public float vikingChristianization
        {
            get => _vikingChristianization;
            set
            {
                _vikingChristianization = Math.Clamp(value, 0, 1);
            }
        }

        public float christianizationCoef = 1f;
        public int baseMaxResources = 50;
        public bool isColony;
        public AreaReference lord = new();
        public List<AreaReference> neighborReferences = new();
        public List<PathRecord> paths = new();

        public IEnumerable<IObjectIdLabeled> GetSubObjects()
        {
            yield break;
        }

        public void RebuildPaths()
        {
            // BFS, since all edge cost == 1
            var pathMap = new Dictionary<string, PathRecord>()
            {
                {objectId, new(){toAreaObjectId = objectId, prevAreaObjectId=null, cost=0}}
            };
            var openSet = new HashSet<string>() { objectId };

            while (openSet.Count > 0)
            {
                var newOpenSet = new HashSet<string>();
                foreach (var areaId in openSet)
                {
                    var area = EntityManager.current.Get<Area>(areaId);
                    var areaPath = pathMap[areaId];

                    foreach (var neiRef in area.neighborReferences)
                    {
                        var neiId = neiRef.objectId;
                        if (pathMap.ContainsKey(neiId))
                            continue;
                        pathMap[neiId] = new() { toAreaObjectId = neiId, prevAreaObjectId = areaId, cost = areaPath.cost + 1 };
                        newOpenSet.Add(neiId);
                    }
                }
                openSet = newOpenSet;
            }

            paths = pathMap.Values.ToList();
        }

        public int GetRaidAssignedResourceLimit()
        {
            if (vikingChristianization == 1)
                return 0;

            return Math.Min(
                Math.Max(
                    (int)Math.Round((1 - vikingChristianization) * 0.5f * vikingResources),
                2),
            vikingResources);
        }

        public int GetConquerAssignedResourceLimit()
        {
            if (vikingChristianization == 1)
                return 0;

            return Math.Min(
                Math.Max(
                    (int)Math.Round((1 - vikingChristianization) * vikingResources),
                2),
            vikingResources);
        }

        public bool IsTransitivityLordTo(Area other)
        {
            var closeSet = new HashSet<string>();
            while (other.lord.objectId != null && other.lord.objectId != "")
            {
                if (other.lord.objectId == objectId)
                    return true;

                if (closeSet.Contains(other.lord.objectId))
                    return false;
                closeSet.Add(other.objectId);

                other = EntityManager.current.Get<Area>(other.lord.objectId);
            }
            return false;
        }

        public bool IsRelatedTo(Area other)
        {
            return IsTransitivityLordTo(other) || other.IsTransitivityLordTo(this);
        }

        public void SetLord(Area other)
        {
            lord.objectId = other.objectId;
        }

        public void ReleaseFromLord()
        {
            lord.objectId = null;
        }
    }
}