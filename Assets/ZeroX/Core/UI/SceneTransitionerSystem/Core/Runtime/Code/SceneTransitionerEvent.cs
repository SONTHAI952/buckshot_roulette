using System;
using UnityEngine;
using UnityEngine.Events;


namespace ZeroX.SceneTransitionerSystem
{
    public partial class SceneTransitioner
    {
        void FireEvent_OnLoadStart(string loadingTargetName)
        {
            OnLoadStartData data = new OnLoadStartData();
            data.loadingTargetName = loadingTargetName;
            
            
            
            InstanceEvents.OnLoadStart.Emit(data);
            Events.OnLoadStart.Emit(data);
        }

        void FireEvent_OnLoadEnd(string loadingTargetName)
        {
            OnLoadEndData data = new OnLoadEndData();
            data.loadingTargetName = loadingTargetName;
            
            
            
            InstanceEvents.OnLoadEnd.Emit(data);
            Events.OnLoadEnd.Emit(data);
        }

        void FireEvent_OnTransitionStart(string loadingTargetName)
        {
            OnTransitionStartData data = new OnTransitionStartData();
            data.loadingTargetName = loadingTargetName;
            
            
            
            InstanceEvents.OnTransitionStart.Emit(data);
            Events.OnTransitionStart.Emit(data);
        }
        
        void FireEvent_OnTransitionEnd(string loadingTargetName)
        {
            OnTransitionEndData data = new OnTransitionEndData();
            data.loadingTargetName = loadingTargetName;
            
            
            
            InstanceEvents.OnTransitionEnd.Emit(data);
            Events.OnTransitionEnd.Emit(data);
        }
        
        void FireEvent_OnProgressChanged(string loadingTargetName, float progress)
        {
            OnProgressChangedData data = new OnProgressChangedData();
            data.loadingTargetName = loadingTargetName;
            data.progress = progress;
            
            
            
            InstanceEvents.OnProgressChanged.Emit(data);
            Events.OnProgressChanged.Emit(data);
        }
    }
}