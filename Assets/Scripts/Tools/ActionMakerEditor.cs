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

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Export - MyHero", GUILayout.Width(150), GUILayout.Height(30)))
            {
                _actionMaker.ExportText(true);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Export - EnemyHero", GUILayout.Width(150), GUILayout.Height(30)))
            {
                _actionMaker.ExportText(false);
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
