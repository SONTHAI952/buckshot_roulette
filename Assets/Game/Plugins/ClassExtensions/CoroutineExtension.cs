using System;
using System.Collections;
using UnityEngine;
public static class CoroutineExtension
{
    public static void StopIfNotNull(this Coroutine coroutine, MonoBehaviour monoBehaviour)
    {
        if(coroutine != null)
            monoBehaviour.StopCoroutine(coroutine);
    }
}
