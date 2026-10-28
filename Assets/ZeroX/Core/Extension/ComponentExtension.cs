using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.Extensions
{
    public static class ComponentExtension
    {
        public static T GetComponentInChildrenFirstLayer<T>(this Component component) where T : Component
        {
            foreach (Transform child in component.transform)
            {
                var comp = child.GetComponent<T>();
                if (comp != null)
                    return comp;
            }

            return null;
        }

        public static List<T> GetComponentsInChildrenFirstLayer<T>(this Component component) where T : Component
        {
            List<T> listComp = new List<T>();
            foreach (Transform child in component.transform)
            {
                var comp = child.GetComponent<T>();
                if (comp != null)
                    listComp.Add(comp);
            }

            return listComp;
        }
    }
}