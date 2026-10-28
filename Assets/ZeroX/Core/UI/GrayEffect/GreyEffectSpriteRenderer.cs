using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ZeroX.UI
{
    public class GreyEffectSpriteRenderer : MonoBehaviour
    {
        [SerializeField] Shader m_shaderGrey;
        [SerializeField] string m_keyFloatGrey = "_EffectAmount";
        [SerializeField] float m_valueNormal = 0f, m_valueGrey = 1f;
        [SerializeField] bool m_isGrey = false;
        [SerializeField] bool m_updateOnStart = false;

        [Header("Component Grey")]
        [SerializeField] SpriteRenderer[] m_listSpriteRenderer;

        Material m_matGrey;
        void Start()
        {
            if (m_updateOnStart)
                SetGrey(true);
        }

        public bool IsGrey
        {
            get { return m_isGrey; }
            set
            {
                m_isGrey = value;
                SetGrey(m_isGrey);
            }
        }

        void SetGrey(bool value)
        {
            if(m_matGrey == null)
            {
                m_matGrey = new Material(m_shaderGrey);
                for (int i = 0; i < m_listSpriteRenderer.Length; i++)
                {
                    m_listSpriteRenderer[i].material = m_matGrey;
                }
            }          
                
            if (value)
                m_matGrey.SetFloat(m_keyFloatGrey, m_valueGrey);
            else
                m_matGrey.SetFloat(m_keyFloatGrey, m_valueNormal);
        }
        
    }
}
