using UnityEngine;

namespace ZeroX.WindowSystem
{
    [System.Serializable]
    public class WindowSortingConfig
    {
        [SerializeField] private bool useWindowSorting = true;
        [SerializeField] private WindowSortingLayerId windowSortingLayerId = WindowSortingLayerId.Popup;
        [SerializeField] private bool visibleWhenBehind = false;

        
        
        public bool UseWindowSorting => useWindowSorting;

        public WindowSortingLayerId WindowSortingLayerId => windowSortingLayerId;
        
        public bool VisibleWhenBehind => visibleWhenBehind;
    }
}