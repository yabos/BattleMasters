using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectListener : MonoBehaviour
{
    public void OnEffectListener(AnimationEvent myEvent)
    {
        string name = myEvent.stringParameter;
        float delay = myEvent.floatParameter;

        //StartCoroutine(PlayEffect(name, delay));
    }

    IEnumerator PlayEffect(string name, float delay)
    {
        yield return new WaitForSeconds(delay);

        var goEfc = EffectManager.Instance.GetEffect(name);
        if (goEfc != null)
        {
            Transform tCen = transform.Find("ef_Center");
            if (tCen != null)
            {
                //Transform tEffect = BattleManager.Instance.transform.Find("Effect");
                //goEfc.transform.parent = tEffect;
                ParticleSystem[] pcs = goEfc.GetComponentsInChildren<ParticleSystem>();
                if (pcs != null)
                {
                    for (int i = 0; i < pcs.Length; ++i)
                    {
                        Renderer render = pcs[i].GetComponent<Renderer>();
                        if (render != null)
                        {
                            render.sortingOrder = 1000;
                            render.sortingLayerName = "Hero";
                        }
                    }
                }

                goEfc.transform.position = tCen.position;
                goEfc.SetActive(true);

                var efcData = goEfc.GetComponent<EffectData>();
                if (efcData != null)
                {
                    yield return new WaitForSeconds(efcData.LifeTime);

                    goEfc.SetActive(false);
                }
            }
        }
    }
}
