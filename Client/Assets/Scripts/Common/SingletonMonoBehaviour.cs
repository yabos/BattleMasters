using UnityEngine;
using System.Collections;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component, new()
{
    protected SingletonMonoBehaviour() { }

    protected static T m_instance = null;
    public static T Instance
    {
        get { return m_instance; }
    }
    public static bool IsValid { get { return (m_instance != null); } }

    public int GetReferenceCount()
    {
        int referenceCount = 0;

        System.Reflection.MemberInfo[] memberInfos = typeof(T).FindMembers(
            System.Reflection.MemberTypes.Field, System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public, null, null);
        foreach (System.Reflection.MemberInfo memberInfo in memberInfos)
        {
            object value = typeof(T).InvokeMember(
                memberInfo.Name, System.Reflection.BindingFlags.DeclaredOnly |
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField,
                null, this, null);
            if (value != null)
            {
                if (value.GetType().IsArray && value.GetType().GetElementType().IsClass)
                {
                    object[] objectArray = value as object[];
                    for (int i = 0; i < objectArray.Length; ++i)
                    {
                        object element = objectArray[i]; if (element != null) { referenceCount++; }
                    }
                }
                else if (value.GetType().IsGenericType)
                {
                    if (value is IList)
                    {
                        IList list = value as IList;
                        referenceCount += list.Count;
                    }
                    else if (value is IDictionary)
                    {
                        IDictionary dictionary = value as IDictionary;
                        referenceCount += dictionary.Count;
                    }
                }
                else if (value.GetType().IsClass)
                {
                    referenceCount++;
                }
            }
        }

        return referenceCount;
    }
}