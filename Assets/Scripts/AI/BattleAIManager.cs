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

    BattleManager Owner;

    bool ProcAI = false;
    float TimeElapsed = 0;
    eAI_Proc CurAI = eAI_Proc.AI_Normal;

    public void Initialize(BattleManager owner)
    {
        Owner = owner;

        Initialize();
    }

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
                        // 살아있는 상대방 적 1인 랜덤으로 선택 해줌.
                        int targetHeroNo = Owner.GetRandomHeroTeam();
                        Owner.ActiveTargetHeroNo = targetHeroNo;
                        Owner.SetOutlineHero(targetHeroNo);
                        Owner.BattleUI.SetProfileUI(targetHeroNo, false);
                        Owner.BattleUI.ActiveBattleProfile(true, true);

                        // 한가지 공격 타입을 설정
                        Owner.SetRandomActionType(Owner.ActiveTurnHeroNo);

                        // 상대방 턴일 경우에는 엑션 유형 선택은 파란색 ui 
                        Owner.BattleUI.ActiveSelActionType(true, false);
                        Owner.BattleUI.SetTurnTimer(Define.SELECT_ACTIONTYPE_LIMITTIME, ETurnTimeType.TURNTIME_SEL_ACTIONTYPE);

                        CurAI = eAI_Proc.AI_Action;
                        TimeElapsed = 0;                        
                    }
                }
                break;

            case eAI_Proc.AI_Action:
                {
                 
                }
                break;
        }
    }
}
