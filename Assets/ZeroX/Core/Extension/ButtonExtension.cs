using UnityEngine.Events;
using UnityEngine.UI;

namespace ZeroX.Extensions
{
    public static class ButtonExtension
    {
        /// <summary>
        /// OnClick event
        /// </summary>
        public static void AddListener(this Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
        }

        /// <summary>
        /// OnClick event
        /// </summary>
        public static void RemoveListener(this Button button, UnityAction action)
        {
            button.onClick.RemoveListener(action);
        }
    }
}