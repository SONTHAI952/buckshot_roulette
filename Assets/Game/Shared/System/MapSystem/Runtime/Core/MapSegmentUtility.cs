using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MapSystem
{
    public static class MapSegmentUtility
    {
        public static void LoadFromData_WithGroup(MapSegment segment, MapSegmentData segmentData)
        {
            //Group
            var dictMapObjectGroup = SpawnListMapObjectGroupFromData(segment, segmentData);

            //Map Object
            Dictionary<long, MapObject> dictMapObject = new Dictionary<long, MapObject>();
            
            //Spawn Map Object
            foreach (var spawnData in segmentData.listObjectSpawnData)
            {
                if (spawnData.prefab == null)
                {
                    Debug.LogError("Cannot spawn mapObject because prefab is empty: " + spawnData.id);
                    continue;
                }

                var mapObject = GameObject.Instantiate(spawnData.prefab).GetComponent<MapObject>();
                dictMapObject.Add(spawnData.id, mapObject);
                
                mapObject.id = spawnData.id;
                mapObject.spawnData = spawnData;

                //Move to group
                if (spawnData.groupId == MapSystemConst.mapObjectGroupIdEmpty)
                {
                    mapObject.transform.SetParent(segment.ObjectContainer);
                }
                else
                {
                    if (dictMapObjectGroup.TryGetValue(spawnData.groupId, out var mapObjectGroup))
                        mapObject.transform.SetParent(mapObjectGroup.transform);
                    else
                        mapObject.transform.SetParent(segment.ObjectContainer);
                }
                
                //Transform
                mapObject.transform.localPosition = spawnData.position;
                mapObject.transform.localEulerAngles = spawnData.eulerAngles;
                mapObject.transform.localScale = spawnData.scale;
            }
            
            
            //Data
            foreach (var mapObject in dictMapObject.Values)
            {
                //Data
                var mapObjectData = mapObject.CreateData();
                if (mapObjectData != null)
                {
                    mapObjectData.ImportFromJson(mapObject.spawnData.dataJson);
                    mapObject.ImportFromData(mapObjectData, dictMapObject);
                }
            }
        }

        public static void LoadFromData_WithoutGroup(MapSegment segment, MapSegmentData segmentData)
        {
            //Map Object
            Dictionary<long, MapObject> dictMapObject = new Dictionary<long, MapObject>();
            
            //Spawn Map Object
            foreach (var spawnData in segmentData.listObjectSpawnData)
            {
                if (spawnData.prefab == null)
                {
                    Debug.LogError("Cannot spawn mapObject because prefab is empty: " + spawnData.id);
                    continue;
                }

                var mapObject = GameObject.Instantiate(spawnData.prefab).GetComponent<MapObject>();
                dictMapObject.Add(spawnData.id, mapObject);
                
                mapObject.id = spawnData.id;
                mapObject.spawnData = spawnData;

                //Move to group
                mapObject.transform.SetParent(segment.ObjectContainer);
                
                //Transform
                mapObject.transform.localPosition = spawnData.position;
                mapObject.transform.localEulerAngles = spawnData.eulerAngles;
                mapObject.transform.localScale = spawnData.scale;
            }
            
            
            //Data
            foreach (var mapObject in dictMapObject.Values)
            {
                //Data
                var mapObjectData = mapObject.CreateData();
                if (mapObjectData != null)
                {
                    mapObjectData.ImportFromJson(mapObject.spawnData.dataJson);
                    mapObject.ImportFromData(mapObjectData, dictMapObject);
                }
            }
        }


        #region Load

        public static void Editor_LoadFromData(MapSegment segment, MapSegmentData segmentData)
        {
#if UNITY_EDITOR

            //Group
            var dictMapObjectGroup = SpawnListMapObjectGroupFromData(segment, segmentData);

            //Map Object
            Dictionary<long, MapObject> dictMapObject = new Dictionary<long, MapObject>();
            
            //Spawn Map Object
            foreach (var spawnData in segmentData.listObjectSpawnData)
            {
                if (spawnData.prefab == null)
                {
                    Debug.LogError("Cannot spawn mapObject because prefab is empty: " + spawnData.id);
                    continue;
                }

                var mapObject = ((GameObject)PrefabUtility.InstantiatePrefab(spawnData.prefab)).GetComponent<MapObject>();
                dictMapObject.Add(spawnData.id, mapObject);
                
                mapObject.id = spawnData.id;
                mapObject.spawnData = spawnData;

                //Move to group
                if (spawnData.groupId == MapSystemConst.mapObjectGroupIdEmpty)
                {
                    mapObject.transform.SetParent(segment.ObjectContainer);
                }
                else
                {
                    if (dictMapObjectGroup.TryGetValue(spawnData.groupId, out var mapObjectGroup))
                        mapObject.transform.SetParent(mapObjectGroup.transform);
                    else
                        mapObject.transform.SetParent(segment.ObjectContainer);
                }
                
                //Transform
                mapObject.transform.localPosition = spawnData.position;
                mapObject.transform.localEulerAngles = spawnData.eulerAngles;
                mapObject.transform.localScale = spawnData.scale;
            }
            
            
            //Data
            foreach (var mapObject in dictMapObject.Values)
            {
                //Data
                var mapObjectData = mapObject.CreateData();
                if (mapObjectData != null)
                {
                    mapObjectData.ImportFromJson(mapObject.spawnData.dataJson);
                    mapObject.ImportFromData(mapObjectData, dictMapObject);
                }
            }
#endif
        }


        private static Dictionary<long, MapObjectGroup> SpawnListMapObjectGroupFromData(MapSegment segment, MapSegmentData segmentData)
        {
            Dictionary<long, MapObjectGroup> dictMapObjectGroup = new Dictionary<long, MapObjectGroup>();
            
            //Spawn and set id
            foreach (var spawnData in segmentData.listObjectGroupSpawnData)
            {
                MapObjectGroup mapObjectGroup = new GameObject(spawnData.name).AddComponent<MapObjectGroup>();
                dictMapObjectGroup.Add(spawnData.id, mapObjectGroup);
                
                mapObjectGroup.id = spawnData.id;
                mapObjectGroup.spawnData = spawnData;
            }

            //Set Parent and transform
            foreach (var mapObjectGroup in dictMapObjectGroup.Values)
            {
                if (mapObjectGroup.spawnData.parentId == MapSystemConst.mapObjectGroupIdEmpty)
                {
                    mapObjectGroup.transform.SetParent(segment.transform);
                }
                else
                {
                    if (dictMapObjectGroup.TryGetValue(mapObjectGroup.spawnData.parentId, out var parent))
                    {
                        mapObjectGroup.transform.SetParent(parent.transform);
                    }
                    else
                    {
                        Debug.LogError("Cannot find parent group with id: " + mapObjectGroup.spawnData.parentId, mapObjectGroup);
                        mapObjectGroup.transform.SetParent(segment.transform);
                    }
                }
                
                mapObjectGroup.transform.localPosition = mapObjectGroup.spawnData.position;
                mapObjectGroup.transform.localEulerAngles = mapObjectGroup.spawnData.eulerAngles;
                mapObjectGroup.transform.localScale = mapObjectGroup.spawnData.scale;
            }

            return dictMapObjectGroup;
        }

        #endregion
        


        #region Save

        public static void Editor_SaveToData(MapSegment segment, MapSegmentData segmentData)
        {
#if UNITY_EDITOR
            
            //MapObjectGroup
            var listMapObjectGroup = GetListMapObjectGroup(segment.ObjectContainer);
            FillListMapObjectGroupId(listMapObjectGroup);
            SaveListMapObjectGroupToData(listMapObjectGroup, segmentData);
            
            //MapObject
            var listMapObject = GetListMapObject(segment.ObjectContainer);
            FillListMapObjectId(listMapObject);
            SaveListMapObjectToData(listMapObject, segmentData);
#endif
        }

        private static List<MapObjectGroup> GetListMapObjectGroup(Transform container)
        {
            var listMapObjectGroup = container.GetComponentsInChildren<MapObjectGroup>().ToList();
            for (int i = 0; i < listMapObjectGroup.Count; i++)
            {
                var mapGroup = listMapObjectGroup[i];
                if (mapGroup.GetComponentInParent<MapObject>() != null)
                {
                    listMapObjectGroup.RemoveAt(i);
                    i--;
                    Debug.LogError("Map group cannot be a child of map object: " + mapGroup.name, mapGroup);
                    continue;
                }
            }

            return listMapObjectGroup;
        }

        private static void FillListMapObjectGroupId(List<MapObjectGroup> listMapObjectGroup)
        {
            for (int i = 0; i < listMapObjectGroup.Count; i++)
            {
                listMapObjectGroup[i].id = i;
            }
        }
        
        private static void SaveListMapObjectGroupToData(List<MapObjectGroup> listMapObjectGroup, MapSegmentData segmentData)
        {
            segmentData.listObjectGroupSpawnData.Clear();

            foreach (var mapObjectGroup in listMapObjectGroup)
            {
                MapObjectGroupSpawnData spawnData = new MapObjectGroupSpawnData();
                segmentData.listObjectGroupSpawnData.Add(spawnData);
                
                spawnData.id = mapObjectGroup.id;
                spawnData.name = mapObjectGroup.name;
                spawnData.parentId = GetMapObjectGroupParentId(mapObjectGroup);
                
                spawnData.position = mapObjectGroup.transform.localPosition;
                spawnData.eulerAngles = mapObjectGroup.transform.eulerAngles;
                spawnData.scale = mapObjectGroup.transform.localScale;
            }
        }

        private static long GetMapObjectGroupParentId(MapObjectGroup mapObjectGroup)
        {
            MapObjectGroup mapObjectGroupParent = mapObjectGroup.transform.parent.GetComponentInParent<MapObjectGroup>();
            if (mapObjectGroupParent == null)
                return MapSystemConst.mapObjectGroupIdEmpty;
            else
                return mapObjectGroupParent.id;
        }

        
        
        
        private static List<MapObject> GetListMapObject(Transform container)
        {
            var listMapObject = container.GetComponentsInChildren<MapObject>().ToList();
            for (int i = 0; i < listMapObject.Count; i++)
            {
                var mapObject = listMapObject[i];
                if (mapObject.transform.parent.GetComponentInParent<MapObject>() != null)
                {
                    listMapObject.RemoveAt(i);
                    i--;
                    continue;
                }
            }

            return listMapObject;
        }
        
        private static void FillListMapObjectId(List<MapObject> listMapObject)
        {
            for (int i = 0; i < listMapObject.Count; i++)
            {
                listMapObject[i].id = i;
            }
        }

        private static void SaveListMapObjectToData(List<MapObject> listMapObject, MapSegmentData segmentData)
        {
            segmentData.listObjectSpawnData.Clear();

            foreach (var mapObject in listMapObject)
            {
                GameObject mapObjectPrefab = GetPrefabOfMapObject(mapObject.gameObject);
                if (mapObjectPrefab == null)
                {
                    Debug.LogError("Cannot save map object that is not prefab: " + mapObject.name, mapObject);
                    continue;
                }
                
                MapObjectSpawnData spawnData = new MapObjectSpawnData();
                segmentData.listObjectSpawnData.Add(spawnData);
                
                spawnData.id = mapObject.id;
                spawnData.groupId = GetGroupIdOfMapObject(mapObject);
                
                spawnData.position = mapObject.transform.localPosition;
                spawnData.eulerAngles = mapObject.transform.eulerAngles;
                spawnData.scale = mapObject.transform.localScale;

                spawnData.prefab = mapObjectPrefab;
                spawnData.dataJson = GetDataJsonOfMapObject(mapObject);
            }
        }

        private static long GetGroupIdOfMapObject(MapObject mapObject)
        {
            if (mapObject.transform.parent == null)
                return MapSystemConst.mapObjectGroupIdEmpty;
            
            MapObjectGroup mapObjectGroupParent = mapObject.transform.parent.GetComponentInParent<MapObjectGroup>();
            if (mapObjectGroupParent == null)
                return MapSystemConst.mapObjectGroupIdEmpty;
            else
                return mapObjectGroupParent.id;
        }

        private static GameObject GetPrefabOfMapObject(GameObject mapObject)
        {
            var mapObjectPrefab = PrefabUtility.GetCorrespondingObjectFromSource(mapObject);
            if (mapObjectPrefab == null)
                return null;

            return mapObjectPrefab;
        }

        private static string GetDataJsonOfMapObject(MapObject mapObject)
        {
            var mapObjectData = mapObject.CreateData();
            if (mapObjectData == null)
                return null;
            
            mapObject.ExportToData(mapObjectData);
            return mapObjectData.ExportToJson();
        }

        #endregion



        public static void Editor_DestroyAllObject(MapSegment segment)
        {
            List<GameObject> listChild = new List<GameObject>();
            foreach (Transform child in segment.ObjectContainer)
            {
                listChild.Add(child.gameObject);
            }

            foreach (var child in listChild)
            {
                GameObject.DestroyImmediate(child);
            }
        }
    }
}