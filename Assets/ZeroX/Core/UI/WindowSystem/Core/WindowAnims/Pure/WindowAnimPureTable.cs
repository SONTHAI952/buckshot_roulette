using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    public static class WindowAnimPureTable
    {
        private static readonly Dictionary<WindowAnimPureId, WindowAnimPure> dictAnim = new Dictionary<WindowAnimPureId, WindowAnimPure>()
        {
            { WindowAnimPureId.Fade, new WindowAnimPure_Fade()},
            { WindowAnimPureId.PingPong, new WindowAnimPure_PingPong()},
            { WindowAnimPureId.FadeAndScaleMirror, new WindowAnimPure_FadeAndScaleMirror()},
            { WindowAnimPureId.FadeAndScaleUp, new WindowAnimPure_FadeAndScaleUp()},
        };
        
        
        public static WindowAnimPure GetAnim(WindowAnimPureId id)
        {
            if (dictAnim.TryGetValue(id, out var anim))
                return anim;

            Debug.LogError("Not found WindowAnimPure with id: " + id);
            return null;
        }
    }
}