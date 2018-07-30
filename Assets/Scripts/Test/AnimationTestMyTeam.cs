using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTestMyTeam : MonoBehaviour
{
    public bool IsMyTeam;

    Actor Actor;
    // Use this for initialization
    IEnumerator Start ()
    {
        //var render = GetComponentInChildren<SpriteRenderer>();
        //render.flipX = IsMyTeam;
        transform.localScale = new Vector3(Define.ACTION_START_SCALE, Define.ACTION_START_SCALE, Define.ACTION_START_SCALE);

        yield return new WaitForSeconds(1);

        Actor = GetComponentInChildren<Actor>();
        if (Actor != null)
        {            
            StartCoroutine(ActionProc());
        }
	}

    
    public IEnumerator ActionProc()
    {
        yield return MoveForward(0.4f, 3, Actor.AniType.ANI_TRACE);
        yield return MoveBackward(0.2f, 1, Actor.AniType.ANI_BREAK);
        yield return MoveBackward(0.2f, 2, Actor.AniType.ANI_DEFEAT);
        yield return AnimationDeley(0.2f, Actor.AniType.ANI_DEFEAT);

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
