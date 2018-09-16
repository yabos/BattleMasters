using UnityEngine;
using System.Collections; 

//해상도 고정 스크립트 2013 05 11 
public class CameraAspect : MonoBehaviour
{
    public float m_fHeight = 720;
    public float m_fWidth = 1280;

    // Use this for initialization 
    void Start()
    {
        GetComponent<Camera>().aspect =  m_fWidth/ m_fHeight;
    }
}