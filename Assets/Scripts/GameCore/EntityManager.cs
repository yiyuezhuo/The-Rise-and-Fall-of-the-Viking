using System;
using System.Collections;
using System.Collections.Generic;


namespace GameCore
{
    public interface IObjectIdLabeled
    {
        string objectId { get; set; }
        IEnumerable<IObjectIdLabeled> GetSubObjects();
        // IEnumerable<IObjectIdLabeled> GetSubObjects()
        // {
        //     yield break;
        // }
    }

    public partial class EntityManager
    {
        public Dictionary<string, IObjectIdLabeled> idToEntity = new();
        public Dictionary<IObjectIdLabeled, object> entityToParent = new();

        // public event EventHandler<string> newGuidCreated;

        public void Reset()
        {
            idToEntity.Clear();
            entityToParent.Clear();
        }

        public void Register(IObjectIdLabeled obj, object parent)
        {
            if (obj.objectId == null || idToEntity.ContainsKey(obj.objectId))
            {
                do
                {
                    obj.objectId = System.Guid.NewGuid().ToString();
                } while (idToEntity.ContainsKey(obj.objectId));
                //  newGuidCreated?.Invoke(obj, obj.objectId);
                ServiceLocator.Get<ILoggerService>().LogWarning($"New guid created: {obj.objectId} for {obj}");
            }
            idToEntity[obj.objectId] = obj;
            entityToParent[obj] = parent;

            foreach (var subObj in obj.GetSubObjects())
            {
                Register(subObj, obj);
            }
        }

        public void Unregister(IObjectIdLabeled obj)
        {
            foreach (var subObj in obj.GetSubObjects())
            {
                Unregister(subObj);
            }

            idToEntity.Remove(obj.objectId);
            entityToParent.Remove(obj);
        }

        public T Get<T>(string id) where T : class
        {
            if (id == null)
                return null;
            return idToEntity.GetValueOrDefault(id) as T;
        }

        public T GetParent<T>(IObjectIdLabeled obj) where T : class
        {
            if (obj == null)
                return null;
            return entityToParent.GetValueOrDefault(obj) as T;
        }

        // Move to partial method
        // public ShipLog GetOnMapShipLog(string id)
        // {
        //     var shipLog = Get<ShipLog>(id);
        //     if (shipLog == null || !shipLog.IsOnMap())
        //         return null;
        //     return shipLog;
        // }

        public static EntityManager current => CoreManager.Instance.entityManager;
    }

}