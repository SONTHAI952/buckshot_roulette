using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ZeroX.UI
{
    public class GreyEffect : MonoBehaviour
    {
        [SerializeField] Shader m_shaderGrey;
        [SerializeField] string m_keyFloatGrey = "_EffectAmount";
        [SerializeField] float m_valueNormal = 0f, m_valueGrey = 1f;
        [SerializeField] bool m_isGrey = false;
        [SerializeField] bool m_updateOnStart = false;

        [Header("Button")]
        [SerializeField] Button m_button;
        [SerializeField] bool m_deactiveButtonWhenGrey = true;

        [Header("Component Grey")]
        [SerializeField] Image[] m_listImage;
        [SerializeField] TextMeshProUGUI[] m_listText;

        [Header("Color Text")]
        //[SerializeField] Color m_colorTextNormal;
        [SerializeField] Color m_colorTextGrey;

        Dictionary<TextMeshProUGUI, Color> m_dictColorTextNormal = null;
        Material m_matGrey;

        public Button ButtonAttached
        {
            get
            {
                return m_button;
            }
        }
        void Start()
        {
            if (m_updateOnStart)
                SetGrey(true);
        }

        void InitDictColorTextNormal()
        {
            m_dictColorTextNormal = new Dictionary<TextMeshProUGUI, Color>();
            TextMeshProUGUI text;
            for (int i = 0; i < m_listText.Length; i++)
            {
                text = m_listText[i];
                m_dictColorTextNormal.Add(text, text.color);
            }
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
            if (m_dictColorTextNormal == null)
                InitDictColorTextNormal();
            if(m_matGrey == null)
            {
                m_matGrey = new Material(m_shaderGrey);
                for (int i = 0; i < m_listImage.Length; i++)
                {
                    m_listImage[i].material = m_matGrey;
                }
            }

            if(!ReferenceEquals(m_button, null))
            {
                if (m_deactiveButtonWhenGrey)
                    m_button.interactable = !value;
                else
                {
                    m_button.interactable = true;
                }
            }
            
                

            if (value)
            {
                m_matGrey.SetFloat(m_keyFloatGrey, m_valueGrey);
                for (int i = 0; i < m_listText.Length; i++)
                {
                    m_listText[i].color = m_colorTextGrey;
                }
            }
            else
            {
                m_matGrey.SetFloat(m_keyFloatGrey, m_valueNormal);
                TextMeshProUGUI text;
                for (int i = 0; i < m_listText.Length; i++)
                {
                    text = m_listText[i];
                    text.color = m_dictColorTextNormal[text];
                    
                }
            }
        }
        
    }
}
