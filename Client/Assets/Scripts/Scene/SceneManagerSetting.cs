using UnityEngine;
using System.Collections;

public class SceneManagerSetting : ManagerSettingBase
{
    private bool m_fading = false;
    public bool Fading
    {
        get { return m_fading; }
    }

    private Color m_color = Color.black;
    private Texture2D m_texture = null;

    private void Awake()
    {
        m_texture = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        int y = 0;
        while (y < m_texture.height)
        {
            int x = 0;
            while (x < m_texture.width)
            {
                m_texture.SetPixel(x, y, m_color);
                ++x;
            }
            ++y;
        }

        m_texture.wrapMode = TextureWrapMode.Repeat;
        m_texture.filterMode = FilterMode.Bilinear;

        m_texture.Apply();
    }

    void OnGUI()
    {
        if (m_fading == false)
            return;

        GUI.color = m_color;
        GUI.depth = -1000;

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_texture);
    }

    public IEnumerator OnFadeCoroutine(float duration, bool inverse)
    {
        if (duration <= 0.0f)
        {
            yield break;
        }

        m_fading = true;

        float t = 0.0f;
        while (t < 1.0f)
        {
            yield return new WaitForEndOfFrame();
            t = Mathf.Clamp01(t + Time.deltaTime / duration);

            m_color.a = inverse ? 1.0f - t : t;
        }

        m_fading = false;
    }
}
