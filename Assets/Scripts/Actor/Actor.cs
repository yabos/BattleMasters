using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour 
{
    public enum AnimationActor
    { 
        ANI_IDLE,
        ANI_ATK,
        ANI_CNT,
        ANI_FAKE,
        ANI_DEFEAT,
        ANI_MAX,
    }

    public string[] ClipName = new string[]
    {
        "Idle",
        "Atk",
        "Cnt",
        "Fake",
        "Defeat",
    };

    public Animator Anim = null;
    AnimationActor mAniState = AnimationActor.ANI_IDLE;

    Hero_Control mHero = null;

    public AnimationActor AniState
    {
        set { mAniState = value; }
        get { return mAniState; }
    }

    // Use this for initialization
    void Start () 
    {
        Anim = transform.GetComponent<Animator>();

        mHero = transform.parent.GetComponent<Hero_Control>();
        if (mHero == null)
        {
            Debug.LogError("Class : HeroActionEvent => mHero is null");
        }
    }

    public void PlayAnimation(AnimationActor eActiveAni)
    {
        Anim.SetBool("Idle", false);
        Anim.SetBool("Atk", false);
        Anim.SetBool("Cnt", false);
        Anim.SetBool("Defeat", false);
        Anim.SetBool("Fake", false);

        Anim.SetBool(ClipName[(int)eActiveAni], true);
    }

    public void SetAnimationSpeed(AnimationActor eActiveAni, float fSeepd = 1.0f)
    {
        Anim.speed = fSeepd;
    }
}
