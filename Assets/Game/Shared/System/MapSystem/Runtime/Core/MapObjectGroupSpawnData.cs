using UnityEngine;

namespace MapSystem
{
    [System.Serializable]
    public class MapObjectGroupSpawnData
    {
        public long id = MapSystemConst.mapObjectGroupIdEmpty;
        public long parentId = MapSystemConst.mapObjectGroupIdEmpty;
        public string name;
        
        public Vector3 position = Vector3.zero;
        public Vector3 eulerAngles = Vector3.zero;
        public Vector3 scale = Vector3.one;
    }
}