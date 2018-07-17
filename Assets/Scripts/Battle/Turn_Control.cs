using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn_Control : MonoBehaviour
{
    List<Hero_Control> ListMyHeroes = new List<Hero_Control>();
    List<Hero_Control> ListEnemyHeroes = new List<Hero_Control>();

    // Use this for initialization
    void Start ()
    {
        CreateTurnIcon();

        ListEnemyHeroes = GameMain.Instance().BattleControl.ListEnemyHeroes;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void CreateTurnIcon()
    {
        var listHero = GameMain.Instance().BattleControl.ListMyHeroes;
        for (int i = 0; i < listHero.Count; ++i)
        {
            GameObject goIcon = GameObject.Instantiate(VResources.Load<GameObject>("UI/Battle/TurnIcon")) as GameObject;
            if (goIcon != null)
            {
                goIcon.transform.parent = transform;
                goIcon.name = listHero[i].HeroNo.ToString();

                goIcon.transform.position = Vector3.zero;
                goIcon.transform.rotation = Quaternion.identity;
            }
        }
    }
}
