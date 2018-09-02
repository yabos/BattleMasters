using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAIManager
{
    public enum eAI_Proc
    {
        AI_Normal,
        AI_Action
    }

    bool ProcAI = false;
    float TimeElapsed = 0;
    eAI_Proc CurAI = eAI_Proc.AI_Normal;

    public void Initialize()
    {
        ProcAI = false;
        TimeElapsed = 0;
        CurAI = eAI_Proc.AI_Normal;
    }

    public void ProceserAI()
    {
        ProcAI = true;
        TimeElapsed = 0;
        CurAI = eAI_Proc.AI_Normal;
    }
	
	public void Update(float fTimeDelta)
    {
        if (ProcAI == false) return;

        TimeElapsed += fTimeDelta;

        switch (CurAI)
        {
            case eAI_Proc.AI_Normal:
                {
                    // 느낌 주기 위해서 3초 이후 엑션을 실행한다.
                    if (TimeElapsed >= 3)
                    {
                        // 우리팀 다 죽었을 경우 AI 안돈다.
                        var Owner = Global.SceneMgr.CurrentScene as BattleScene;
                        if (Owner != null)
                        {
                            if (BattleHeroManager.Instance.IsMyTeamAllDie())
                            {
                                CurAI = eAI_Proc.AI_Action;
                                TimeElapsed = 0;
                                return;
                            }

                            // 살아있는 상대방 적 1인 랜덤으로 선택 해줌.
                            int targetHeroNo = BattleHeroManager.Instance.GetRandomHeroTeam();
                            Owner.ActiveTargetHeroNo = targetHeroNo;
                            BattleHeroManager.Instance.SetHeroOutline(targetHeroNo);

                            var battleUI = Global.UIMgr.GetUI<UIBattle>(UIManager.eUIType.eUI_Battle);
                            if (battleUI != null)
                            {
                                battleUI.SetProfileUI(targetHeroNo, false);
                                battleUI.ActiveBattleProfile(true, true);

                                // 한가지 공격 타입을 설정
                                SetRandomActionType(Owner.ActiveTurnHeroNo);

                                // 상대방 턴일 경우에는 엑션 유형 선택은 파란색 ui 
                                battleUI.ActiveSelActionType(true, false);
                                battleUI.SetTurnTimer(Define.SELECT_ACTIONTYPE_LIMITTIME, ETurnTimeType.TURNTIME_SEL_ACTIONTYPE);
                            }

                            CurAI = eAI_Proc.AI_Action;
                            TimeElapsed = 0;
                        }
                    }
                }
                break;

            case eAI_Proc.AI_Action:
                {
                 
                }
                break;
        }
    }

    // Enemy AI
    public void SetRandomActionType(int heroNo)
    {
        var Owner = Global.SceneMgr.CurrentScene as BattleScene;
        if (Owner != null)
        {
            var heroCont = BattleHeroManager.Instance.GetHeroControl(heroNo);
            if (heroCont != null)
            {
                // 상대방이 특정 행동만 하게 하려면 여기를 주석 걸고,
                heroCont.ActionType = (BattleHero.EAtionType)Random.Range(0, (int)BattleHero.EAtionType.ACTION_MAX);

                // 이곳의 주석을 풀고 해당 EAtionType 을 직접 넣어주면 된다.
                // heroCont.ActionType = Hero.EAtionType.ACTION_COUNT;
            }
        }
    }
}
