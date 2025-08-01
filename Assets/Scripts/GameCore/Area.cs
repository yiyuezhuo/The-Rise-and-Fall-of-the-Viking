using System;
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

        public IEnumerable<IObjectIdLabeled> GetSubObjects()
        {
            yield break;
        }

        // public void Clamp()
        // {
        //     vikingResources = Math.Clamp(vikingResources, 0, baseMaxResources);
        //     hostResources = Math.Clamp(hostResources, 0, 50);
        //     vikingOccupyingPercent = Math.Clamp(vikingOccupyingPercent, 0, 1);
        //     christianizationCoef = Math.Clamp(christianizationCoef, 0, 1);
        // }

        public int GetRaidAssignedResourceLimit()
        {
            if (vikingChristianization == 1)
                return 0;

            return Math.Min(
                Math.Max(
                    (int)Math.Round((1 - vikingChristianization) * 0.2f * vikingResources),
                2),
            vikingResources);
        }

        public int GetConquerAssignedResourceLimit()
        {
            if (vikingChristianization == 1)
                return 0;

            return Math.Min(
                Math.Max(
                    (int)Math.Round((1 - vikingChristianization) * 0.5f * vikingResources),
                2),
            vikingResources);
        }
    }
}