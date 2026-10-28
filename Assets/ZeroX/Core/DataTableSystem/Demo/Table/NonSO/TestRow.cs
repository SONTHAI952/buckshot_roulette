using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.DataTableSystem.Demo
{
    [System.Serializable]
    public class TestRow : TestRowBase
    {
        [SerializeField] private string des3;
        [SerializeField] private Material mat;
        [SerializeField] private Sprite icon;
    }
}