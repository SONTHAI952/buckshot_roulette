using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZeroX.UI
{
    public abstract class GrayEffectUI : GrayEffect
    {
        [Header("Button")]
        [SerializeField] protected Button m_button;
        [SerializeField] protected bool m_buttonInteractableWhenGray = false;

        [Header("Graphic")]
        [SerializeField] protected List<Graphic> m_listGraphic;
        
        [Header("Text")]
        [SerializeField] protected Color m_textColorWhenGray = Color.black;
        [SerializeField] protected List<TextMeshProUGUI> m_listText;
        
        public Button Button => m_button;

        public bool ButtonInteractableWhenGray
        {
            get => m_buttonInteractableWhenGray;
            set
            {
                m_buttonInteractableWhenGray = value;

                if (m_button != null)
                {
                    if (m_isGray)
                    {
                        m_button.interactable = m_buttonInteractableWhenGray;
                    }
                    else
                    {
                        m_button.interactable = true;
                    }
                }
            }
        }
    }
}