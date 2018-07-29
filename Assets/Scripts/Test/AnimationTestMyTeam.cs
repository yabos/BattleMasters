using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTestMyTeam : MonoBehaviour
{
    public bool IsMyTeam;

    Actor Actor;
    // Use this for initialization
    void Start ()
    {
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
        // 튕겨내지는 모션
        yield return MoveBackward(0.3f, Define.MOVE_BACK_BREAK_SPEED_X, Actor.AniType.ANI_BREAK);

        // 처맞는 모션
        yield return MoveBackward(1.0f, Define.MOVE_BACK_DEFEAT_SPEED_X, Actor.AniType.ANI_ATK);

        Actor.PlayAnimation(Actor.AniType.ANI_IDLE);
    }

    protected IEnumerator AnimationDeley(float delay, Actor.AniType aniType)
    {
        Actor.PlayAnimation(aniType);

        yield return new WaitForSeconds(delay);
    }

    protected IEnumerator MoveForward(float duration, float speed, Actor.AniType aniType)
    {
        float ElapsedTime = 0;
        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.deltaTime;

            Vector3 vPos = transform.position;
            vPos.x += IsMyTeam ? speed : (-1 * speed);
            transform.position = vPos;

            Actor.PlayAnimation(aniType);
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator MoveForwardDistance(float duration, float dist, Actor.AniType aniType)
    {
        float ElapsedTime = 0;
        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.deltaTime;

            Vector3 vPos = transform.position;
            float posX = IsMyTeam ? ((ElapsedTime / duration) * dist) : (-1 * (ElapsedTime / duration) * dist);
            vPos.x = posX;
            transform.position = vPos;

            Actor.PlayAnimation(aniType);
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator MoveBackward(float duration, float speed, Actor.AniType aniType)
    {
        float ElapsedTime = 0;
        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.deltaTime;

            Vector3 vPos = transform.position;
            vPos.x += IsMyTeam ? (-1 * speed) : speed;
            transform.position = vPos;

            Actor.PlayAnimation(aniType);
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator MoveBackwardDistance(float duration, float dist, Actor.AniType aniType)
    {
        float ElapsedTime = 0;
        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.deltaTime;

            Vector3 vPos = transform.position;
            float posX = IsMyTeam ? (-1 * (ElapsedTime / duration) * dist) : ((ElapsedTime / duration) * dist);
            vPos.x = posX;
            transform.position = vPos;

            Actor.PlayAnimation(aniType);
            yield return new WaitForEndOfFrame();
        }
    }
}
