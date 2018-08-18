#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionMaker), true)]
public class ActionMakerEditor : Editor
{
    ActionMaker _actionMaker;

    public void OnEnable()
    {
        if (PrefabUtility.GetPrefabType(target) == PrefabType.Prefab)
        {
            return;
        }

        _actionMaker = (ActionMaker)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUIUtility.labelWidth = 86;

        GUILayout.BeginHorizontal();
        {
            SerializedProperty sp = serializedObject.FindProperty("Loop");
            EditorGUILayout.PropertyField(sp, new GUIContent("Loop"));

        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Play", GUILayout.Width(50), GUILayout.Height(30)))
            {
                _actionMaker.PlayerOnce();
            }
        }
        GUILayout.EndHorizontal();        

        GUILayout.Space(25);
        GUILayout.BeginHorizontal();
        GUILayout.Label("-------------------------------------");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("------------ Set My Hero --------");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("-------------------------------------");
        GUILayout.EndHorizontal();

        DrawProperties("heroExcType", "myHero", "heroActionData");

        GUILayout.Space(25);

        GUILayout.BeginHorizontal();
        GUILayout.Label("-------------------------------------");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("------------ Set Enemy Hero -----");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("-------------------------------------");
        GUILayout.EndHorizontal();        

        DrawProperties("enmeyExcType", "enemyHero", "enemyActionData");

        serializedObject.ApplyModifiedProperties();
    }

    void DrawProperties(string heroExc, string heroProperty, string actionProperty)
    {
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Import", GUILayout.Width(150), GUILayout.Height(30)))
            {
                if (heroExc.Equals("heroExcType"))
                {
                    _actionMaker.LoadActionType(true);
                }
                else
                {
                    _actionMaker.LoadActionType(false);
                }
            }

            if (GUILayout.Button("Export", GUILayout.Width(150), GUILayout.Height(30)))
            {
                if (heroExc.Equals("heroExcType"))
                {
                    _actionMaker.ExportText(true);
                }
                else
                {
                    _actionMaker.ExportText(false);
                }
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            SerializedProperty sp = serializedObject.FindProperty(heroExc);
            EditorGUILayout.PropertyField(sp, new GUIContent("ActionType"));
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        {
            if (PrefabUtility.GetPrefabType(target) == PrefabType.Prefab)
            {
                return;
            }

            SerializedProperty sp = serializedObject.FindProperty(heroProperty);
            EditorGUILayout.PropertyField(sp, new GUIContent("Hero"));

        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        bool showChildren = true;
        var myIterator = serializedObject.FindProperty(actionProperty);

        CommendType currentCommend = CommendType.AnimationDelay;
        while (myIterator.NextVisible(showChildren))
        {
            // NextVisible() 함수는 serializedObject.FindProperty(actionProperty) 와
            // 상관없이 모든 ActionMaker의 변수를 뒤진다. 
            // 때문에 여기서 그려질 필요가 없는 건 넘겨준다.
            if (myIterator.name.Equals("heroExcType") ||
                myIterator.name.Equals("enmeyExcType") ||
                myIterator.name.Equals("Loop") ||
                myIterator.name.Equals("heroActionData") ||
                myIterator.name.Equals("enemyActionData") ||
                myIterator.name.Equals("myHero") ||
                myIterator.name.Equals("enemyHero"))
            {
                continue;
            }

            var myRect = GUILayoutUtility.GetRect(0f, 16f);
            showChildren = EditorGUI.PropertyField(myRect, myIterator);            
            if (myIterator.name.Equals("commend"))
            {
                currentCommend = (CommendType)myIterator.enumValueIndex;
            }
            else if (myIterator.name.Equals("duration"))
            {
                if (currentCommend == CommendType.AnimationDelay || currentCommend == CommendType.FadeOut)
                {
                    EditorGUI.BeginDisabledGroup(true);
                }
                else
                {
                    EditorGUI.EndDisabledGroup();
                }
            }            
            else
            {
                EditorGUI.EndDisabledGroup();
            }
        }        
    }
}
#endif