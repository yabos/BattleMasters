using UnityEngine;
using System.Collections;

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
    SpriteRenderer mSR = new SpriteRenderer();

    public SpriteRenderer SR
    {
        set { mSR = value; }
        get { return mSR; }
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

        SpriteRenderer[] sr = transform.GetComponentsInChildren<SpriteRenderer>();
        if (sr != null && sr.Length > 0)
        {
            for (int i = 0; i < sr.Length; ++i)
            {
                if (sr[i].name.Equals("Shadow") == false)
                {
                    SR = sr[i];
                }
            }
        }
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
}
