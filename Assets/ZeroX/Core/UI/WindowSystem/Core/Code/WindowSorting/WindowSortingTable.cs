using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    public class WindowSortingTable : ScriptableObject
    {
        [SerializeField] private int activeWindowCapacityPerLayer = 20;
        
        [SerializeField] List<WindowSortingLayerData> listWindowSortingLayerData = new List<WindowSortingLayerData>()
        {
            new WindowSortingLayerData(WindowSortingLayerId.Base),
            
            new WindowSortingLayerData(WindowSortingLayerId.BarBehindPopup),
            new WindowSortingLayerData(WindowSortingLayerId.Popup),
            new WindowSortingLayerData(WindowSortingLayerId.BarFrontPopup),
            
            new WindowSortingLayerData(WindowSortingLayerId.SceneTransitioner),
            new WindowSortingLayerData(WindowSortingLayerId.FrontAll),
        };

        
        
        //Temp
        private Dictionary<WindowSortingLayerId, int> dictIdToIndex;
        private static WindowSortingTable instance;

        
        
        //Property
        public static WindowSortingTable Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<WindowSortingTable>(WindowConst.ResourcesPath.WindowSortingTable);
                    if (instance == null)
                    {
                        Debug.LogError("Cannot found Window Sorting Database at path: " + WindowConst.ResourcesPath.WindowSortingTable);
                    }
                }

                return instance;
            }
        }

        public static WindowSortingTable GetInstanceWithoutLog()
        {
            if (instance == null)
                instance = Resources.Load<WindowSortingTable>(WindowConst.ResourcesPath.WindowSortingTable);

            return instance;
        }

        Dictionary<WindowSortingLayerId, int> DictIdToIndex
        {
            get
            {
                if (dictIdToIndex == null)
                {
                    dictIdToIndex = new Dictionary<WindowSortingLayerId, int>();
                    for (int i = 0; i < listWindowSortingLayerData.Count; i++)
                    {
                        dictIdToIndex.Add(listWindowSortingLayerData[i].id, i);
                    }
                }

                return dictIdToIndex;
            }
        }
        
        
        
        
        
        public int GetLayerIndexById(WindowSortingLayerId windowSortingLayerId)
        {
            if (DictIdToIndex.TryGetValue(windowSortingLayerId, out var index))
                return index;
            else
            {
                Debug.LogError("Không tìm thấy index với window sorting layer id: " + windowSortingLayerId);
                return -1;
            }
        }

        public int GetStartOrderInLayer(WindowSortingLayerId windowSortingLayerId)
        {
            if (DictIdToIndex.TryGetValue(windowSortingLayerId, out var index) == false)
            {
                Debug.LogError("Không tìm thấy index với window sorting layer id: " + windowSortingLayerId);
                return index * activeWindowCapacityPerLayer;
            }

            int startOrder = 0;
            for (int i = 0; i < index; i++)
            {
                var data = listWindowSortingLayerData[i];
                startOrder += data.spacingStart;
                startOrder += activeWindowCapacityPerLayer;
            }

            return startOrder + listWindowSortingLayerData[index].spacingStart;
        }
    }
}