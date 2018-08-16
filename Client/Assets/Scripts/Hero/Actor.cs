using UnityEngine;
using System.Collections.Generic;

public class Actor : MonoBehaviour 
{
    public enum AniType
    { 
        ANI_IDLE,
        ANI_ATK,
        ANI_CNT,
        ANI_FAKE,
        ANI_DEFEAT,
        ANI_TRACE,
        ANI_BREAK,
        ANI_MAX,
    }

    public string[] ClipName = new string[]
    {
        "Idle",
        "Atk",
        "Cnt",
        "Fake",
        "Defeat",
        "Trace",
        "Break",
    };

    public Animator Anim = null;
    AniType mAniState = AniType.ANI_IDLE;

    public AnimationCurve BackStepCurve;

    public List<SpriteRenderer> ListSR
    {
        private set;
        get;
    }

    public AniType AniState
    {
        set { mAniState = value; }
        get { return mAniState; }
    }

    // Use this for initialization
    void Awake () 
    {
        Anim = transform.GetComponent<Animator>();
        ListSR = new List<SpriteRenderer>();

        SpriteRenderer[] sr = transform.GetComponentsInChildren<SpriteRenderer>();
        if (sr != null && sr.Length > 0)
        {
            for (int i = 0; i < sr.Length; ++i)
            {
                ListSR.Add(sr[i]);
            }
        }
    }

    protected virtual void Update()
    {

    }

    public void PlayAnimation(AniType eActiveAni)
    {
        if (AniState == eActiveAni) return;

        Anim.Play(ClipName[(int)eActiveAni], 0, 0f);
        AniState = eActiveAni;
    }

    public void SetAnimationSpeed(AniType eActiveAni, float fSeepd = 1.0f)
    {
        Anim.speed = fSeepd;
    }

    public AniType GetAniType(string aniType)
    {
        for (int i = 0; i < ClipName.Length; ++i)
        {
            if (ClipName[i].Equals(aniType))
            {
                return (AniType)i;
            }
        }

        Debug.LogError("Return Failed AniType!!");
        return AniType.ANI_MAX;
    }

    public string GetAniTypeClip(AniType aniType)
    {
        return ClipName[(int)aniType];
    }
}
