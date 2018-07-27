using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectListener : MonoBehaviour
{
    public void OnEffectListener(string name, float delay, float lifeTime)
    {

        EffectManager.Instance.GetEffect(name);
    }
}
