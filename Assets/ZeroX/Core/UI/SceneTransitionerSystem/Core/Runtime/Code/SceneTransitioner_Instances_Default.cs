using UnityEngine;

namespace ZeroX.SceneTransitionerSystem
{
    //Default sẽ đặc biệt hơn chút là nếu setting cũng không có thì dùng template
    public partial class SceneTransitioner
    {
        private const string defaultId = "default";
        
        public static SceneTransitioner Default
        {
            get
            {
                var registered = SceneTransitionerManager.GetRegistered(defaultId);
                if (registered != null)
                    return registered;


                if (NeedUseTemplateForDefault())
                {
                    var templatePrefab = Resources.Load<SceneTransitioner>(SceneTransitionerConst.filePathInResources_Template);
                    var template = Instantiate(templatePrefab);
                    Internal_SetId(template, defaultId);
                    SceneTransitionerManager.Register(template);
                    return template;
                }

                return SceneTransitionerManager.Get(defaultId);
            }
        }

        private static bool NeedUseTemplateForDefault()
        {
            if (SceneTransitionerSetting.HasInstance() == false)
                return true;

            if (SceneTransitionerSetting.Instance.GetPrefabById(defaultId) == null)
                return true;

            return false;
        }
    }
}