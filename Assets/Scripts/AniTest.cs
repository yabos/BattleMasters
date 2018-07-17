using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniTest : MonoBehaviour
{
    Animator Anim;
    AnimatorStateInfo currentBaseState;
    Actor Actor;

    // Use this for initialization
    void Start ()
    {
        Anim = GetComponent<Animator>();
        Actor = GetComponent<Actor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Actor.PlayAnimation(Actor.AnimationActor.ANI_ATK);
        }

        currentBaseState = Anim.GetCurrentAnimatorStateInfo(0);
        if (currentBaseState.IsName("Atk"))
        {
            if (currentBaseState.normalizedTime > 1)
            {
                Anim.SetBool("Atk", false);
                Anim.SetBool("Idle", true);
            }
        }
    }
}
