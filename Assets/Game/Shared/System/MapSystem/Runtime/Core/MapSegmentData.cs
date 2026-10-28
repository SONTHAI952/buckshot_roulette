using System.Collections.Generic;
using UnityEngine;

namespace MapSystem
{
    [CreateAssetMenu(menuName = "ZeroX/Map System/Map Segment Data")]
    public class MapSegmentData : ScriptableObject
    {
        [SerializeField] public List<MapObjectGroupSpawnData> listObjectGroupSpawnData = new List<MapObjectGroupSpawnData>();
        [SerializeField] public List<MapObjectSpawnData> listObjectSpawnData = new List<MapObjectSpawnData>();
    }
}