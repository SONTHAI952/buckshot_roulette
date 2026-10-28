using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.Extensions
{
    public static class Physics2DExtra
    {
        public static TComp OverlapCircle_Component<TComp>(Vector2 position, float radius) where TComp : Component
        {
            var collider = Physics2D.OverlapCircle(position, radius);
            var comp = collider.GetComponent<TComp>();
            return comp;
        }

        public static TComp OverlapCircleAll_Component<TComp>(Vector2 position, float radius) where TComp : Component
        {
            var colliders = Physics2D.OverlapCircleAll(position, radius);
            foreach (var collider in colliders)
            {
                var comp = collider.GetComponent<TComp>();
                if (comp != null)
                    return comp;
            }

            return null;
        }

        public static List<TComp> OverlapCircleAll_Components<TComp>(Vector2 position, float radius)
            where TComp : Component
        {
            List<TComp> listComp = new List<TComp>();

            var colliders = Physics2D.OverlapCircleAll(position, radius);
            foreach (var collider in colliders)
            {
                var comp = collider.GetComponent<TComp>();
                if (comp != null)
                    listComp.Add(comp);
            }

            return listComp;
        }
    }
}