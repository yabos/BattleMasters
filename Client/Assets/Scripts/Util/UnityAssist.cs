using System.IO;
using UnityEngine;

public static class UnityAssist
{
    static public GameObject AttachObject(Transform parent, GameObject sp, bool Instantiate)
    {
        if (parent == null && sp == null)
            return null;

        GameObject go = null;
        if (Instantiate)
        {
            go = GameObject.Instantiate(sp) as GameObject;
        }
        else
        {
            go = sp;
        }

        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.SetParent(parent);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = parent.gameObject.layer;
        }

        return go;
    }


    static public Transform NextChildTransform(this Transform transform)
    {
        if (transform.childCount > 0)
        {
            return transform.GetChild(0);
        }

        Debug.LogError("NextChildTransform: " + transform.name);

        return null;
    }

    static public Transform FindChildTransform(this Transform transform, string name)
    {
        Transform tranchild = transform.Find(name);
        if (tranchild != null)
        {
            return tranchild;
        }
        else
        {
            Debug.LogWarning("FindChildGameObject: " + name);
        }
        return null;
    }

    static public GameObject FindChildGameObject(this Transform transform, string name)
    {
        Transform tranchild = transform.Find(name);
        if (tranchild != null)
        {
            return tranchild.gameObject;
        }
        else
        {
            Debug.LogWarning("FindChildGameObject" + name);
        }
        return null;
    }


    static public T FindChildComponent<T>(this Transform transform, string name) where T : Component
    {
        Transform tranchild = transform.Find(name);
        if (tranchild != null)
        {
            T findobj = tranchild.GetComponent<T>();
            return findobj;
        }
        else
        {
            Debug.LogError("FindChildComponent" + name);
        }
        return null;
    }

    static public T GetComponent<T>(MonoBehaviour mono) where T : Component
    {
        if (mono != null)
        {
            T findobj = mono.GetComponent<T>();
            if (findobj == null)
            {
                Debug.LogError("GetComponent" + mono.name);
            }
            return findobj;
        }
        return null;
    }

    static public T GetComponent<T>(GameObject mono) where T : Component
    {
        if (mono != null)
        {
            T findobj = mono.GetComponent<T>();
            if (findobj == null)
            {
                Debug.LogError("GetComponent" + mono.name);
            }
            return findobj;
        }
        return null;
    }

    static public Transform FindChildDeep(this Transform trans, string name)
    {
        Transform found = trans.Find(name);
        if (found == null)
        {
            for (int i = 0; i < trans.childCount; i++)
            {
                Transform child = FindChildDeep(trans.GetChild(i), name);
                if (child != null)
                    return child;
            }
        }
        return found;
    }

    static public string GetFileName(string srcPath, bool bRemoveExtension)
    {
        string name = srcPath;
        int lastSlashIndex;

        lastSlashIndex = name.LastIndexOfAny(new char[] { '/', '\\' });
        if (lastSlashIndex >= 0)
        {
            name = name.Substring(lastSlashIndex + 1);
        }

        if (bRemoveExtension)
        {
            int lastDotIndex = name.LastIndexOf('.');
            if (lastDotIndex >= 0)
            {
                name = name.Substring(0, lastDotIndex);
            }
        }

        return name;
    }

    static public string GetFileExt(string srcPath)
    {
        if (null == srcPath)
        {
            return null;
        }

        int lastDotIndex = srcPath.LastIndexOfAny(new char[] { '/', '\\', '.' });

        if (lastDotIndex < 0 ||
            srcPath[lastDotIndex] == '/' ||
            srcPath[lastDotIndex] == '\\')
        {
            return string.Empty;
        }
        // The extension of the specified path (including the period ".")
        return srcPath.Substring(lastDotIndex);
    }

    static public string GetDirectory(string srcPath)
    {
        int lastSlashIndex = srcPath.LastIndexOfAny(new char[] { '/', '\\' });
        string directory = (lastSlashIndex > 0) ? srcPath.Substring(0, lastSlashIndex) : string.Empty;
        return directory;
    }

    static public string CombinePath(string lhPath, string rhPath)
    {
        if ((lhPath == null) || (rhPath == null))
        {
            throw new System.ArgumentNullException((lhPath == null) ? "lhPath" : "rhPath");
        }
        if (rhPath.Length == 0)
        {
            return lhPath;
        }
        if (lhPath.Length == 0)
        {
            return rhPath;
        }

        string output = lhPath + (lhPath.EndsWith(Path.DirectorySeparatorChar.ToString()) ? rhPath : Path.DirectorySeparatorChar + rhPath);

        return output;
    }

    static public bool GetActive(Behaviour mb)
    {
        return mb && mb.enabled && mb.gameObject.activeInHierarchy;
    }
}