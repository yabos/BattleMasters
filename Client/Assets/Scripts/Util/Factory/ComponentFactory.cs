using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum IfNotExist
{
    AddNew,
    ReturnNull
};

public class ComponentFactory
{
    public static T GetComponent<T>(GameObject gameObject, IfNotExist ifNotExist = IfNotExist.AddNew) where T : Component
    {
        if (gameObject == null)
        {
            return null;
        }

        T t = gameObject.GetComponent<T>();
        if (t == null && ifNotExist == IfNotExist.AddNew)
        {
            t = AddComponent<T>(gameObject);
        }
        return t;
    }
    public static T AddComponent<T>(GameObject gameObject) where T : Component
    {
        return gameObject.AddComponent<T>();
    }

    public static T GetChildComponent<T>(GameObject parent, IfNotExist ifNotExist = IfNotExist.AddNew) where T : Component
    {
        if (parent == null)
        {
            return null;
        }

        T t = parent.GetComponentInChildren<T>();
        if (t == null && ifNotExist == IfNotExist.AddNew)
        {
            t = AddChildComponent<T>(parent);
        }
        return t;
    }

    public static T[] GetChildComponents<T>(GameObject parent, IfNotExist ifNotExist = IfNotExist.AddNew) where T : Component
    {
        if (parent == null)
        {
            return null;
        }

        T[] t = parent.GetComponentsInChildren<T>();
        if (t == null && ifNotExist == IfNotExist.AddNew)
        {
            t = new T[1];
            t[0] = AddChildComponent<T>(parent);
        }

        return t;
    }

    public static T[] GetChildComponents<T>(GameObject parent, bool includeInactive, IfNotExist ifNotExist = IfNotExist.AddNew) where T : Component
    {
        if (parent == null)
        {
            return null;
        }

        T[] t = parent.GetComponentsInChildren<T>(includeInactive);
        if (t == null && ifNotExist == IfNotExist.AddNew)
        {
            t = new T[1];
            t[0] = AddChildComponent<T>(parent);
        }

        return t;
    }

    public static T AddChildComponent<T>(GameObject parent) where T : Component
    {
        GameObject gameObj = new GameObject(typeof(T).ToString());

        if (parent != null)
        {
            gameObj.transform.SetParent(parent.transform, true);

            gameObj.transform.localPosition = Vector3.zero;
            gameObj.transform.rotation = Quaternion.identity;
            gameObj.transform.localScale = Vector3.one;
        }

        return gameObj.AddComponent<T>();
    }

    public static Transform FindDescendantByName(Transform parent, string childName)
    {
        // depth first search
        for (int i = 0; i < parent.childCount; ++i)
        {
            Transform child = parent.GetChild(i);
            if (child.name == childName)
            {
                return child;
            }

            Transform grandChild = FindDescendantByName(child, childName);
            if (grandChild != null)
            {
                return grandChild;
            }
        }

        return null;
    }

    public static GameObject FindDescendantGameObjectByName(Transform parent, string childName)
    {
        Transform transform = FindDescendantByName(parent, childName);
        return (transform != null) ? transform.gameObject : null;
    }


    public static T FindInAncestors<T>(GameObject gameObject) where T : Component
    {
        if (gameObject != null)
        {
            T component = gameObject.GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            if (gameObject.transform.parent != null)
            {
                return FindInAncestors<T>(gameObject.transform.parent.gameObject);
            }
        }

        return null;
    }

    public static T FindInParentSiblingsByName<T>(Transform parent, string childName) where T : Component
    {
        object component = null;

        while (parent != null)
        {
            // search children of parent
            for (int i = 0; i < parent.childCount; ++i)
            {
                Transform child = parent.GetChild(i);
                if (child.name == childName)
                {
                    component = child.gameObject.GetComponent<T>();
                    if (component != null)
                    {
                        return (T)component;
                    }
                }
            }

            // search amongst children of grandparent
            parent = parent.parent;
        }

        return (T)component;
    }

    public static Transform FindInDescendantsByName(Transform parent, string childName)
    {
        // depth first search
        for (int i = 0; i < parent.childCount; ++i)
        {
            Transform child = parent.GetChild(i);
            if (child.name == childName)
            {
                return child;
            }

            Transform grandChild = FindInDescendantsByName(child, childName);
            if (grandChild != null)
            {
                return grandChild;
            }
        }

        return null;
    }

    public static T FindInDescendantsByName<T>(Transform parent, string childName) where T : Component
    {
        T component = null;

        // depth first search
        for (int i = 0; i < parent.childCount; ++i)
        {
            Transform child = parent.GetChild(i);
            if (child.name == childName)
            {
                component = child.gameObject.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }

            component = FindInDescendantsByName<T>(child, childName);
            if (component != null)
            {
                break;
            }
        }

        return component;
    }

    public static T FindInChildrenByName<T>(Transform parent, string childName) where T : Component
    {
        T component = null;

        // depth first search
        for (int i = 0; i < parent.childCount; ++i)
        {
            Transform child = parent.GetChild(i);
            if (child.name == childName)
            {
                component = child.gameObject.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }
        }

        return component;
    }

    public static T GetComponent<T>(Transform transform, string path) where T : Component
    {
        T t = null;

        Transform ta = transform.Find(path);
        if (ta != null)
        {
            t = ta.GetComponent<T>();
            if (t == null)
            {
                Debug.LogError("Could not locate component<" + typeof(T).ToString() + "> on \"" + path + "\"", transform);
            }
        }
        else
        {
            return null;
        }
        return t;
    }
}