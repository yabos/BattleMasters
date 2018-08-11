using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class UtilExtendMethod
{
    public static void Invoke(this MonoBehaviour m, Action method, float time)
    {
        m.Invoke(method.Method.Name, time);
    }
}
