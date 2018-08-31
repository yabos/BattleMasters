using UnityEngine;
using System;
using System.Collections;

public class Singleton<T> where T : class, new()
{
    private static T instance;
    static Singleton()
    {
        if (Singleton<T>.instance == null)
        {
            instance = new T();
        }
    }
    public static T Instance
    {
        get { return instance; }
    }
    protected Singleton() { }
}