using UnityEngine;

namespace MapSystem
{
    [System.Serializable]
    public class MapObjectSpawnData
    {
        public long id = MapSystemConst.mapObjectIdEmpty;
        public long groupId = MapSystemConst.mapObjectGroupIdEmpty;

        public Vector3 position = Vector3.zero;
        public Vector3 eulerAngles = Vector3.zero;
        public Vector3 scale = Vector3.one;
        
        public GameObject prefab;
        public string dataJson;
    }
}