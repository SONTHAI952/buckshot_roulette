using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.SceneTransitionerSystem
{
    public class SceneTransitionerSetting : ScriptableObject
    {
        public List<SceneTransitionerPrefabData> listPrefabData = new List<SceneTransitionerPrefabData>()
        {
            new SceneTransitionerPrefabData("default")
        };



        private static SceneTransitionerSetting instance;
        
        public static SceneTransitionerSetting Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<SceneTransitionerSetting>(SceneTransitionerConst.filePathInResources_Setting);
                    if (instance == null)
                    {
                        Debug.LogError("Scene Transitioner Setting not exist at path in resources: " + SceneTransitionerConst.filePathInResources_Setting);
                    }
                }
                
                return instance;
            }
        }

        public static bool HasInstance()
        {
            return Resources.Load<SceneTransitionerSetting>(SceneTransitionerConst.filePathInResources_Setting) != null;
        }


        public SceneTransitioner GetPrefabById(string id)
        {
            var data = listPrefabData.Find(d => d.id == id);
            if (data == null)
                return null;

            return data.prefab;
        }
    }
}