using UnityEngine;

namespace ZeroX.DashboardSystem
{
    public abstract class DashboardDrawer : ScriptableObject
    {
        public string id;
        
        public abstract void DrawPreview();

        public abstract void Draw();
    }
}