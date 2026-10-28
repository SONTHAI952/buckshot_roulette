using System;
using UnityEngine;

namespace ZeroX.TabSystem
{
    public class TabInteractiveTest : TabInteractive
    {
        [Min(0)]
        [SerializeField] private float smoothTabIndex = 0;


        private void Update()
        {
            SetCurrentSmoothTabIndex(smoothTabIndex, true);
        }
    }
}