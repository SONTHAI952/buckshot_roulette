using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    public class GlobalWindowTransitionTable : ScriptableObject
    {
        [SerializeField] private List<GlobalWindowTransitionRow> listRow = new List<GlobalWindowTransitionRow>();

        
        
        
        //Temp
        private static GlobalWindowTransitionTable instance;
        private Dictionary<GlobalWindowTransitionId, GlobalWindowTransitionRow> dictRow;
        private static WindowTransitionConfig defaultTransitionConfig = WindowTransitionConfig.CreateDefault();

        //Property
        public List<GlobalWindowTransitionRow> RawRowsI => listRow;
        
        
        
        public static GlobalWindowTransitionTable Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<GlobalWindowTransitionTable>(WindowConst.ResourcesPath.GlobalWindowTransitionTable);
                    if(instance == null)
                        Debug.LogError("GlobalWindowTransitionTable not exist at path: " + WindowConst.ResourcesPath.GlobalWindowTransitionTable);
                }

                return instance;
            }
        }

        public static GlobalWindowTransitionTable GetInstanceWithoutLog()
        {
            if (instance == null)
                instance = Resources.Load<GlobalWindowTransitionTable>(WindowConst.ResourcesPath.GlobalWindowTransitionTable);

            return instance;
        }


        private void InitDictRowIfNot()
        {
            if(dictRow != null)
                return;

            dictRow = new Dictionary<GlobalWindowTransitionId, GlobalWindowTransitionRow>();
            foreach (var row in listRow)
            {
                if (dictRow.TryAdd(row.id, row) == false)
                {
                    Debug.LogError("GlobalWindowTransitionTable duplicate id: " + row.id);
                }
            }
        }

        public static GlobalWindowTransitionRow GetRowById(GlobalWindowTransitionId id)
        {
            var tempInstance = Instance;
            tempInstance.InitDictRowIfNot();

            if (tempInstance.dictRow.TryGetValue(id, out var row) == false)
            {
                Debug.LogError("GlobalWindowTransitionTable not contains row with id: " + id);
            }

            return row;
        }

        /// <summary>
        /// Khoong bao giờ null, vì nếu không có thì sẽ trả về default
        /// </summary>
        public static WindowTransitionConfig GetConfig(GlobalWindowTransitionId id)
        {
            var row = GetRowById(id);
            return row == null ? defaultTransitionConfig : row.config;
        }

        public static float GetOpenDuration(GlobalWindowTransitionId id)
        {
            return GetConfig(id).openDuration;
        }
        
        public static float GetCloseDuration(GlobalWindowTransitionId id)
        {
            return GetConfig(id).closeDuration;
        }
        
        public static bool GetIgnoreTimeScale(GlobalWindowTransitionId id)
        {
            return GetConfig(id).ignoreTimeScale;
        }
        
        public static IWindowAnim GetAnim(GlobalWindowTransitionId id)
        {
            return GetConfig(id).Anim;
        }
    }
}