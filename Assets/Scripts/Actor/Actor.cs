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

        //var info1 = Anim.GetCurrentAnimatorStateInfo(0);
        //Debug.LogError(info1.fullPathHash);
        //if (newState != info1.fullPathHash)
        //{
        //    animator.Play(newState, -1, 0f);
        //    var info2 = animator.GetCurrentAnimatorStateInfo(0);
        //    if (info1.nameHash == info2.nameHash)
        //    {
        //        Debug.LogWarning("State not changed");
        //    }
        //}
    }

    public void PlayAnimation(AnimationActor eActiveAni)
    {
        //Anim.SetBool("Idle", false);
        //Anim.SetBool("Atk", false);
        //Anim.SetBool("Cnt", false);
        //Anim.SetBool("Defeat", false);
        //Anim.SetBool("Fake", false);

        //Anim.SetBool(ClipName[(int)eActiveAni], true);
        Anim.Play(ClipName[(int)eActiveAni], -1, 0f);
    }

    public void SetAnimationSpeed(AnimationActor eActiveAni, float fSeepd = 1.0f)
    {
        Anim.speed = fSeepd;
    }
}
