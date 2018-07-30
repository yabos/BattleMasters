using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTestEnemy : MonoBehaviour
{
    public bool IsMyTeam;

    Actor Actor;
    // Use this for initialization
    IEnumerator Start()
    {
        transform.localScale = new Vector3(Define.ACTION_START_SCALE, Define.ACTION_START_SCALE, Define.ACTION_START_SCALE);

        yield return new WaitForSeconds(1);
        //var render = GetComponentInChildren<SpriteRenderer>();
        //render.flipX = IsMyTeam;

        Actor = GetComponentInChildren<Actor>();
        if (Actor != null)
        {
            StartCoroutine(ActionProc());
        }
    }

    public IEnumerator ActionProc()
    {
        yield return AnimationDeley(0.3f, Actor.AniType.ANI_IDLE);      
        yield return MoveForwardMoment(1.5f, Actor.AniType.ANI_CNT);
        yield return AnimationDeley(0.2f, Actor.AniType.ANI_CNT);
        yield return MoveForwardMoment(2, Actor.AniType.ANI_ATK);
        yield return AnimationDeley(0.5f, Actor.AniType.ANI_ATK);


        Actor.PlayAnimation(Actor.AniType.ANI_IDLE);
    }

    protected IEnumerator AnimationDeley(float delay, Actor.AniType aniType)
    {
        Actor.PlayAnimation(aniType);

        yield return new WaitForSeconds(delay);
    }

    protected IEnumerator MoveForward(float duration, float dist, Actor.AniType aniType)
    {
        float ElapsedTime = 0;
        float SumX = 0;
        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.deltaTime;
            Vector3 vPos = transform.position;
            float tickX = (Time.deltaTime / duration) * dist;
            SumX += tickX;
            if (SumX >= dist)
            {
                tickX = 0;
            }

            if (IsMyTeam == false)
            {
                tickX *= -1;
            }

            vPos.x += tickX;
            transform.position = vPos;

            Actor.PlayAnimation(aniType);
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator MoveForwardMoment(float dist, Actor.AniType aniType)
    {
        if (IsMyTeam == false)
        {
            dist *= -1;
        }

        Vector3 vPos = transform.position;
        vPos.x += dist;
        transform.position = vPos;

        Actor.PlayAnimation(aniType);
        yield return new WaitForEndOfFrame();        
    }

    protected IEnumerator MoveBackward(float duration, float dist, Actor.AniType aniType)
    {
        float ElapsedTime = 0;
        float SumX = 0;
        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.deltaTime;
            Vector3 vPos = transform.position;
            float tickX = (Time.deltaTime / duration) * dist;
            SumX += tickX;
            if (SumX >= dist)
            {
                tickX = 0;
            }

            if (IsMyTeam)
            {
                tickX *= -1;
            }

            vPos.x += tickX;
            transform.position = vPos;

            Actor.PlayAnimation(aniType);
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator MoveBackwardMoment(float dist, Actor.AniType aniType)
    {
        if (IsMyTeam)
        {
            dist *= -1;
        }

        Vector3 vPos = transform.position;
        vPos.x += dist;
        transform.position = vPos;

        Actor.PlayAnimation(aniType);
        yield return new WaitForEndOfFrame();
    }
}
