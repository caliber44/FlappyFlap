using System;
using UnityEngine;

public class PlayerSpriteAnimator : MonoBehaviour
{
    private static PlayerSpriteAnimator s_instance;

    private SpriteRenderer m_renderer;
    private int m_index;

    [SerializeField] Sprite[] m_sprites;

    private void Awake()
    {
        s_instance = this;


        if (s_instance != null)
        {
            Destroy(s_instance);
        }

        m_renderer = GetComponent<SpriteRenderer>();

        m_index = 0;

    }

    public static void NextSprite()
    {
        s_instance.SetNextSprite();
    }

    public static void ResetSprite() 
    {
        s_instance.SetFirstSprite();
    }
    private void SetNextSprite() 
    {
        m_index++;

        if(m_index >= m_sprites.Length)
        {
            m_index = 0;
        }

        SetSprite(m_index);

    }
    private void SetSprite(int index)
    {
        m_renderer.sprite = m_sprites[index];
    }
    private void SetFirstSprite()
    {
        m_renderer.sprite = m_sprites[0];
    }
}
