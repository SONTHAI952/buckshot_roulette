using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapSystem
{
    public class MapObject : MonoBehaviour
    {
        public long id = MapSystemConst.mapObjectIdEmpty;
        [NonSerialized] internal MapObjectSpawnData spawnData;
        
        public virtual MapObjectData CreateData()
        {
            return null;
        }

        public virtual void ExportToData(MapObjectData data)
        {
        }

        public virtual void ImportFromData(MapObjectData data, Dictionary<long, MapObject> dictMapObject)
        {
        }
    }
}