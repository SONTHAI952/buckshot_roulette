using System;
using UnityEngine;
using ZeroX.ToastSystem;

namespace Game.Primary.GamePlaySystem
{
    /// <summary>
    /// Gọi ToastSingle của ZeroX nhưng không để lỗi UI làm dừng flow gameplay
    /// </summary>
    public static class GameToast
    {
        public static void Show(string message)
        {
            try
            {
                ToastSingle.Show(message);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
