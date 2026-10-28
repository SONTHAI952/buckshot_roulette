using System;
using UnityEngine;

namespace MapSystem
{
    public class MapObjectGroup : MonoBehaviour
    {
        public long id = MapSystemConst.mapObjectGroupIdEmpty;
        
        [NonSerialized] internal MapObjectGroupSpawnData spawnData;
    }
}