using System;
using UnityEngine;

namespace ZeroX
{
    [AttributeUsage(AttributeTargets.Field)]
    public class HideFieldIfRunningAttribute : PropertyAttribute
    {
    }
}