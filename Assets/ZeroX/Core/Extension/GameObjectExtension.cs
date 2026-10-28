using UnityEngine;

namespace ZeroX.Extensions
{
    public static class GameObjectExtension
    {
        public static void SetLayerRecursive(GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                SetLayerRecursive(child.gameObject, layer);
            }
        }
    }
}