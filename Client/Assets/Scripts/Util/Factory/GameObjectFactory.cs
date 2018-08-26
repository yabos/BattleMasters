using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectFactory
{
    public static List<GameObject> GetRootGameObject()
    {
        List<GameObject> roots = new List<GameObject>();

        Transform[] transforms = Object.FindObjectsOfType<Transform>();
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i].parent == null)
            {
                roots.Add(transforms[i].gameObject);
            }
        }
        return roots;
    }

    public static GameObject FindRootGameObject(string tag)
    {

        List<GameObject> roots = GetRootGameObject();

        for (int i = 0; i < roots.Count; i++)
        {
            if (roots[i] == null)
                continue;

            if (roots[i].CompareTag(tag))
            {
                return roots[i];
            }
        }

        return null;
    }

    public static GameObject FindChildGameObject(GameObject fromGameObject, string withName)
    {
        if (fromGameObject == null)
        {
            return null;
        }

        Transform[] childTransforms = fromGameObject.transform.GetComponentsInChildren<Transform>();

        for (int i = 0; i < childTransforms.Length; i++)
        {
            if (childTransforms[i].gameObject.name == withName)
            {
                return childTransforms[i].gameObject;
            }
        }

        return null;
    }

    public static GameObject[] FindChildGameObjects(GameObject fromGameObject, string[] withNames)
    {

        if (fromGameObject == null)
        {
            return null;
        }

        List<GameObject> output = new List<GameObject>();

        Transform[] childTransforms = fromGameObject.transform.GetComponentsInChildren<Transform>();

        for (int i = 0; i < childTransforms.Length; i++)
        {
            if (childTransforms[i] == null || childTransforms[i].gameObject == null)
            {
                continue;
            }

            for (int j = 0; j < withNames.Length; j++)
            {
                if (childTransforms[i].gameObject.name == withNames[i])
                {
                    output.Add(childTransforms[i].gameObject);
                }
            }
        }

        return (output.Count != 0) ? output.ToArray() : null;
    }

    public static GameObject AddChild(GameObject fromParent, GameObject fromChild)
    {
        if (null == fromParent || null == fromChild)
            return null;

        fromChild.transform.SetParent(fromParent.transform, true);
        fromChild.transform.localPosition = Vector3.zero;
        fromChild.transform.localRotation = Quaternion.identity;
        fromChild.transform.localScale = Vector3.one;
        fromChild.layer = fromParent.layer;

        return fromChild;
    }

    public static T AddChild<T>(GameObject fromParent, GameObject fromChild) where T : Component
    {
        return AddChild(fromParent, fromChild).GetComponent<T>();
    }

    public static void Destroy(UnityEngine.GameObject unityObject)
    {
        //if (unityObject is Transform)
        //{
        //    Debug.LogError(" Destroying the transform component is not allowed.");
        //    return;
        //}

#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            GameObject.Destroy(unityObject);
        }
        else
        {
            GameObject.DestroyImmediate(unityObject);
        }
#else //UNITY_EDITOR
        GameObject.Destroy(unityObject);
#endif //UNITY_EDITOR
    }

    public static void DestroyComponent(UnityEngine.Object unityObject)
    {
        if (unityObject is Transform)
        {
            Debug.LogError(" Destroying the transform component is not allowed.");
            return;
        }

#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            GameObject.Destroy(unityObject);
        }
        else
        {
            GameObject.DestroyImmediate(unityObject);
        }
#else //UNITY_EDITOR
        GameObject.Destroy(unityObject);
#endif //UNITY_EDITOR
    }

    public static string TransformFullPath(Transform t)
    {
        string result = string.Empty;

        if (t.parent != null)
        {
            result = TransformFullPath(t.parent) + "/";
        }

        result += t.name;

        return result;
    }

    public static string GameObjectFullPath(GameObject go)
    {
        return TransformFullPath(go.transform);
    }

    public static void SetLayer(GameObject fromGameObject, int layerNumber, bool isChildren = true)
    {
        if (null == fromGameObject)
        {
            return;
        }

        if (isChildren == true)
        {

            Transform[] childTransforms = fromGameObject.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < childTransforms.Length; i++)
            {
                childTransforms[i].gameObject.layer = layerNumber;
            }
        }
        else
        {
            fromGameObject.layer = layerNumber;
        }
    }

    public static void SetLayer(GameObject fromGameObject, string layerName, bool isChildren = true)
    {
        SetLayer(fromGameObject, LayerMask.NameToLayer(layerName), isChildren);
    }
}