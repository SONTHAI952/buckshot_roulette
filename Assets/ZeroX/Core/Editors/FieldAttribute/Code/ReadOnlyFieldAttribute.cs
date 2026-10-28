using System;
using UnityEngine;

namespace ZeroX
{
    /// <summary>
    /// Nếu field có nhiều attribute liên quan tới draw như TextArea thì nên đặt attribute này ở đầu
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyFieldAttribute : PropertyAttribute
    {
        
    }
}