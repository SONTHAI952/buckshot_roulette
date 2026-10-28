using UnityEngine;

namespace ZeroX.WindowSystem
{
    public class WindowTemplateTable : ScriptableObject
    {
        public GameObject canvasOverlayPortrait;
        public GameObject canvasOverlayLandscape;
        
        public GameObject overlay;
        public GameObject nonOverlay;
       
        
        
        
        private static WindowTemplateTable instance;

        public static WindowTemplateTable Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<WindowTemplateTable>(WindowConst.ResourcesPath.WindowTemplateTable);
                    if (instance == null)
                    {
                        Debug.LogError("Window Template Table not found at path: " + WindowConst.ResourcesPath.WindowTemplateTable);
                    }
                }
                return instance;
            }
        }

        public static WindowTemplateTable GetInstanceWithoutLog()
        {
            if (instance == null)
                instance = Resources.Load<WindowTemplateTable>(WindowConst.ResourcesPath.WindowTemplateTable);
            
            return instance;
        }
    }
}